// Script: Dump.cs
// Version: 1.1
// Author: Oak (ssalter)
// Servers: RunUO 2.0
// Date: 7/7/2006
// Purpose: 
// Player Command. [dump allows a player to dump everything from one container to another.
//   Allowed containers to dump FROM are main backpack or a subcontainer thereof.
// History: 
//  Written for RunUO 1.0 shard, Sylvan Dreams,  in February 2005.
//  Modified for RunUO 2.0, removed shard specific customizations (wing layers, etc.), commented out xmlspawner questholder check.
// If you have XmlSpawner2, find the if(xx is Questholder) blocks that are commented out and uncomment them or players will use this command to store items in questholder (if you use questholders) and thus have a blessed container.

using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Accounting; 
using System.Collections; 
using Server.Network;


namespace Server.Commands 
{ 
  public class Dump
  { 
    public static void Initialize() 
    { 
      CommandSystem.Register( "Dump", AccessLevel.Player, new CommandEventHandler( Dump_OnCommand ) ); 
    } 
    public static void Dump_OnCommand( CommandEventArgs e ) 
    { 
		Mobile from = e.Mobile; 
		from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to dump items from."); 
		from.Target = new PackFromTarget( from);
	}
	private class PackFromTarget : Target
	{
		public PackFromTarget( Mobile from ) : base( -1, true, TargetFlags.None )
		{
		}
			
		protected override void OnTarget( Mobile from, object o )
		{
			if(o is Container)
			{
				Container xx = o as Container;

				if (xx is QuestHolder)
				{
					from.SendMessage( MessageUtil.MessageColorPlayer, "You can not dump from a questbook."); 
					return;
				}

				// Container that is either in the player's backpack or a child thereof
				if ( xx.IsChildOf( from.Backpack) || xx == from.Backpack)
				{
					from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to dump items into."); 
					from.Target = new PackToTarget( from, xx );
				}
				else
				{
					from.SendMessage( MessageUtil.MessageColorPlayer, "The container to dump from must be in your main backpack or be your main backpack."); 
				}
			}
			else
			{
				from.SendMessage( MessageUtil.MessageColorPlayer, "That is not a container!"); 
			}
		}
	}
	private class PackToTarget : Target
	{
		private Container FromCont;

			public PackToTarget( Mobile from, Container cont ) : base( -1, true, TargetFlags.None )
		{
			FromCont = cont;
		}
		
		protected override void OnTarget( Mobile from, object o )
		{
			if( o is Container)
			{
				Container xx = o as Container;

				if (xx is QuestHolder)
				{
                    from.SendMessage( MessageUtil.MessageColorPlayer, "You can not dump into a questbook."); 
					return;
				}
				//make sure they aren't targeting same container
                //also they aren't targeting a parent container
				if (xx == FromCont)// || FromCont.IsChildOf(xx))
				{
					from.SendMessage( MessageUtil.MessageColorPlayer, "Isn't that too obvious?");
					return;
				}

                bool isChildCon = xx.IsChildOf(FromCont);

				Item[] items =  FromCont.FindItemsByType( typeof(Item), true );
				foreach (Item item in items) 
				{
                    // pass the child container and anything within it
                    // also pass anything that is the parent container of the destination container
                    if (isChildCon && (item == xx || item.IsChildOf(xx) || ((item is Container) && xx.IsChildOf(item)))) 
                        continue;
                    if ( item.Parent is BaseBoard )
                        continue;
					if(item.Movable && !(xx.TryDropItem( from, item, true )))
						from.SendMessage( MessageUtil.MessageColorPlayer, "That container is either too full or not accessible.");
				}
			}
			else
			{
				from.SendMessage( MessageUtil.MessageColorPlayer, "That is not a container."); 
			}
		}
	}	
  } 
} 