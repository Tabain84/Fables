using System;
using Server;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;


namespace Server.Commands
{
	public class DotCommand_Summon
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Summon", AccessLevel.GameMaster, new CommandEventHandler( OnCommand_Summon ) );
		}

		[Usage( "Summon" )]
		[Description( "Summons the player who's name you enter" )]
		private static void OnCommand_Summon( CommandEventArgs e )
		{
			new SummonInstance( e.Mobile, e.ArgString );
		}
			/*{
			string theName = e.ArgString.ToLower();
			if (theName == "")
			{
				e.Mobile.SendMessage( "Please include the name of the player you want to summon" );
				return;
			}
			
			List<NetState> states = NetState.Instances;
			for ( int i = 0; i < states.Count; ++i )
			{
				Mobile m = ((NetState)states[i]).Mobile;

				if ( m != null && !m.Deleted)
				{
					if ( m.Name.ToLower().IndexOf(theName) >= 0 )
					{
						m.MoveToWorld( e.Mobile.Location, e.Mobile.Map );
						e.Mobile.SendMessage( "Summoned " + m.Name );
					}
				}
			}
		}*/

		private class SummonInstance : IPlayerSelect
		{
			PlayerMobile m_Player;
			public SummonInstance( Mobile player, string argString )
			{
				m_Player = (PlayerMobile)player;
				PlayerSelect.SelectOnlinePlayer( m_Player, this, argString );
			}

			public void OnPlayerSelected( PlayerMobile selectedMobile )
			{
				if ( selectedMobile != null ) {
					selectedMobile.MoveToWorld(m_Player.Location, m_Player.Map );
					m_Player.SendMessage( MessageUtil.MessageColorGM, "You summoned " + selectedMobile.Name );
					selectedMobile.SendMessage( MessageUtil.MessageColorPlayer, "You were summoned by " + m_Player.Name );
				}
				else
					m_Player.SendMessage( MessageUtil.MessageColorGM, "Unable to find that player." );
			}

			public void OnPlayerSelectCanceled()
			{
				m_Player.SendMessage( MessageUtil.MessageColorGM, "Unable to find that player." );
			}
		}
	}
}