using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;

namespace Server.Commands
{
    public class DotCommand_Spec
    {
        public static void Initialize()
        {
            CommandSystem.Register( "Spec", AccessLevel.Player, new CommandEventHandler( OnCommand_Spec ) );
        }

        [Usage( "Spec" )]
        [Description( "Allows you you to choose your specializations" )]
        private static void OnCommand_Spec( CommandEventArgs e )
        {
            new SpecInstance( e.Mobile );
        }
    }

    public class SpecInstance
    {
        private int currentPage;
        private int maxPages = 5;

        private SkillName[] m_Specials, m_Primaries, m_Secondaries, m_Tradeskills;
        private int m_buttonID;
        private Mobile player;

        public SpecInstance( Mobile theMobile )
        {
            currentPage = 0;
            player = theMobile;

            //Load the player's current specs
            PlayerMobile pm = (PlayerMobile)player;
            LoadPlayerSpecs( pm );
            SendSpecGump();
        }

        private void LoadPlayerSpecs( PlayerMobile player )
        {
            if ( (SkillName[])player.SpecialSkills != null )
                m_Specials = (SkillName[])player.SpecialSkills.Clone();
            if ( (SkillName[])player.PrimarySkills != null )
                m_Primaries = (SkillName[])player.PrimarySkills.Clone();
            if ( (SkillName[])player.SecondarySkills != null )
                m_Secondaries = (SkillName[])player.SecondarySkills.Clone();
            if ((SkillName[])player.TradeSkills != null)
                m_Tradeskills = (SkillName[])player.TradeSkills.Clone();
        }

        private void SavePlayerSpecs( PlayerMobile player )
        {
            List<SkillName> newSkills = new List<SkillName>();

            newSkills.AddRange( m_Specials );
            newSkills.AddRange( m_Primaries );
            newSkills.AddRange( m_Secondaries );

            List<SkillName> oldSkills = new List<SkillName>();

            oldSkills.AddRange( player.SpecialSkills );
            oldSkills.AddRange( player.PrimarySkills );
            oldSkills.AddRange( player.SecondarySkills );

            //foreach ( SkillName skill in oldSkills ) {
            //    if ( newSkills.Contains( skill ) )
            //        continue;
            //    else if ( player.Skills[skill].Base < 50 )
            //        player.Skills[skill].BaseFixedPoint = 0;
            //}


            player.SpecialSkills = (SkillName[])m_Specials.Clone();
            player.PrimarySkills = (SkillName[])m_Primaries.Clone();
            player.SecondarySkills = (SkillName[])m_Secondaries.Clone();
            player.TradeSkills = (SkillName[])m_Tradeskills.Clone();
        }

        private void SendSpecGump()
        {
            player.SendGump( new SpecGump( player, this, currentPage ) );
        }

        public SkillName[] CurrentSpecials { get { return m_Specials; } }
        public SkillName[] CurrentPrimaries { get { return m_Primaries; } }
        public SkillName[] CurrentSecondaries { get { return m_Secondaries; } }
        public SkillName[] CurrentTradeskills { get { return m_Tradeskills; } }

        public void SpecBookButtonPress( int buttonID )
        {
            if ( buttonID == 0 ) //Gump was closed
            {
                player.SendMessage( "No changes were made." );
                return;
            }
            else if ( buttonID == GumpUtil.BUTTONID_LAST_PAGE ) {
                currentPage--;
                if ( currentPage < 0 )
                    currentPage = 0;
                SendSpecGump();
                return;
            }
            else if ( buttonID == GumpUtil.BUTTONID_NEXT_PAGE ) {
                currentPage++;
                if ( currentPage > maxPages )
                    currentPage = maxPages;
                SendSpecGump();
                return;
            }
            else if ( buttonID == GumpUtil.BUTTONID_CONFIRM ) {
                //Verify that all the skills are selected
                for ( int i = 0; i < PlayerMobile.SpecialsPermitted; i++ ) {
                    if ( m_Specials[i] == SkillName.TasteID ) {
                        player.SendMessage( "You must select all skills before you save" );
                        SendSpecGump();
                        return;
                    }
                }
                for ( int i = 0; i < PlayerMobile.PrimariesPermitted; i++ ) {
                    if ( m_Primaries[i] == SkillName.TasteID ) {
                        player.SendMessage( "You must select all skills before you save" );
                        SendSpecGump();
                        return;
                    }
                }
                for ( int i = 0; i < PlayerMobile.SecondariesPermitted; i++ ) {
                    if ( m_Secondaries[i] == SkillName.TasteID ) {
                        player.SendMessage( "You must select all skills before you save" );
                        SendSpecGump();
                        return;
                    }
                }
                for ( int i = 0; i < PlayerMobile.TradeskillsPermitted; i++ ) {
                    if ( m_Tradeskills[i] == SkillName.TasteID ) {
                        player.SendMessage( "You must select all skills before you save" );
                        SendSpecGump();
                        return;
                    }
                }

                //Save the specs
                PlayerMobile pm = (PlayerMobile)player;
                SavePlayerSpecs( pm );
                pm.CheckSkillCaps();

                player.SendMessage( "The changes have been saved." );
                return;
            }
            else {
                //Ok, we're trying to change a skill spec slot
                m_buttonID = buttonID;
                if ( currentPage == 1 ) {
                    player.SendGump( new SkillSelectGump( player, this, 0 ) );
                    return;
                }
                else if ( currentPage == 2 ) {
                    player.SendGump( new SkillSelectGump( player, this, 0 ) );
                    return;
                }
                else if ( currentPage == 3 ) {
                    player.SendGump( new SkillSelectGump( player, this, 1 ) );
                    return;
                }
                else if ( currentPage == 4 ) {
                    player.SendGump( new SkillSelectGump( player, this, 2 ) );
                    return;
                }
                else {
                    player.SendMessage( "Error:  invalid gump return" );
                }

            }
        }

        public void SkillSelected( int selectedSkillNum )
        {
            if ( selectedSkillNum < 0 ) {
                player.SendMessage( "Canceled." );
                SendSpecGump();
                return;
            }

            SkillName selectedSkill = (SkillName)selectedSkillNum;

            //Now we make sure that we didn't already pick that skill
            if ( IsAlreadySelectedSkill( selectedSkill ) ) {
                player.SendMessage( "But you already have that skill selected!" );
                SendSpecGump();
                return;
            }

            //Now we figure out which arraylist to use
            if ( currentPage == 1 ) {
                m_Specials[m_buttonID - 1] = selectedSkill;
            }
            else if ( currentPage == 2 ) {
                m_Primaries[m_buttonID - 1] = selectedSkill;
            }
            else if ( currentPage == 3 ) {
                m_Secondaries[m_buttonID - 1] = selectedSkill;
            }
            else if ( currentPage == 4 ) {
                m_Tradeskills[m_buttonID - 1] = selectedSkill;
            }
            else {
                player.SendMessage( "You should not be seeing this because its an error." );
                return;
            }

            SendSpecGump();
            return;
        }

        private bool IsAlreadySelectedSkill( SkillName theSkill )
        {
            foreach ( SkillName thisSkill in m_Specials ) {
                if ( thisSkill == theSkill )
                    return true;
            }
            foreach ( SkillName thisSkill in m_Primaries ) {
                if ( thisSkill == theSkill )
                    return true;
            }
            foreach ( SkillName thisSkill in m_Secondaries ) {
                if ( thisSkill == theSkill )
                    return true;
            }
            foreach ( SkillName thisSkill in m_Tradeskills ) {
                if ( thisSkill == theSkill )
                    return true;
            }
            return false;
        }
    }


    public class SpecGump : Gump
    {
        public const int gumpOffsetX = 50;
        public const int gumpOffsetY = 50;
        private int currentPage;
        private SpecInstance m_parent;

        public SpecGump( Mobile callingPlayer, SpecInstance parentInstance )
            : this( callingPlayer, parentInstance, 0 )
        {
        }

        public SpecGump( Mobile callingPlayer, SpecInstance parentInstance, int requestedPage )
            : base( gumpOffsetX, gumpOffsetY )
        {
            currentPage = requestedPage;
            m_parent = parentInstance;

            //Close the gump if they already have one open
            callingPlayer.CloseGump( typeof( SpecGump ) );

            BuildCurrentGumpPage( callingPlayer );
        }


        public void BuildCurrentGumpPage( Mobile callingPlayer )
        {
            AddPage( 0 );
            AddImage( gumpOffsetX, gumpOffsetY, GumpUtil.GoldBordedBook );

            switch ( currentPage ) {
                case 0: {
                        AddButton( ( gumpOffsetX + 356 ), gumpOffsetY, GumpUtil.GoldBordedBookNextPage, GumpUtil.GoldBordedBookNextPage, GumpUtil.BUTTONID_NEXT_PAGE, GumpButtonType.Reply, 0 );
                        AddLabel( ( gumpOffsetX + 40 ), ( gumpOffsetY + 15 ), 1323, ".Spec" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 40 ), 0, "Normally, all skills are" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 55 ), 0, "limited to a low level." );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 70 ), 0, "Each character can," );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 85 ), 0, "however, select a number" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 100 ), 0, "of skills to 'specialize'" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 115 ), 0, "in. These skills can rise" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 130 ), 0, "to a higher level than" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 145 ), 0, "non-specialized skills." );

                        AddLabel( ( gumpOffsetX + 220 ), ( gumpOffsetY + 40 ), 0, "On the following pages," );
                        AddLabel( ( gumpOffsetX + 220 ), ( gumpOffsetY + 55 ), 0, "select the skills you wish" );
                        AddLabel( ( gumpOffsetX + 220 ), ( gumpOffsetY + 70 ), 0, "to specialize in.  No" );
                        AddLabel( ( gumpOffsetX + 220 ), ( gumpOffsetY + 85 ), 0, "actual changes will be" );
                        AddLabel( ( gumpOffsetX + 220 ), ( gumpOffsetY + 100 ), 0, "made until you press the" );
                        AddLabel( ( gumpOffsetX + 220 ), ( gumpOffsetY + 115 ), 0, "'SAVE' button on the" );
                        AddLabel( ( gumpOffsetX + 220 ), ( gumpOffsetY + 130 ), 0, "last page." );
                        break;
                    }

                case 1: {
                        AddButton( gumpOffsetX, gumpOffsetY, GumpUtil.GoldBordedBookLastPage, GumpUtil.GoldBordedBookLastPage, GumpUtil.BUTTONID_LAST_PAGE, GumpButtonType.Reply, 0 );
                        AddButton( ( gumpOffsetX + 356 ), gumpOffsetY, GumpUtil.GoldBordedBookNextPage, GumpUtil.GoldBordedBookNextPage, GumpUtil.BUTTONID_NEXT_PAGE, GumpButtonType.Reply, 0 );
                        AddLabel( ( gumpOffsetX + 40 ), ( gumpOffsetY + 35 ), 1323, "Specials:" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 60 ), 0, "You can have " + PlayerMobile.SpecialsPermitted );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 75 ), 0, "special skill.  This" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 90 ), 0, "skill can be raised up" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 105 ), 0, "to a limit of 110." );

                        int zSpacer = (int)( 140 / PlayerMobile.SpecialsPermitted );
                        for ( int i = 0; i < PlayerMobile.SpecialsPermitted; i++ ) {
                            AddButton( ( gumpOffsetX + 230 ), ( gumpOffsetY + 35 + ( i * zSpacer ) ), GumpUtil.ButtonBlueUp, GumpUtil.ButtonBlueDown, ( i + 1 ), GumpButtonType.Reply, 0 );
                            if ( i < m_parent.CurrentSpecials.Length && m_parent.CurrentSpecials[i] != SkillName.TasteID ) {
                                AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, callingPlayer.Skills[(SkillName)m_parent.CurrentSpecials[i]].Name );
                            }
                            else {
                                AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, "-Empty-" );
                            }
                        }

                        break;
                    }

                case 2: {
                        AddButton( gumpOffsetX, gumpOffsetY, GumpUtil.GoldBordedBookLastPage, GumpUtil.GoldBordedBookLastPage, GumpUtil.BUTTONID_LAST_PAGE, GumpButtonType.Reply, 0 );
                        AddButton( ( gumpOffsetX + 356 ), gumpOffsetY, GumpUtil.GoldBordedBookNextPage, GumpUtil.GoldBordedBookNextPage, GumpUtil.BUTTONID_NEXT_PAGE, GumpButtonType.Reply, 0 );
                        AddLabel( ( gumpOffsetX + 40 ), ( gumpOffsetY + 35 ), 1323, "Primaries:" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 60 ), 0, "You can have up to " + PlayerMobile.PrimariesPermitted );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 75 ), 0, "primary skills.  These" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 90 ), 0, "skills can be raised up" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 105 ), 0, "to a limit of 100." );

                        int zSpacer = (int)( 140 / PlayerMobile.PrimariesPermitted );
                        for ( int i = 0; i < PlayerMobile.PrimariesPermitted; i++ ) {
                            AddButton( ( gumpOffsetX + 230 ), ( gumpOffsetY + 35 + ( i * zSpacer ) ), GumpUtil.ButtonBlueUp, GumpUtil.ButtonBlueDown, ( i + 1 ), GumpButtonType.Reply, 0 );
                            if ( i < m_parent.CurrentPrimaries.Length && m_parent.CurrentPrimaries[i] != SkillName.TasteID ) {
                                AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, callingPlayer.Skills[(SkillName)m_parent.CurrentPrimaries[i]].Name );
                            }
                            else {
                                AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, "-Empty-" );
                            }
                        }

                        break;
                    }

                case 3: {
                        AddButton( gumpOffsetX, gumpOffsetY, GumpUtil.GoldBordedBookLastPage, GumpUtil.GoldBordedBookLastPage, GumpUtil.BUTTONID_LAST_PAGE, GumpButtonType.Reply, 0 );
                        AddButton( ( gumpOffsetX + 356 ), gumpOffsetY, GumpUtil.GoldBordedBookNextPage, GumpUtil.GoldBordedBookNextPage, GumpUtil.BUTTONID_NEXT_PAGE, GumpButtonType.Reply, 0 );
                        AddLabel( ( gumpOffsetX + 40 ), ( gumpOffsetY + 35 ), 1323, "Secondaries:" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 60 ), 0, "You can have up to " + PlayerMobile.SecondariesPermitted );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 75 ), 0, "secondary skills.  These" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 90 ), 0, "skills can be raised up" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 105 ), 0, "to a limit of 80. If" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 120 ), 0, "you select a trade-skill," );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 135 ), 0, "it can be raised to 100." );

                        int zSpacer = (int)( 140 / PlayerMobile.SecondariesPermitted );
                        for ( int i = 0; i < PlayerMobile.SecondariesPermitted; i++ ) {
                            AddButton( ( gumpOffsetX + 230 ), ( gumpOffsetY + 35 + ( i * zSpacer ) ), GumpUtil.ButtonBlueUp, GumpUtil.ButtonBlueDown, ( i + 1 ), GumpButtonType.Reply, 0 );
                            if ( i < m_parent.CurrentSecondaries.Length && m_parent.CurrentSecondaries[i] != SkillName.TasteID ) {
                                AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, callingPlayer.Skills[(SkillName)m_parent.CurrentSecondaries[i]].Name );
                            }
                            else {
                                AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, "-Empty-" );
                            }
                        }

                        break;
                    }
                
            case 4: {
                    AddButton( gumpOffsetX, gumpOffsetY, GumpUtil.GoldBordedBookLastPage, GumpUtil.GoldBordedBookLastPage, GumpUtil.BUTTONID_LAST_PAGE, GumpButtonType.Reply, 0 );
                    AddButton( ( gumpOffsetX + 356 ), gumpOffsetY, GumpUtil.GoldBordedBookNextPage, GumpUtil.GoldBordedBookNextPage, GumpUtil.BUTTONID_NEXT_PAGE, GumpButtonType.Reply, 0 );
                    AddLabel( ( gumpOffsetX + 40 ), ( gumpOffsetY + 35 ), 1323, "Tradeskills:" );
                    AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 60 ), 0, "You can always select " + PlayerMobile.TradeskillsPermitted );
                    AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 75 ), 0, "trade skill. This" );
                    AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 90 ), 0, "skill can be raised up" );
                    AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 105 ), 0, "to a limit of 100, but" );
                    AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 120 ), 0, "may not be a combat-" );
                    AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 135 ), 0, "related skill." );

                    int zSpacer = (int)( 140 / PlayerMobile.TradeskillsPermitted );
                    for ( int i = 0; i < PlayerMobile.TradeskillsPermitted; i++ ) {
                        AddButton( ( gumpOffsetX + 230 ), ( gumpOffsetY + 35 + ( i * zSpacer ) ), GumpUtil.ButtonBlueUp, GumpUtil.ButtonBlueDown, ( i + 1 ), GumpButtonType.Reply, 0 );
                        if ( i < m_parent.CurrentTradeskills.Length && m_parent.CurrentTradeskills[i] != SkillName.TasteID ) {
                            AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, callingPlayer.Skills[(SkillName)m_parent.CurrentTradeskills[i]].Name );
                        }
                        else {
                            AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, "-Empty-" );
                        }
                    }
                    break;
                }

                case 5: {
                        AddButton( gumpOffsetX, gumpOffsetY, GumpUtil.GoldBordedBookLastPage, GumpUtil.GoldBordedBookLastPage, GumpUtil.BUTTONID_LAST_PAGE, GumpButtonType.Reply, 0 );
                        AddLabel( ( gumpOffsetX + 40 ), ( gumpOffsetY + 35 ), 1323, "Save Changes:" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 60 ), 0, "Once you save changes" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 75 ), 0, "made, any skill above" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 90 ), 0, "its new max. will be" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 105 ), 0, "capped. Only save once" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 120 ), 0, "you are certain you" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 135 ), 0, "are happy with the" );
                        AddLabel( ( gumpOffsetX + 50 ), ( gumpOffsetY + 150 ), 0, "changes you've made" );

                        AddButton( ( gumpOffsetX + 230 ), ( gumpOffsetY + 170 ), GumpUtil.ButtonRed, GumpUtil.ButtonRed, GumpUtil.BUTTONID_CONFIRM, GumpButtonType.Reply, 0 );
                        AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 166 ), 32, "SAVE CHANGES" );
                        break;
                    }
            }
        }

        //Handles button presses
        public override void OnResponse( NetState state, RelayInfo info )
        {
            m_parent.SpecBookButtonPress( info.ButtonID );
        }
    }
    
    public class SkillSelectGump : Gump
    {
        public const int gumpOffsetX = 50;
        public const int gumpOffsetY = 50;
        private SkillName[] m_AvailableSkills;
        private SpecInstance m_parentSpecInstance;

        public SkillSelectGump( Mobile callingPlayer, SpecInstance parentSpecInstance )
            : this( callingPlayer, parentSpecInstance, 0 )
        {
        }

        public SkillSelectGump( Mobile callingPlayer, SpecInstance parentSpecInstance, int showTradeSkills )
            : base( gumpOffsetX, gumpOffsetY )
        {
            //Close the gump if they already have one open
            callingPlayer.CloseGump( typeof( SkillSelectGump ) );

            if ( showTradeSkills == 0 )
                m_AvailableSkills = SkillMaster.CombatSkills;
            else if (showTradeSkills == 1)
                m_AvailableSkills = SkillMaster.AllSkills;
            else
                m_AvailableSkills = SkillMaster.TradeSkills;

            m_parentSpecInstance = parentSpecInstance;
            BuildCurrentGumpPage( callingPlayer );
        }

        public void BuildCurrentGumpPage( Mobile callingPlayer )
        {
            int skillsPerPage = 6;

            AddPage( 0 );
            AddImage( gumpOffsetX, gumpOffsetY, 0x01F4 );

            int totalSkills = m_AvailableSkills.Length;
            int totalPages = totalSkills / ( skillsPerPage * 2 );
            if ( ( totalSkills % ( skillsPerPage * 2 ) ) != 0 )
                totalPages++;

            int zSpacer = (int)( 180 / skillsPerPage );

            for ( int pageNum = 1; pageNum < ( totalPages + 1 ); pageNum++ ) {
                AddPage( pageNum );
                if ( pageNum > 1 )
                    AddButton( gumpOffsetX, gumpOffsetY, GumpUtil.GoldBordedBookLastPage, GumpUtil.GoldBordedBookLastPage, GumpUtil.BUTTONID_LAST_PAGE, GumpButtonType.Page, ( pageNum - 1 ) );
                if ( pageNum < totalPages )
                    AddButton( ( gumpOffsetX + 356 ), gumpOffsetY, GumpUtil.GoldBordedBookNextPage, GumpUtil.GoldBordedBookNextPage, GumpUtil.BUTTONID_NEXT_PAGE, GumpButtonType.Page, ( pageNum + 1 ) );

                int startingSkillNum = ( 2 * ( pageNum - 1 ) ) * skillsPerPage;
                for ( int i = 0; i < skillsPerPage; i++ ) {
                    int currentSkillNum = startingSkillNum + i;
                    if ( currentSkillNum < m_AvailableSkills.Length ) {
                        SkillName currentSkill = (SkillName)m_AvailableSkills[currentSkillNum];

                        AddButton( ( gumpOffsetX + 50 ), ( gumpOffsetY + 35 + ( i * zSpacer ) ), GumpUtil.ButtonBlueUp, GumpUtil.ButtonBlueDown, ( currentSkillNum + 1 ), GumpButtonType.Reply, 0 );
                        AddLabel( ( gumpOffsetX + 68 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, callingPlayer.Skills[currentSkill].Name );
                    }
                }

                startingSkillNum = ( 2 * ( pageNum - 1 ) + 1 ) * skillsPerPage;
                for ( int i = 0; i < skillsPerPage; i++ ) {
                    int currentSkillNum = startingSkillNum + i;
                    if ( currentSkillNum < m_AvailableSkills.Length ) {
                        SkillName currentSkill = (SkillName)m_AvailableSkills[currentSkillNum];

                        AddButton( ( gumpOffsetX + 230 ), ( gumpOffsetY + 35 + ( i * zSpacer ) ), GumpUtil.ButtonBlueUp, GumpUtil.ButtonBlueDown, ( currentSkillNum + 1 ), GumpButtonType.Reply, 0 );
                        AddLabel( ( gumpOffsetX + 248 ), ( gumpOffsetY + 32 + ( i * zSpacer ) ), 0, callingPlayer.Skills[currentSkill].Name );
                    }
                }
            }
        }

        //Handles button presses
        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile player = state.Mobile;
            if ( info.ButtonID == 0 ) //Gump was closed
            {
                player.SendMessage( "No changes were made." );
                m_parentSpecInstance.SkillSelected( -1 );
                return;
            }

            int selectedSkillNum = info.ButtonID - 1;
            if ( selectedSkillNum > m_AvailableSkills.Length ) {
                player.SendMessage( "Error:  that skill wasn't even available!" );
                return;
            }

            SkillName selectedSkillID = (SkillName)m_AvailableSkills[selectedSkillNum];
            m_parentSpecInstance.SkillSelected( (int)selectedSkillID );
        }
    }
}