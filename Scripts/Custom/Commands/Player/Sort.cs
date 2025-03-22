// Script: Sort.cs
// Version: 1.1
// Author: Oak (ssalter)
// Servers: RunUO 2.0
// Date: 7/7/2006
// Purpose: 
// Player Command. [sort allows a player to sort items from one container to another
//   Type [sort followed by a keyword, target FROM container and then target TO container
//   Keywords are: gems, wands, regs, scrolls, armor, weapons, clothing, potions, hides, jewelry, help (to get list of keywords)
// History: 
//  Written for RunUO 1.0 shard in March 2005.
//  Modified for RunUO 2.0. Commented out XmlSpawner Questholder check.
// If you have XmlSpawner2, find the if(xx is Questholder) blocks that are commented out and uncomment them or players will use this command to store items in questholder (if you use questholders) and thus have a blessed container.
using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Accounting;
using System.Collections;
using Server.Network;
//using Server.Items.Plant;


namespace Server.Commands
{
    public class Sort
    {
        private static Type[] m_SortType = new Type[]
	{
		typeof( BaseArmor ),
		typeof( BaseWeapon ),
		typeof( BaseClothing ),
		typeof( SpellScroll ),
		typeof( BaseReagent ),
		typeof( BaseHides ),
		typeof( BasePotion ),
		typeof( BaseJewel ),
		typeof( Diamond ),
		typeof( BaseWand),
        //typeof(GarlicUprooted)
	};

        public static void Initialize()
        {
            CommandSystem.Register( "Sort", AccessLevel.Player, new CommandEventHandler( Sort_OnCommand ) );
        }
        public static void Sort_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            // if we have a command after just the word "sort", as we should
            if ( e.Length != 0 ) {
                switch ( e.GetString( 0 ).ToLower() ) {
                    case "wands":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[9] );
                        break;
                    case "gems":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[8] );
                        break;
                    case "regs":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[4] );
                        break;
                    case "scrolls":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[3] );
                        break;
                    case "armor":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[0] );
                        break;
                    case "clothing":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[2] );
                        break;
                    case "weapons":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[1] );
                        break;
                    case "hides":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[5] );
                        break;
                    case "potions":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[6] );
                        break;
                    case "jewelry":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[7] );
                        break;
                    case "wildregs":
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.Target = new PackFromTarget( from, m_SortType[10] );
                        break;
                    case "help":
                        //from.SendMessage( Util.MessageColorPlayer, "Select the container you want to sort FROM." );
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Usage: .sort and one of the following words: gems, wands, regs, scrolls, armor, weapons, clothing, potions, hides, jewelry, wildregs" );
                        break;
                    default:
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Usage: .sort and one of the following words: gems, wands, regs, scrolls, armor, weapons, clothing, potions, hides, jewelry, wildregs" );
                        break;
                }
            }
            else {
                from.SendMessage( MessageUtil.MessageColorPlayer, "Usage: .sort and one of the following words: gems, wands, regs, scrolls, armor, weapons, clothing, potions, hides, jewelry, wildregs" );
            }
        }
        private class PackFromTarget : Target
        {
            private Type SortType;
            public PackFromTarget( Mobile from, Type type )
                : base( -1, true, TargetFlags.None )
            {
                SortType = type;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if ( o is Container ) {
                    Container xx = o as Container;
                    if ( xx is QuestHolder ) {
                        from.SendMessage( MessageUtil.MessageColorError, "You can not sort from a questbook." );
                        return;
                    }

                    // Container that is either in a pack
                    if ( xx.IsChildOf( from.Backpack ) || xx == from.Backpack ) {
                        from.SendMessage( MessageUtil.MessageColorPlayer, "Select the container you want to sort into" );
                        from.Target = new PackToTarget( from, xx, SortType );
                    }
                    else {
                        from.SendMessage( MessageUtil.MessageColorError, "The container to sort from must be in your main backpack or be your main backpack." );
                    }
                }
                else
                    from.SendMessage( MessageUtil.MessageColorError, "That is not a container." );
            }
        }
        private class PackToTarget : Target
        {
            private Container FromCont;
            private Type SortType;

            public PackToTarget( Mobile from, Container cont, Type type )
                : base( -1, true, TargetFlags.None )
            {
                SortType = type;
                FromCont = cont;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if ( o is Container ) {
                    Container xx = o as Container;

                    if ( xx is QuestHolder ) {
                        from.SendMessage( MessageUtil.MessageColorError, "You can not sort into a questbook." );
                        return;
                    }

                    //make sure they aren't targeting same container
                    //also they aren't targeting a parent container
                    if ( xx == FromCont || FromCont.IsChildOf( xx ) ) {
                        from.SendMessage( MessageUtil.MessageColorError, "Isn't that too obvious?" );
                        return;
                    }

                    bool isChildCon = xx.IsChildOf( FromCont );

                    // gems are a special case 
                    if ( SortType == typeof( Diamond ) ) {
                        Item[] itema =  FromCont.FindItemsByType( typeof( Amber ), true );
                        Item[] itemb =  FromCont.FindItemsByType( typeof( Amethyst ), true );
                        Item[] itemc =  FromCont.FindItemsByType( typeof( Citrine ), true );
                        Item[] itemd =  FromCont.FindItemsByType( typeof( Diamond ), true );
                        Item[] iteme =  FromCont.FindItemsByType( typeof( Emerald ), true );
                        Item[] itemf =  FromCont.FindItemsByType( typeof( Ruby ), true );
                        Item[] itemg =  FromCont.FindItemsByType( typeof( Sapphire ), true );
                        Item[] itemh =  FromCont.FindItemsByType( typeof( StarSapphire ), true );
                        Item[] itemi =  FromCont.FindItemsByType( typeof( Tourmaline ), true );
                        /*Item[] itemj =  FromCont.FindItemsByType( typeof( CutAmber ), true );
                        Item[] itemk =  FromCont.FindItemsByType( typeof( CutAmethyst ), true );
                        Item[] iteml =  FromCont.FindItemsByType( typeof( CutCitrine ), true );
                        Item[] itemm =  FromCont.FindItemsByType( typeof( CutDiamond ), true );
                        Item[] itemn =  FromCont.FindItemsByType( typeof( CutEmerald ), true );
                        Item[] itemo =  FromCont.FindItemsByType( typeof( CutRuby ), true );
                        Item[] itemp =  FromCont.FindItemsByType( typeof( CutSapphire ), true );
                        Item[] itemq =  FromCont.FindItemsByType( typeof( CutStarSapphire ), true );
                        Item[] itemr =  FromCont.FindItemsByType( typeof( CutTourmaline ), true );*/
                        foreach ( Item item in itema ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itema ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemb ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemc ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemd ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in iteme ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemf ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemg ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemh ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemi ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                        /*foreach ( Item item in itemj ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemk ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in iteml ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemm ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemn ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemo ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemp ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemq ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemr ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }*/
                    }
                    /*else if ( SortType == typeof( GarlicUprooted ) ) {
                        Item[] itema =  FromCont.FindItemsByType( typeof( GarlicUprooted ), true );
                        Item[] itemb =  FromCont.FindItemsByType( typeof( MandrakeUprooted ), true );
                        Item[] itemc =  FromCont.FindItemsByType( typeof( WhiteSageUprooted ), true );
                        Item[] itemd =  FromCont.FindItemsByType( typeof( NightshadeUprooted ), true );
                        Item[] iteme =  FromCont.FindItemsByType( typeof( GinsengUprooted ), true );

                        foreach ( Item item in itema ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemb ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemc ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in itemd ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                        foreach ( Item item in iteme ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorPlayer, "That container is too full or not accessible." );
                        }
                    }*/
                    // all other supported sort items have a base type
                    else {
                        Item[] items =  FromCont.FindItemsByType( SortType, true );
                        foreach ( Item item in items ) {
                            // pass the child container and anything within it
                            // also pass anything that is the parent container of the destination container
                            if ( isChildCon && ( item == xx || item.IsChildOf( xx ) || ( ( item is Container ) && xx.IsChildOf( item ) ) ) )
                                continue;

                            //					Console.WriteLine ("Sorttype=" + SortType + " and item is " + item + "");
                            if ( item.Movable && !( xx.TryDropItem( from, item, false ) ) )
                                from.SendMessage( MessageUtil.MessageColorError, "That container is too full or not accessible." );
                        }
                    }
                }
                else {
                    from.SendMessage( MessageUtil.MessageColorError, "That is not a container." );
                }
            }
        }
    }
}