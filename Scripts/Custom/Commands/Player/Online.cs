using System;
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections;
using System.Collections.Generic;



namespace Server.Commands
{
	public class DotCommand_Online
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Who", AccessLevel.Player, new CommandEventHandler( OnCommand_Online ) );
			CommandSystem.Register( "Players", AccessLevel.Player, new CommandEventHandler( OnCommand_Online ) );

		}

		[Usage( "Who" )]
		[Description( "Lists the players who are online" )]
		private static void OnCommand_Online ( CommandEventArgs e )
		{
			e.Mobile.SendGump( new OnlineGump( e.Mobile, 0 ) );
		}
	}

	public class OnlineGump : Gump
	{
		ArrayList onlinePlayers;
		public const int gumpOffsetX = 50;
		public const int gumpOffsetY = 50;
		
		public const int playersPerPage = 15;

		private int currentPage;

		public OnlineGump( Mobile callingPlayer ) : this (callingPlayer, 0 )
		{
		}
		
		public OnlineGump( Mobile callingPlayer, int requestedPage ) : base ( gumpOffsetX, gumpOffsetY )
		{
		
			currentPage = requestedPage;
			//Close the gump if they already have one open
			callingPlayer.CloseGump( typeof( OnlineGump ) );
			
			//Build the list of online players
			onlinePlayers = BuildOnlineList( callingPlayer );
			
			BuildCurrentGumpPage( );
		}

		//Builds the list of online players.  Hides GMs from players
		public static ArrayList BuildOnlineList( Mobile callingPlayer )
		{
			ArrayList list = new ArrayList();
			List<NetState> states = NetState.Instances;

			for ( int i = 0; i < states.Count; ++i )
			{
				Mobile m = ((NetState)states[i]).Mobile;

				if ( m != null && !m.Deleted && (m == callingPlayer || !m.Hidden || callingPlayer.AccessLevel >= m.AccessLevel) )
					list.Add( m );
			}

			return list;
		}
		
		public void BuildCurrentGumpPage( )
		{
			//Figure out how many players are on this page
			int playersOnPage = onlinePlayers.Count - (currentPage * playersPerPage);
			if ( playersOnPage < 0 )
				playersOnPage = 0;
			else if ( playersOnPage > playersPerPage )
				playersOnPage = playersPerPage;
			
			int totalHeight = 21 * playersOnPage + 22;

			AddPage( 0 );
			AddBackground( 0, 0, 243, totalHeight+20, GumpUtil.Background_PlainGrey );
			AddImageTiled( 10, 10, 223, totalHeight, GumpUtil.Background_PureBlack );
			AddImageTiled( 11, 11, 189, 20, 0xbbc );
			
			AddLabel( 23, 11, 0, String.Format( "Page {0} of {1} ({2})", currentPage+1, (onlinePlayers.Count + playersPerPage - 1) / playersPerPage, onlinePlayers.Count ) );

			AddImageTiled( 191, 11, 20, 20, 0x0E14 );
			if (currentPage > 0)
			{
				AddButton( 193, 13, GumpUtil.GoldArrowUp1, GumpUtil.GoldArrowUp2, GumpUtil.BUTTONID_LAST_PAGE, GumpButtonType.Reply, 0 );
			}

			AddImageTiled( 212, 11, 20, 20, 0x0E14 );
			if ( (currentPage + 1) * playersOnPage < onlinePlayers.Count )
			{
				AddButton( 214, 13, GumpUtil.GoldArrowDown1, GumpUtil.GoldArrowDown2, GumpUtil.BUTTONID_NEXT_PAGE, GumpButtonType.Reply, 0 );
			}
			
			int y = 11;
			//Go through each player in the list and display their name
			for ( int i = 0; i < playersOnPage; ++i)
			{
				y += 21;
				Mobile m = (Mobile)onlinePlayers[currentPage * playersPerPage + i];
				String theString = (currentPage * playersPerPage + i + 1) + " " + m.Name;

				AddImageTiled( 11, y, 221, 20, 0x0BBC );
				AddLabelCropped( 13, y, 210, 20, GetHueFor( m ), theString );
			}
		}

		private static int GetHueFor( Mobile m )
		{
			switch ( m.AccessLevel )
			{
				case AccessLevel.Administrator: return 0x516;
				case AccessLevel.Seer: return 0x144;
				case AccessLevel.GameMaster: return 0x21;
				case AccessLevel.Counselor: return 0x2;
				case AccessLevel.Player: default:
				{
					if ( m.Kills >= 5 )
						return 0x21;
					else if ( m.Criminal )
						return 0x3B1;

					return 0x58;
				}
			}
		}

		//Handles button presses
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile player = state.Mobile;
			switch ( info.ButtonID )
			{
				case 0: // Closed
				{
					player.SendMessage( MessageUtil.MessageColorPlayer, "Finished" );
					return;
				}
				case GumpUtil.BUTTONID_NEXT_PAGE: // Next
				{
					if ( (currentPage + 1) * playersPerPage < onlinePlayers.Count )
						player.SendGump( new OnlineGump( player, currentPage + 1 ) );
					else
						player.SendMessage( MessageUtil.MessageColorError, "Error:  invalid gump return" );

					break;
				}
				case GumpUtil.BUTTONID_LAST_PAGE: // Previous
				{
					if ( currentPage > 0 )
						player.SendGump( new OnlineGump( player, currentPage - 1 ) );
					else
						player.SendMessage( MessageUtil.MessageColorError, "Error:  invalid gump return" );

					break;
				}
				default:
				{
					player.SendMessage( MessageUtil.MessageColorError, "Error:  invalid gump return" );
					break;
				}
			}
		}
	}
}