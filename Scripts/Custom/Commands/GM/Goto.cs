using Server.Gumps;
using Server.Mobiles;

namespace Server.Commands
{
	public class DotCommand_Goto
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Goto", AccessLevel.GameMaster, new CommandEventHandler( OnCommand_Goto ) );
		}

		[Usage( "Goto" )]
		[Description( "Takes you to the player who's name you enter" )]
		private static void OnCommand_Goto( CommandEventArgs e )
		{
			new GotoInstance( e.Mobile, e.ArgString );
		}

		private class GotoInstance : IPlayerSelect
		{
			PlayerMobile m_Player;
			public GotoInstance( Mobile player, string argString )
			{
				m_Player = (PlayerMobile)player;
				PlayerSelect.SelectOnlinePlayer( m_Player, this, argString );
			}

			public void OnPlayerSelected( PlayerMobile selectedMobile )
			{
				if ( selectedMobile != null ) {
					if ( selectedMobile.Hidden && !m_Player.Hidden ) {
						m_Player.Hidden = true;
					}
					m_Player.MoveToWorld( selectedMobile.Location, selectedMobile.Map );
					m_Player.SendMessage( MessageUtil.MessageColorGM, "You went to " + selectedMobile.Name );
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