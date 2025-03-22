using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Network;
using System;

namespace Server.Commands
{
    public class lookCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register( "Look", AccessLevel.Player, new CommandEventHandler( look_OnCommand ) );
        }

        [Usage( "Look" )]
        [Description( "Used to look at someone who is near you or to change your character's description." )]
        public static void look_OnCommand( CommandEventArgs e )
        {
            if ( e.Mobile is PlayerMobile ) {
                e.Mobile.SendMessage( MessageUtil.MessageColorPlayer,"Who would you like to look at?" );
                e.Mobile.Target = new lookTarget();
            }
        }
    }

    public class lookTarget : Target
    {
        public lookTarget()
            : base( -1, false, TargetFlags.None )
        {
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            if ( from is PlayerMobile && targeted is PlayerMobile ) {
                if ( from.Equals( targeted ) ) {
                    ( (Mobile)targeted ).DisplayPaperdollTo( from );
                    from.Send( new DisplayProfile( !from.ProfileLocked, from, "Description of " + from.RawName, from.Profile, "Use the space above to describe your character." ) );
                }
                else {
                    //( (Mobile)targeted ).SendMessage( "You notice that {0} is looking at you.", from.Name );
                    //( (Mobile)targeted ).DisplayPaperdollTo( from );
                    from.CloseGump( typeof( lookGump ) );
                    from.SendGump( new lookGump( from, (Mobile)targeted ) );
                }
            }
            else if ( from is PlayerMobile && targeted is BaseItem ) {
                BaseItem item = targeted as BaseItem;
                if ( !item.Stackable )
                    if ( item.LookText != null )
                        from.SendGump( new lookItemGump( from, item ) );
                    else if ( from.AccessLevel == AccessLevel.Owner )
                        from.SendGump( new lookEditGump( from, item ) );

            }
            else
                from.SendMessage( MessageUtil.MessageColorPlayer, "There's nothing special about it, it isn't worth looking..." );
        }
    }

    public class lookGump : Gump
    {
        private const int Width = 300;
        private const int Height = 200;

        public lookGump( Mobile m, Mobile target )
            : base( 100, 100 )
        {
            AddPage( 0 );

            AddBackground( 0, 0, Width, Height, 0xDAC );

            AddPage( 1 );

            AddHtml( 0, 10, Width, 25, "<CENTER>" + "Looking at " + target.Name, false, false );
            AddHtml( 20, 30, Width - 40, Height - 50, target.Profile, true, true );
        }
    }

    public class lookItemGump : Gump
    {
        private const int Width = 300;
        private const int Height = 200;

        public lookItemGump( Mobile m, BaseItem target )
            : base( 100, 100 )
        {
            AddPage( 0 );

            AddBackground( 0, 0, Width, Height, 0xDAC );

            AddPage( 1 );

            AddHtml( 0, 10, Width, 25, String.Format( "<CENTER>Looking at: {0}", target.Name != null ? target.Name : target.ItemData.Name ), false, false );
            AddHtml( 20, 30, Width - 40, Height - 50, target.LookText, true, true );
        }
    }

    public class lookEditGump : Gump
    {
        BaseItem target;

        private const int Width = 300;
        private const int Height = 450;

        public lookEditGump( Mobile m, BaseItem m_target )
            : base( 100, 100 )
        {
            target = m_target;

            AddPage( 0 );

            AddBackground( 0, 0, Width, Height, 0xDAC );

            AddPage( 1 );

            AddHtml( 0, 10, Width, 25, String.Format( "<CENTER>Describing: {0}", target.Name != null ? target.Name : target.ItemData.Name ), false, false );
            AddTextEntry( 20, 30, Width - 40, Height - 50, 32, 0, @"*", 1500 );
            AddTextEntry( 20, 155, Width - 40, Height - 50, 32, 1, @"*", 1500 );
            AddTextEntry( 20, 255, Width - 40, Height - 50, 32, 2, @"*", 1500 );

            AddButton( 200, 400, 247, 248, 30, GumpButtonType.Reply, 0 ); // Okay

            //AddHtml( 20, 30, Width - 40, Height - 50, target.LookText, true, true );
        }

        public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
            switch ( info.ButtonID ) {
                case 0: // Cancel

                    from.SendMessage( MessageUtil.MessageColorPlayer, "Cancelled." );

                    break;
                default:
                    if ( info.TextEntries[0].Text.Length < 10 )
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Your text is too short." );
                    else if ( info.TextEntries[1].Text.Length == 1 )
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Please remove the * before saving." );
                    else if ( info.TextEntries[2].Text.Length == 1 )
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Please remove the * before saving." );
                    else {
                        string text =info.TextEntries[0].Text + info.TextEntries[1].Text + info.TextEntries[2].Text;
                        target.LookText = text;
                        from.SendMessage( MessageUtil.MessageColorPlayer, "The item's .look has been set." );
                    }
                    break;
            }
        }
    }
}
