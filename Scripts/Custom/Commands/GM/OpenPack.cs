using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
	public class DotCommand_OpenPack
	{
		public static void Initialize()
		{
			CommandSystem.Register( "OpenPack", AccessLevel.GameMaster, new CommandEventHandler( OnCommand_OpenPack ) );
		}

		[Usage( "OpenPack" )]
		[Description( "Opens the pack of the targetted player or NPC" )]
		private static void OnCommand_OpenPack ( CommandEventArgs e )
		{
			new OpenPackInstance( e.Mobile, e.ArgString );
		}

		private class OpenPackInstance : IPlayerSelect
		{
			Mobile m_Player;
			 public OpenPackInstance( Mobile player, string argString )
			 {
			 	//If they didn't add any text, just give them a target picker
			 	if (argString == string.Empty)
			 	{
					player.Target = new InternalOpenPackTarget( player );
			 		return;
			 	}
			 	
			 	m_Player = player;
				int serialInt;
				try
				{
					//Guess if its in hex or not
					if (argString.StartsWith( "0x" ) )
						serialInt = Convert.ToInt32(argString, 16);
					else
						serialInt = Convert.ToInt32(argString);

					if ( serialInt != 0 )
					{
						Serial serial = new Serial(serialInt);
						IEntity theEnt = World.FindEntity( serial );
						if ( theEnt != null )
						{
							if ( theEnt is Mobile )
								OpenTheBackpack( player, (theEnt as Mobile) );
							else
								player.SendMessage( MessageUtil.MessageColorError, "That is a valid serial number, but is an item, not a mobile" );

							return;
						}
					}
				}
				catch
				{
				}

				PlayerSelect.SelectOnlinePlayer( m_Player, this, argString );
			}

			public void OnPlayerSelected( PlayerMobile selectedMobile )
			{
				OpenPackInstance.OpenTheBackpack( m_Player, selectedMobile );
			}

			public void OnPlayerSelectCanceled( )
			{
				return;
			}

			public static void OpenTheBackpack( Mobile player, Mobile targettedMob )
			{
				Container thePack = targettedMob.Backpack;
				if ( thePack == null )
				{
					player.SendMessage( "That creature doesn't have a backpack: Creating one." );
	
					Container newPack = new Backpack();
					newPack.Movable = false;
					targettedMob.AddItem( newPack );
	
					thePack = targettedMob.Backpack;
					if ( thePack == null )
					{
						player.SendMessage( "Unable to add backpack, aborting" );
						return;
					}
				}
				thePack.DisplayTo( player );
			}

			private class InternalOpenPackTarget : Target
			{
				Mobile mPlayer;
				public InternalOpenPackTarget( Mobile player) : base( 15, true, TargetFlags.None )
				{
					player.SendMessage( "Select a mobile who's pack you'd like to see" );
					mPlayer = player;
				}
	
				protected override void OnTarget( Mobile from, object o )
				{
					if ( o is Mobile)
						OpenPackInstance.OpenTheBackpack( from, (o as Mobile) );
					else
						from.SendMessage("Canceled, or invalid location");
				}
			}
		}
	}
}