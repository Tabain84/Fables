using System;
using System.Collections.Generic;
using System.Text;

using Server.Gumps;
using Server.Mobiles;
using Server.Network;

using Server.Commands;

namespace Server.Training
{
    public class TrainGump : Gump
    {
        private object[] m_State;
        private int spendingExp;
        private SkillName toTrain;

        /*public static void Initialize()
        {
            CommandSystem.Register( "Train", AccessLevel.Owner, new CommandEventHandler( Train_OnCommand ) );
        }


        [Usage( "train" )]
        [Description( "Opens a gump for manipulating areas of items." )]
        private static void Train_OnCommand( CommandEventArgs e )
        {
            if ( e.Mobile.HasGump( typeof( TrainGump ) ) )
                e.Mobile.CloseGump( typeof( TrainGump ) );

            int spendingExp = 0;
            SkillName toTrain = SkillName.TasteID;

            object[] state = new object[2];

            state[0] = spendingExp;
            state[1] = toTrain;

            e.Mobile.SendGump( new TrainGump( e.Mobile as PlayerMobile, state ) );

        }*/

        public TrainGump( PlayerMobile from, object[] state )
            : base( 0, 0 )
        {
            m_State = state;
            MakeTrainGump( from );
        }
        private void MakeTrainGump( PlayerMobile from )
        {
            this.Closable = false;
            this.Disposable = false;
            this.Dragable = true;
            this.Resizable = false;

            spendingExp = (int)m_State[0];
            toTrain = (SkillName)m_State[1];

            AddPage( 0 );
            AddBackground( 15, 60, 408, 288, 9350 );

            AddLabel( 75, 75, 55, @"Skill:" );
            AddLabel( 210, 75, 55, @"Current:" );
            AddLabel( 285, 75, 55, @"Cap:" );
            AddLabel( 340, 75, 55, @"Increase:" );

            AddLabel( 25, 290, 55, @"Current Exp:" );
            AddLabel( 125, 290, 55, @"Spending:" );
            AddLabel( 225, 290, 55, @"Select Exp:" );

            AddLabel( 30, 310, 1153, from.CurrentExperience.ToString() ); // Current Exp
            AddLabel( 125, 310, 1153, spendingExp.ToString() ); // Selected Exp

            AddTextEntry( 225, 310, 70, 20, 1153, 1, spendingExp.ToString(), 7 ); //Enter exp to spend
            AddImageTiled( 225, 330, 70, 1, 9274 );

            if ( toTrain == SkillName.TasteID )
                AddButton( 340, 290, 239, 240, 20, GumpButtonType.Reply, 0 ); // Apply
            else
                AddButton( 340, 290, 247, 248, 30, GumpButtonType.Reply, 0 ); // Okay

            AddButton( 340, 315, 242, 241, 0, GumpButtonType.Reply, 0 ); // Cancel

            //Add Skill Labels
            int skillHue;
            int ySpacer = 20;
            for ( int i = 0; i < PlayerMobile.SpecialsPermitted; i++ ) {

                if ( toTrain != SkillName.TasteID && toTrain == from.SpecialSkills[i] )
                    skillHue = 36;
                else
                    skillHue = 352;

                if ( from.SpecialSkills[i] != SkillName.TasteID ) {
                    AddLabel( 50, 77 + ySpacer, skillHue, from.Skills[from.SpecialSkills[i]].Name ); // Dynamic Skill Label
                    AddLabel( 220, 77 + ySpacer, 1153, from.Skills[from.SpecialSkills[i]].Base.ToString() ); // Current Skill #
                    AddLabel( 285, 77 + ySpacer, 1153, from.GetSkillCap( from.SpecialSkills[i] ).ToString() ); // Skill Cap #
                    AddLabel( 345, 77 + ySpacer, 1153, String.Format( "+{0:0.##}", TrainMaster.GetTrainingIncrease( from, from.SpecialSkills[i], spendingExp ) ) ); // Increase Skill #
                }
                else
                    AddLabel( 50, 77 + ySpacer, skillHue, "-Empty-" ); // Dynamic Skill Label

                ySpacer += 20;

            }
            for ( int i = 0; i < PlayerMobile.PrimariesPermitted; i++ ) {

                if ( toTrain != SkillName.TasteID && toTrain == from.PrimarySkills[i] )
                    skillHue = 36;
                else
                    skillHue = 352;

                if ( from.PrimarySkills[i] != SkillName.TasteID ) {
                    AddLabel( 50, 77 + ySpacer, skillHue, from.Skills[from.PrimarySkills[i]].Name ); // Dynamic Skill Label
                    AddLabel( 220, 77 + ySpacer, 1153, from.Skills[from.PrimarySkills[i]].Base.ToString() ); // Current Skill #
                    AddLabel( 285, 77 + ySpacer, 1153, from.GetSkillCap( from.PrimarySkills[i] ).ToString() ); // Skill Cap #
                    AddLabel( 345, 77 + ySpacer, 1153, String.Format( "+{0:0.##}", TrainMaster.GetTrainingIncrease( from, from.PrimarySkills[i], spendingExp ) ) ); // Increase Skill #
                }
                else
                    AddLabel( 50, 77 + ySpacer, skillHue, "-Empty-" ); // Dynamic Skill Label

                ySpacer += 20;

            }
            for ( int i = 0; i < PlayerMobile.SecondariesPermitted; i++ ) {

                if ( toTrain != SkillName.TasteID && toTrain == from.SecondarySkills[i] )
                    skillHue = 36;
                else
                    skillHue = 352;

                if ( from.SecondarySkills[i] != SkillName.TasteID ) {
                    AddLabel( 50, 77 + ySpacer, skillHue, from.Skills[from.SecondarySkills[i]].Name ); // Dynamic Skill Label
                    AddLabel( 220, 77 + ySpacer, 1153, from.Skills[from.SecondarySkills[i]].Base.ToString() ); // Current Skill #
                    AddLabel( 285, 77 + ySpacer, 1153, from.GetSkillCap( from.SecondarySkills[i] ).ToString() ); // Skill Cap #
                    AddLabel( 345, 77 + ySpacer, 1153, String.Format( "+{0:0.##}", TrainMaster.GetTrainingIncrease( from, from.SecondarySkills[i], spendingExp ) ) ); // Increase Skill #
                }
                else
                    AddLabel( 50, 77 + ySpacer, skillHue, "-Empty-" ); // Dynamic Skill Label

                ySpacer += 20;

            }
            for (int i = 0; i < PlayerMobile.TradeskillsPermitted; i++)
            {

                if (toTrain != SkillName.TasteID && toTrain == from.TradeSkills[i])
                    skillHue = 36;
                else
                    skillHue = 352;

                if (from.SecondarySkills[i] != SkillName.TasteID)
                {
                    AddLabel(50, 77 + ySpacer, skillHue, from.Skills[from.TradeSkills[i]].Name); // Dynamic Skill Label
                    AddLabel(220, 77 + ySpacer, 1153, from.Skills[from.TradeSkills[i]].Base.ToString()); // Current Skill #
                    AddLabel(285, 77 + ySpacer, 1153, from.GetSkillCap(from.TradeSkills[i]).ToString()); // Skill Cap #
                    AddLabel(345, 77 + ySpacer, 1153, String.Format("+{0:0.##}", TrainMaster.GetTrainingIncrease(from, from.TradeSkills[i], spendingExp))); // Increase Skill #
                }
                else
                    AddLabel(50, 77 + ySpacer, skillHue, "-Empty-"); // Dynamic Skill Label

                ySpacer += 20;

            }

            //Add Skill buttons
            if ( spendingExp > 0 ) {

                ySpacer = 20;
                int buttonIndex = 1;
                for ( int i = 0; i < PlayerMobile.SpecialsPermitted; i++ ) {

                    if ( from.SpecialSkills[i] != SkillName.TasteID )
                        AddButton( 25, 80 + ySpacer, 1210, 1209, buttonIndex, GumpButtonType.Reply, 0 ); // Dynamic Skill Button

                    ySpacer += 20;
                    buttonIndex += 1;
                }
                for ( int i = 0; i < PlayerMobile.PrimariesPermitted; i++ ) {

                    if ( from.PrimarySkills[i] != SkillName.TasteID )
                        AddButton( 25, 80 + ySpacer, 1210, 1209, buttonIndex, GumpButtonType.Reply, 0 ); // Dynamic Skill Button

                    ySpacer += 20;
                    buttonIndex += 1;
                }
                for ( int i = 0; i < PlayerMobile.SecondariesPermitted; i++ ) {

                    if ( from.SecondarySkills[i] != SkillName.TasteID )
                        AddButton( 25, 80 + ySpacer, 1210, 1209, buttonIndex, GumpButtonType.Reply, 0 ); // Dynamic Skill Button

                    ySpacer += 20;
                    buttonIndex += 1;
                }
                for (int i = 0; i < PlayerMobile.TradeskillsPermitted; i++)
                {
                    if (from.TradeSkills[i] != SkillName.TasteID)
                        AddButton(25, 80 + ySpacer, 1210, 1209, buttonIndex, GumpButtonType.Reply, 0); // Dynamic Skill Button

                    ySpacer += 20;
                    buttonIndex += 1;
                }
            }
        }



        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if ( !( sender.Mobile is PlayerMobile ) || sender.Mobile == null ) {
                return;
            }

            PlayerMobile from = sender.Mobile as PlayerMobile;

            if ( info == null )
                return;

            if ( info.GetTextEntry( 1 ) != null )
                spendingExp = Utility.ToInt32( info.GetTextEntry( 1 ).Text );

            switch ( info.ButtonID ) {
                case 0: {
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Cancelled" );
                        break;
                    }
                case 1: {
                        m_State[1] = from.SpecialSkills[0];
                        from.SendMessage( MessageUtil.MessageColorPlayer, "You selected {0} to train. Press Okay to confirm.", from.Skills[from.SpecialSkills[0]].Name );
                        from.SendGump( new TrainGump( from, m_State ) );
                        break;
                    }
                case 2: {
                        m_State[1] = from.PrimarySkills[0];
                        from.SendMessage( MessageUtil.MessageColorPlayer, "You selected {0} to train. Press Okay to confirm.", from.Skills[from.PrimarySkills[0]].Name );
                        from.SendGump( new TrainGump( from, m_State ) );
                        break;
                    }
                case 3: {
                        m_State[1] = from.PrimarySkills[1];
                        from.SendMessage( MessageUtil.MessageColorPlayer, "You selected {0} to train. Press Okay to confirm.", from.Skills[from.PrimarySkills[1]].Name );
                        from.SendGump( new TrainGump( from, m_State ) );
                        break;
                    }
                case 4:
                    {
                        m_State[1] = from.PrimarySkills[2];
                        from.SendMessage( MessageUtil.MessageColorPlayer, "You selected {0} to train. Press Okay to confirm.", from.Skills[from.PrimarySkills[2]].Name);
                        from.SendGump(new TrainGump(from, m_State));
                        break;
                    }
                case 5: {
                        m_State[1] = from.SecondarySkills[0];
                        from.SendMessage( MessageUtil.MessageColorPlayer, "You selected {0} to train. Press Okay to confirm.", from.Skills[from.SecondarySkills[0]].Name );
                        from.SendGump( new TrainGump( from, m_State ) );
                        break;
                    }
                case 6: {
                        m_State[1] = from.SecondarySkills[1];
                        from.SendMessage( MessageUtil.MessageColorPlayer, "You selected {0} to train. Press Okay to confirm.", from.Skills[from.SecondarySkills[1]].Name );
                        from.SendGump( new TrainGump( from, m_State ) );
                        break;
                    }
                case 7: {
                        m_State[1] = from.SecondarySkills[2];
                        from.SendMessage( MessageUtil.MessageColorPlayer, "You selected {0} to train. Press Okay to confirm.", from.Skills[from.SecondarySkills[2]].Name );
                        from.SendGump( new TrainGump( from, m_State ) );
                        break;
                    }
                case 8: {
                        m_State[1] = from.SecondarySkills[3];
                        from.SendMessage( MessageUtil.MessageColorPlayer, "You selected {0} to train. Press Okay to confirm.", from.Skills[from.SecondarySkills[3]].Name );
                        from.SendGump( new TrainGump( from, m_State ) );
                        break;
                    }
                case 9:
                    {
                        m_State[1] = from.TradeSkills[0];
                        from.SendMessage( MessageUtil.MessageColorPlayer, "You selected {0} to train. Press Okay to confirm.", from.Skills[from.TradeSkills[0]].Name);
                        from.SendGump(new TrainGump(from, m_State));
                        break;
                    }
                case 20: {// Apply
                        if ( spendingExp > from.CurrentExperience ) {
                            spendingExp = from.CurrentExperience;
                            from.SendMessage( MessageUtil.MessageColorPlayer, "You do not have that much experience. You only have {0} experience to spend.", from.CurrentExperience );
                        }
                        else if ( spendingExp < 1 ) {
                            spendingExp = 0;
                            from.SendMessage( MessageUtil.MessageColorPlayer, "You have to enter an amount of experience to spend on training." );
                        }
                        m_State[0] = spendingExp;
                        from.SendGump( new TrainGump( from, m_State ) );
                        break;
                    }
                case 30: {
                        if ( m_State[0] == null )
                            return;

                        double skillToCap = from.GetSkillCap( (SkillName)m_State[1] ) - from.Skills[(SkillName)m_State[1]].Base;
                        
                        if ( TrainMaster.GetExpCost( from, (SkillName)m_State[1], skillToCap ) < (int)m_State[0] )
                            m_State[0] = TrainMaster.GetExpCost( from, (SkillName)m_State[1], skillToCap );


                        if ( from.Skills[(SkillName)m_State[1]].Base >= from.GetSkillCap( (SkillName)m_State[1] ) ) {
                            from.SendMessage( MessageUtil.MessageColorPlayer, "That skill is already at your skill cap." );
                            break;
                        }

                        if ( TrainMaster.GetTrainingTime( from, (SkillName)m_State[1], TrainMaster.GetTrainingIncrease( from, (SkillName)m_State[1], (int)m_State[0] ) ) == 0 ) {
                            from.SendMessage( MessageUtil.MessageColorPlayer, "You need to spend more experience to get any meaningful training." );
                            break;
                        }

                        if ( from.TrainingPoints.ContainsKey( (SkillName)m_State[1] ) ) {

                            double trainingToCap = from.GetSkillCap( (SkillName)m_State[1] ) - ( from.Skills[(SkillName)m_State[1]].Base + TrainMaster.GetCurrentTraining( from, (SkillName)m_State[1] ) );
                            
                            if ( trainingToCap > 0.0 ) {
                                if ( TrainMaster.GetExpCost( from, (SkillName)m_State[1], trainingToCap ) < (int)m_State[0] )
                                    m_State[0] = TrainMaster.GetExpCost( from, (SkillName)m_State[1], trainingToCap );
                            }
                            else {
                                from.SendMessage( MessageUtil.MessageColorPlayer, "You're already training that skill to your skill cap." );
                                break;
                            }
                            from.TrainingPoints[(SkillName)m_State[1]] += TrainMaster.GetTrainingTime( from, (SkillName)m_State[1], TrainMaster.GetTrainingIncrease( from, (SkillName)m_State[1], (int)m_State[0] ) );
                        }
                        else {
                            from.TrainingPoints.Add( (SkillName)m_State[1], TrainMaster.GetTrainingTime( from, (SkillName)m_State[1], TrainMaster.GetTrainingIncrease( from, (SkillName)m_State[1], (int)m_State[0] ) ) );
                        }

                        from.SendMessage( MessageUtil.MessageColorPlayer, "Training {0} for {1} for a cost of {2} experience points.", (SkillName)m_State[1], TrainMaster.GetTrainingTimeString( from, (SkillName)m_State[1] ), (int)m_State[0] );
                        from.CurrentExperience -= (int)m_State[0];
                        break;
                    }
                default: {
                        //from.SendMessage( "You should not see this message. You should probably turn off Razor macros." );
                        break;
                    }
            }
        }
    }
}
