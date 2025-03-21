using System;
using System.Collections.Generic;
using System.Text;

using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Exp
{
	class ExpMaster
	{
		public const int DailyMaxExp = 50000;
        public const int TotalMax = 250000;

		public static void Initialize()
		{
			CommandSystem.Register( "GiveExp", AccessLevel.GameMaster, new CommandEventHandler( OnCommand_GiveExp ) );
			CommandSystem.Register( "Exp", AccessLevel.Player, new CommandEventHandler( OnCommand_Exp ) );
            CommandSystem.Register( "ShowExp", AccessLevel.GameMaster, new CommandEventHandler( OnCommand_ShowExp ) );
		}

		[Usage( ".exp" )]
		[Description( "Displays your total experience earned, and how much you currently have to use." )]
		private static void OnCommand_Exp( CommandEventArgs e )
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;

			pm.SendMessage( MessageUtil.MessageColorPlayer, "Total Experience Earned: {0}", pm.TotalExperience );
			pm.SendMessage( MessageUtil.MessageColorPlayer, "Current Experience Available: {0}", pm.CurrentExperience );
            pm.SendMessage( MessageUtil.MessageColorPlayer, "Daily Experience: {0}", pm.DailyExperience );
        }

		[Usage( ".giveexp #ExpToGive" )]
		[Description( "Allows staff to give a character additional experience points as a reward." )]
		private static void OnCommand_GiveExp( CommandEventArgs e )
		{
			int amount = 0;

			if ( e.Length >= 1 )
				amount = e.GetInt32( 0 );

			if ( amount > 0 ) {
				e.Mobile.Target = new GiveExpTarget( amount );
                e.Mobile.SendMessage( MessageUtil.MessageColorGM, "Target a player to give experience to." );
			}
			else
				e.Mobile.SendMessage( MessageUtil.MessageColorGM, "Usage: .GiveExp #ExpToGive" );
		}

		private class GiveExpTarget : Target
		{
			private int m_amount;

			public GiveExpTarget( int amount )
				: base( 20, false, TargetFlags.None )
			{
				m_amount = amount;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o == null )
					return;

				if ( o is PlayerMobile ) {

					PlayerMobile mobile = (PlayerMobile)o;

					ExpGain.AwardExperience( mobile, m_amount, false );
					from.SendMessage( MessageUtil.MessageColorGM, "You gave {0} {1} experience points.", mobile.Name, m_amount );
					CommandLogging.WriteLine( from, "{0} gave {1} {2} experience points.", from.Name, mobile.Name, m_amount );

				}
				else
					from.SendMessage( MessageUtil.MessageColorGM, "You can only give experience to players." );
			}
		}

        [Usage( ".showexp" )]
        [Description( "Allows staff to see a characters experience points." )]
        private static void OnCommand_ShowExp( CommandEventArgs e )
        {

                e.Mobile.Target = new ShowExpTarget();
                e.Mobile.SendMessage( MessageUtil.MessageColorGM, "Target a player to show their experience points." );
        }

        private class ShowExpTarget: Target
        {

            public ShowExpTarget( )
                : base( 20, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if ( o == null )
                    return;

                if ( o is PlayerMobile ) {

                    PlayerMobile pm = (PlayerMobile)o;

                    from.SendMessage( MessageUtil.MessageColorGM, "Total Experience Earned: {0}", pm.TotalExperience );
                    from.SendMessage( MessageUtil.MessageColorGM, "Current Experience Available: {0}", pm.CurrentExperience );
                }
                else
                    from.SendMessage( MessageUtil.MessageColorGM, "Only player characters would have experience." );
            }
        }
	}
}
