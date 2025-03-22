using System;
using System.IO;
using System.Xml;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;
using Server.Prompts;

namespace Server.Commands
{
	public class DotCommand_ItemWipe
	{
		public static void Initialize()
		{
			CommandSystem.Register( "ItemWipe", AccessLevel.GameMaster, new CommandEventHandler( OnCommand_ItemWipe ) );
		}

		[Usage( "ItemWipe [range]" )]
		[Description( "Generates all items within the specified range (default range of 3)" )]
		private static void OnCommand_ItemWipe ( CommandEventArgs e )
		{
			int range = 3;
			string[] cmdArgs = e.Arguments;
			if (cmdArgs.Length > 0)
			{
				try
				{
					range = Convert.ToInt32(cmdArgs[0]);
					if (range < 1)
					{
						e.Mobile.SendMessage( MessageUtil.MessageColorGM, "Invalid range - range is now 1" );
						range = 1;
					}
					else if (range > 10)
					{
						e.Mobile.SendMessage( MessageUtil.MessageColorGM, "Range too large - range is now 10" );
						range = 10;
					}
				}
				catch
				{
					e.Mobile.SendMessage( "Caught an error in command arguments: range invalid" );
					return;
				}
			}

			ArrayList existingItems = new ArrayList();
			foreach (Item item in e.Mobile.Map.GetItemsInRange( e.Mobile.Location, range ) )
			{
				existingItems.Add( item );
			}

			if (existingItems.Count < 1)
			{
				e.Mobile.SendMessage( MessageUtil.MessageColorGM, "Canceled - no items within specified range" );
				return;
			}

			e.Mobile.SendMessage( MessageUtil.MessageColorGM, "This will delete " + existingItems.Count + " items within a range of " + range );
			e.Mobile.SendMessage( MessageUtil.MessageColorGM, "You must type 'YES' to confirm (or enter to cancel):" );
			e.Mobile.Prompt = new ConfirmPrompt( existingItems );
		}


		private class ConfirmPrompt : Prompt
		{
			ArrayList m_existingItems;
			public ConfirmPrompt( ArrayList existingItems )
			{
				m_existingItems = existingItems;
			}

			public override void OnResponse( Mobile from, string text )
			{
				if ( text.Equals( "YES" ) )
				{
					foreach (Item item in m_existingItems)
						item.Delete();
				}
				else
				{
					from.SendMessage( "ItemWipe canceled.  Whew!" );
				}
			}

			public override void OnCancel( Mobile from )
			{
				from.SendMessage( "ItemWipe canceled.  Whew!" );
			}
		}
	}
}

