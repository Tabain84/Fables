using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Commands
{
	public class DotCommand_UseItem
	{
		public static void Initialize( )
		{
			CommandSystem.Register("UseItem", AccessLevel.Player, new CommandEventHandler(OnCommand_UseItem));
		}

		[Usage("UseItem [itemtype]")]
		[Description("Attempts to find an item of the given itemtype in your backpack, and use it.  An itemtype is usually the item's name, but without spaces or the amount")]
		private static void OnCommand_UseItem(CommandEventArgs e)
		{
			Mobile player = e.Mobile;

			if (e.ArgString == string.Empty)
			{
				player.SendMessage(MessageUtil.MessageColorError, "Error:  please supply an itemtype to use");
				return;
			}

			//Try to find the type they want
			Type t = ScriptCompiler.FindTypeByName(e.ArgString);
			if (t == null)
			{
				player.SendMessage(MessageUtil.MessageColorError, "Error:  Invalid item type");
				return;
			}
			else if (!( t.IsSubclassOf(typeof(Item)) ))
			{
				player.SendMessage(MessageUtil.MessageColorError, "Error:  That's not a type of item.");
				return;
			}
			else if (t.IsAbstract)
			{
				player.SendMessage(MessageUtil.MessageColorError, "Error:  That's not a valid type of Item");
				return;
			}

			//Try to find one of these in their backpack
			Item theItem = player.Backpack.FindItemByType(t);
			if (theItem == null)
			{
				player.SendMessage(MessageUtil.MessageColorError, "You don't have any of that item.");
				return;
			}

			player.Use(theItem);
		}
	}
}