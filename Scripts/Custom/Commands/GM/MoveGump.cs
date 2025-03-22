using System.Collections;
using System.Collections.Generic;

using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Items;

using Server.Engines.XmlSpawner2;

namespace Server.Commands
{
    public class MoveGump: Gump
    {
        #region Fields

        private static Hashtable Userdata;
        private static Hashtable Usercopy;
        private object[] m_State;

        private bool zcheck;
        private int offset;
        private int minZ;
        private int maxZ;
        private bool IDcheck;
        private int minID;
        private int maxID;

        #endregion

        #region Static Methods
        public static void Initialize()
        {
            Userdata = new Hashtable();
            Usercopy = new Hashtable();

            CommandSystem.Register( "Massmove", AccessLevel.Administrator, new CommandEventHandler( Massmove_OnCommand ) );
            EventSink.Disconnected += new DisconnectedEventHandler( EventSink_Disconnect );
        }

        public static void EventSink_Disconnect( DisconnectedEventArgs e )
        {
            if ( Userdata[e.Mobile] != null ) {

                ArrayList worlditems = Userdata[e.Mobile] as ArrayList;

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, true );

                Userdata[e.Mobile] = null;
            }

        }

        [Usage( "Massmove" )]
        [Description( "Opens a gump for manipulating areas of items." )]
        private static void Massmove_OnCommand( CommandEventArgs e )
        {
            if ( e.Mobile.HasGump( typeof( MoveGump ) ) )
                return;

            object[] state = new object[7];

            state[0] = false;
            state[1] = 1;
            state[2] = 0;
            state[3] = 127;
            state[4] = false;
            state[5] = 2;
            state[6] = 16300;

            e.Mobile.SendGump( new MoveGump( e.Mobile, state ) );
        }

        #endregion

        #region Sub routines

        public static void InvokeMassMove( Mobile from, ArrayList items )
        {
            if ( items == null ) {
                from.SendMessage( MessageUtil.MessageColorGM, "No items present." );
                return;
            }

            if ( Userdata[from] != null ) {
                from.SendMessage( MessageUtil.MessageColorGM, "You cannot do that with items selected for massmove." );
                return;
            }

            if ( from.HasGump( typeof( MoveGump ) ) )
                from.CloseGump( typeof( MoveGump ) );

            Userdata[from] = items;

            foreach ( Item item in items )
                ToggleHighlight( item, false );

            object[] state = new object[7];

            state[0] = false;
            state[1] = 1;
            state[2] = 0;
            state[3] = 127;
            state[4] = false;
            state[5] = 2;
            state[6] = 16300;

            from.SendGump( new MoveGump( from, state ) );
        }

        private static void Offset( Mobile from, int xOffset, int yOffset, int zOffset )
        {
            ArrayList items = Userdata[from] as ArrayList;

            if ( items != null ) {
                foreach ( Item item in items ) {
                    if ( !item.Deleted ) {
                        item.X += xOffset;
                        item.Y += yOffset;
                        item.Z += zOffset;
                    }
                }
            }
        }

        public static void ToggleHighlight( Item item, bool remove )
        {
            XmlHue hue = (XmlHue)XmlAttach.FindAttachment( item, typeof( XmlHue ) );

            if ( item != null ) {
                if ( hue == null && !remove )
                    XmlAttach.AttachTo( item, new XmlHue( 43, 30.0 ) );
                else if ( remove && hue != null )
                    hue.Delete();
            }
        }

        public static void DeleteSelection( Mobile from, bool yes, object[] state )
        {
            if ( yes ) {
                ArrayList worlditems = Userdata[from] as ArrayList;
                from.SendMessage( 43, "" + worlditems.Count + " items deleted!" );

                foreach ( Item item in worlditems )
                    item.Delete();

                Userdata[from] = null;

                from.SendGump( new MoveGump( from, state ) );
            }
            else
                from.SendGump( new MoveGump( from, state ) );
        }

        #endregion

        #region Area Target

        private void SelectOnTarget( Mobile from, Map map, Point3D start, Point3D end, object state )
        {
            object[] states = (object[])state;

            bool add = (bool)states[0];
            bool remove = (bool)states[1];
            bool countZ = (bool)states[2];
            bool countID = (bool)states[3];



            if ( add && Userdata[from] != null ) {
                Rectangle2D rect = new Rectangle2D( start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1 );
                IPooledEnumerable eable = map.GetItemsInBounds( rect );

                ArrayList worlditems = Userdata[from] as ArrayList;

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, false );

                foreach ( Item item in eable ) {
                    if ( worlditems.Contains( item ) == false ) {
                        if ( countZ && countID ) {
                            if ( item.ItemID >= minID && item.ItemID <= maxID && item.Z >= minZ && item.Z <= maxZ ) {
                                worlditems.Add( item );
                                ToggleHighlight( item, false );
                            }
                        }
                        else if ( countZ && item.Z >= minZ && item.Z <= maxZ ) {
                            worlditems.Add( item );
                            ToggleHighlight( item, false );
                        }
                        else if ( countID && item.ItemID >= minID && item.ItemID <= maxID ) {
                            worlditems.Add( item );
                            ToggleHighlight( item, false );
                        }
                        else if ( !countID && !countZ ) {
                            worlditems.Add( item );
                            ToggleHighlight( item, false );
                        }
                    }
                }

                eable.Free();


                Userdata[from] = worlditems;

                from.SendMessage( 43, "You have selected {0} items.", worlditems.Count );

                return;
            }
            else if ( remove ) {
                Rectangle2D rect = new Rectangle2D( start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1 );
                IPooledEnumerable eable = map.GetItemsInBounds( rect );

                ArrayList worlditems = Userdata[from] as ArrayList;

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, false );

                foreach ( Item item in eable ) {
                    if ( worlditems.Contains( item ) == true ) {
                        if ( countZ && countID ) {
                            if ( item.ItemID >= minID && item.ItemID <= maxID && item.Z >= minZ && item.Z <= maxZ ) {
                                worlditems.Remove( item );
                                ToggleHighlight( item, true );
                            }
                        }
                        else if ( countZ && item.Z >= minZ && item.Z <= maxZ ) {
                            worlditems.Remove( item );
                            ToggleHighlight( item, true );
                        }
                        else if ( countID && item.ItemID >= minID && item.ItemID <= maxID ) {
                            worlditems.Remove( item );
                            ToggleHighlight( item, true );
                        }
                        else if ( !countID && !countZ ) {
                            worlditems.Remove( item );
                            ToggleHighlight( item, true );
                        }
                    }
                }

                eable.Free();

                Userdata[from] = worlditems;

                from.SendMessage( 43, "You have selected {0} items.", worlditems.Count );

                return;
            }
            else { //Set

                ArrayList worlditems;

                if ( Userdata[from] != null ) {
                    worlditems = Userdata[from] as ArrayList;

                    foreach ( Item item in worlditems )
                        ToggleHighlight( item, true );
                }

                worlditems = new ArrayList();

                Rectangle2D rect = new Rectangle2D( start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1 );
                IPooledEnumerable eable = map.GetItemsInBounds( rect );

                foreach ( Item item in eable ) {
                    if ( countZ && countID ) {
                        if ( item.ItemID >= minID && item.ItemID <= maxID && item.Z >= minZ && item.Z <= maxZ ) {
                            worlditems.Add( item );
                            ToggleHighlight( item, false );
                        }
                    }
                    else if ( countZ && item.Z >= minZ && item.Z <= maxZ ) {
                        worlditems.Add( item );
                        ToggleHighlight( item, false );
                    }
                    else if ( countID && item.ItemID >= minID && item.ItemID <= maxID ) {
                        worlditems.Add( item );
                        ToggleHighlight( item, false );
                    }
                    else if ( !countID && !countZ ) {
                        worlditems.Add( item );
                        ToggleHighlight( item, false );
                    }
                }
                
                eable.Free();

                Userdata[from] = worlditems;
                from.SendMessage( 43, "You have selected {0} items.", worlditems.Count );
                return;
            }
        }

        #endregion

        #region Copy Target
        private class CopyTarget: Target
        {
            object[] m_State;

            public CopyTarget( object[] state )
                : base( 20, true, TargetFlags.None )
            {
                m_State = state;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                IPoint3D p = o as IPoint3D;

                if ( p == null ) {
                    from.SendGump( new MoveGump( from, m_State ) );
                    from.SendMessage( 43, "Something went wrong, try again." );
                    return;
                }

                if ( p is Item )
                    p = ( (Item)p ).GetWorldTop();
                else if ( p is Mobile )
                    p = ( (Mobile)p ).Location;

                Point3D point = new Point3D( p );

                int startX = 7144;
                int startY = 4096;
                int startZ = 127;

                ArrayList worlditems = Userdata[from] as ArrayList;

                foreach ( Item item in worlditems ) {

                    if ( item.X < startX )
                        startX = item.X;

                    if ( item.Y < startY )
                        startY = item.Y;

                    if ( item.Z < startZ )
                        startZ = item.Z;
                }

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, true );

                ArrayList worldcopy = new ArrayList();

                foreach ( Item item in worlditems ) {
                    Static bItem = new Static( item.ItemID );

                    bItem.Hue = item.Hue;
                    bItem.Name = item.Name;

                    if ( item.Amount > 1 ) {
                        bItem.Stackable = true;
                        bItem.Amount = item.Amount;
                    }

                    bItem.MoveToWorld( new Point3D( item.X - startX + point.X, item.Y - startY + point.Y, item.Z - startZ + point.Z ), from.Map );

                    worldcopy.Add( bItem );
                }

                foreach ( Item item in worldcopy )
                    ToggleHighlight( item, false );

                Usercopy[from] = worldcopy;

                from.SendMessage( 43, "Copied " + worlditems.Count + " items." );

                YesNo.SimpleConfirm( new YesNoCallbackState( CopyConfirm ), from, true, m_State );
                //from.SendGump( new MoveGump( from, m_State ) );

                return;

            }
            protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
            {
                if ( cancelType == TargetCancelType.Canceled ) {
                    from.SendMessage( "Cancelled" );
                    from.SendGump( new MoveGump( from, m_State ) );
                }
            }
        }

        public static void CopyConfirm( Mobile from, bool yes, object[] m_State )
        {
            if ( yes ) {

                Userdata[from] = Usercopy[from];
                Usercopy[from] = null;

                from.SendMessage( 43, "Copy complete!" );
                from.SendGump( new MoveGump( from, m_State ) );
            }
            else {
                ArrayList worlditems = Usercopy[from] as ArrayList;

                foreach ( Item item in worlditems )
                    item.Delete();

                worlditems = Userdata[from] as ArrayList;

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, false );

                Usercopy[from] = null;

                from.SendMessage( 43, "Copy removed!" );
                from.SendGump( new MoveGump( from, m_State ) );
            }

        }
        #endregion

        #region Move Target

        private class MoveTarget: Target
        {
            object[] m_State;

            public MoveTarget( object[] state )
                : base( 20, true, TargetFlags.None )
            {
                m_State = state;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                IPoint3D p = o as IPoint3D;

                if ( p == null ) {
                    from.SendGump( new MoveGump( from, m_State ) );
                    from.SendMessage( 43, "Something went wrong, try again." );
                    return;
                }

                if ( p is Item )
                    p = ( (Item)p ).GetWorldTop();
                else if ( p is Mobile )
                    p = ( (Mobile)p ).Location;

                Point3D point = new Point3D( p );

                int startX = 7144;
                int startY = 4096;
                int startZ = 127;

                ArrayList worlditems = Userdata[from] as ArrayList;

                foreach ( Item item in worlditems ) {

                    if ( item.X < startX )
                        startX = item.X;

                    if ( item.Y < startY )
                        startY = item.Y;

                    if ( item.Z < startZ )
                        startZ = item.Z;
                }

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, true );

                ArrayList worldcopy = new ArrayList();

                foreach ( Item item in worlditems ) {
                    Static bItem = new Static( item.ItemID );

                    bItem.Hue = item.Hue;
                    bItem.Name = item.Name;

                    if ( item.Amount > 1 ) {
                        bItem.Stackable = true;
                        bItem.Amount = item.Amount;
                    }

                    bItem.MoveToWorld( new Point3D( item.X - startX + point.X, item.Y - startY + point.Y, item.Z - startZ + point.Z ), from.Map );

                    worldcopy.Add( bItem );
                }

                foreach ( Item item in worldcopy )
                    ToggleHighlight( item, false );

                Usercopy[from] = worldcopy;

                from.SendMessage( 43, "Moved " + worlditems.Count + " items." );

                YesNo.SimpleConfirm( new YesNoCallbackState( MoveConfirm ), from, true, m_State );
                //from.SendGump( new MoveGump( from, m_State ) );

                return;

            }

            protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
            {
                if ( cancelType == TargetCancelType.Canceled ) {
                    from.SendMessage( "Cancelled" );
                    from.SendGump( new MoveGump( from, m_State ) );
                }
            }
        }

        public static void MoveConfirm( Mobile from, bool yes, object[] state )
        {
            if ( yes ) {

                ArrayList worlditems = Userdata[from] as ArrayList;

                foreach ( Item item in worlditems )
                    item.Delete();

                Userdata[from] = Usercopy[from];
                Usercopy[from] = null;

                from.SendMessage( 43, "Move complete!" );
                from.SendGump( new MoveGump( from, state ) );
            }
            else {
                ArrayList worldcopy = Usercopy[from] as ArrayList;

                foreach ( Item item in worldcopy )
                    item.Delete();

                ArrayList worlditems = Userdata[from] as ArrayList;

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, false );

                Usercopy[from] = null;

                from.SendMessage( 43, "Move aborted!" );
                from.SendGump( new MoveGump( from, state ) );
            }

        }

        #endregion

        #region Add Target
        private class AddTarget: Target
        {
            private bool m_remove;
            object[] m_State;

            public AddTarget( bool remove, object[] state )
                : base( 20, false, TargetFlags.None )
            {
                m_remove = remove;
                m_State = state;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if ( o == null ) {
                    from.SendGump( new MoveGump( from, m_State ) );
                    return;
                }

                if ( o is Item ) {

                    Item item = o as Item;

                    ArrayList worlditems;

                    if ( Userdata[from] != null )
                        worlditems = Userdata[from] as ArrayList;
                    else
                        worlditems = new ArrayList();

                    if ( !m_remove && worlditems.Contains( item ) == false ) {
                        worlditems.Add( item );
                        ToggleHighlight( item, m_remove );
                    }
                    else if ( m_remove && worlditems.Contains( item ) == true ) {
                        worlditems.Remove( item );
                        ToggleHighlight( item, m_remove );
                    }

                    Userdata[from] = worlditems;

                    from.Target = new AddTarget( m_remove, m_State );
                    return;
                }
                else {
                    from.SendMessage( 43, "You can only select items." );
                    from.Target = new AddTarget( m_remove, m_State );
                    return;
                }
            }
            protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
            {
                if ( cancelType == TargetCancelType.Canceled )
                    from.SendGump( new MoveGump( from, m_State ) );
            }
        }
        #endregion

        #region Check Target
        private class CheckTarget: Target
        {
            private string value;
            object[] m_State;

            public CheckTarget( string m_value, object[] state )
                : base( 20, true, TargetFlags.None )
            {
                value = m_value;
                m_State = state;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                int propvalue = 0;

                if ( o == null ) {
                    from.SendGump( new MoveGump( from, m_State ) );
                    from.SendMessage( 43, "Something went wrong, try again." );
                    return;
                }

                if ( value == "MoveMinZ" ) {
                    IPoint3D p = o as IPoint3D;
                    m_State[2] = p.Z;
                    propvalue = p.Z;
                }
                else if ( value == "MoveMaxZ" ) {
                    IPoint3D p = o as IPoint3D;
                    m_State[3] = p.Z;
                    propvalue = p.Z;
                }
                else if ( value == "MoveMinID" ) {
                    if ( o is Item ) {
                        Item item = o as Item;
                        m_State[5] = item.ItemID;
                        propvalue = item.ItemID;
                    }
                    else {
                        from.SendMessage( 43, "You can only select items." );
                        from.Target = new CheckTarget( value, m_State );
                        return;
                    }
                }
                else if ( value == "MoveMaxID" ) {
                    if ( o is Item ) {
                        Item item = o as Item;
                        m_State[6] = item.ItemID;
                        propvalue = item.ItemID;
                    }
                    else {
                        from.SendMessage( 43, "You can only select items." );
                        from.Target = new CheckTarget( value, m_State );
                        return;
                    }
                }
                from.SendGump( new MoveGump( from, m_State ) );
                from.SendMessage( 43, "Selection property set to: " + propvalue + "." );
                return;

            }
            protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
            {
                if ( cancelType == TargetCancelType.Canceled ) {
                    from.SendGump( new MoveGump( from, m_State ) );
                    from.SendMessage( 43, "Cancelled." );
                }
            }
        }
        #endregion

        #region Gump Constructor
        public MoveGump( Mobile from, object[] state )
            : base( 0, 0 )
        {
            m_State = state;

            zcheck = (bool)m_State[0];
            offset = (int)m_State[1];
            minZ = (int)m_State[2];
            maxZ = (int)m_State[3];
            IDcheck = (bool)m_State[4];
            minID = (int)m_State[5];
            maxID = (int)m_State[6];

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage( 0 );
            AddBackground( 80, 55, 300, 300, 9350 );

            AddButton( 122, 61, 4500, 4500, 1, GumpButtonType.Reply, 0 );
            AddButton( 122, 130, 4504, 4504, 2, GumpButtonType.Reply, 0 );
            AddButton( 86, 95, 4508, 4508, 3, GumpButtonType.Reply, 0 );
            AddButton( 156, 95, 4502, 4502, 4, GumpButtonType.Reply, 0 );
            AddButton( 155, 128, 4503, 4503, 5, GumpButtonType.Reply, 0 );
            AddButton( 155, 63, 4501, 4501, 6, GumpButtonType.Reply, 0 );
            AddButton( 87, 63, 4507, 4507, 7, GumpButtonType.Reply, 0 );
            AddButton( 87, 128, 4505, 4505, 8, GumpButtonType.Reply, 0 );

            AddButton( 261, 67, 4504, 4504, 9, GumpButtonType.Reply, 0 );
            AddButton( 295, 67, 4500, 4500, 10, GumpButtonType.Reply, 0 );

            AddButton( 86, 268, 1210, 1209, 20, GumpButtonType.Reply, 0 );
            AddLabel( 105, 266, 55, @"Select Area" );
            AddButton( 86, 246, 1210, 1209, 21, GumpButtonType.Reply, 0 );
            AddLabel( 105, 244, 55, @"Remove Area" );
            AddButton( 86, 224, 1210, 1209, 22, GumpButtonType.Reply, 0 );
            AddLabel( 105, 222, 55, @"Add Area" );
            AddButton( 86, 202, 1210, 1209, 28, GumpButtonType.Reply, 0 );
            AddLabel( 105, 202, 55, @"Clear Area" );

            AddButton( 191, 246, 1210, 1209, 26, GumpButtonType.Reply, 0 );
            AddLabel( 212, 244, 55, @"Add Target" );
            AddButton( 191, 268, 1210, 1209, 27, GumpButtonType.Reply, 0 );
            AddLabel( 212, 266, 55, @"Remove Target" );

            AddButton( 305, 268, 1210, 1209, 23, GumpButtonType.Reply, 0 );
            AddLabel( 324, 266, 55, @"Delete" );
            AddButton( 305, 246, 1210, 1209, 24, GumpButtonType.Reply, 0 );
            AddLabel( 324, 244, 55, @"Copy" );
            AddButton( 305, 224, 1210, 1209, 25, GumpButtonType.Reply, 0 );
            AddLabel( 324, 222, 55, @"Move" );


            AddCheck( 89, 300, 210, 211, zcheck, 1 );
            AddLabel( 117, 300, 55, @"Define Z" );
            AddButton( 180, 303, 1210, 1209, 29, GumpButtonType.Reply, 0 );
            AddTextEntry( 200, 300, 45, 20, 55, 2, minZ.ToString(), 5 );
            AddImageTiled( 200, 318, 40, 1, 9274 );
            AddLabel( 245, 300, 55, @"Min" );
            AddButton( 275, 303, 1210, 1209, 30, GumpButtonType.Reply, 0 );
            AddTextEntry( 295, 300, 45, 20, 55, 3, maxZ.ToString(), 5 );
            AddImageTiled( 295, 318, 40, 1, 9274 );
            AddLabel( 340, 300, 55, @"Max" );

            AddCheck( 89, 322, 210, 211, IDcheck, 2 );
            AddLabel( 117, 322, 55, @"Define ID" );
            AddButton( 180, 325, 1210, 1209, 31, GumpButtonType.Reply, 0 );
            AddTextEntry( 200, 322, 45, 20, 55, 4, minID.ToString(), 5 );
            AddImageTiled( 200, 340, 40, 1, 9274 );
            AddLabel( 245, 322, 55, @"Min" );
            AddButton( 275, 325, 1210, 1209, 32, GumpButtonType.Reply, 0 );
            AddTextEntry( 295, 322, 45, 20, 55, 5, maxID.ToString(), 5 );
            AddImageTiled( 295, 340, 40, 1, 9274 );
            AddLabel( 340, 322, 55, @"Max" );

            AddTextEntry( 318, 140, 30, 20, 55, 1, offset.ToString(), 3 );
            AddImageTiled( 318, 158, 30, 1, 9274 );
            AddLabel( 264, 140, 55, @"Offset:" );
        }
        #endregion

        #region Gump Response
        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            XmlValue XMLoffset = (XmlValue)XmlAttach.FindAttachment( from, typeof( XmlValue ), "MoveOffset" );

            XmlValue XMLZCheck = (XmlValue)XmlAttach.FindAttachment( from, typeof( XmlValue ), "MoveZCheck" );
            XmlValue XMLMinZ = (XmlValue)XmlAttach.FindAttachment( from, typeof( XmlValue ), "MoveMinZ" );
            XmlValue XMLMaxZ = (XmlValue)XmlAttach.FindAttachment( from, typeof( XmlValue ), "MoveMaxZ" );

            XmlValue XMLIDCheck = (XmlValue)XmlAttach.FindAttachment( from, typeof( XmlValue ), "MoveIDCheck" );
            XmlValue XMLMinID = (XmlValue)XmlAttach.FindAttachment( from, typeof( XmlValue ), "MoveMinID" );
            XmlValue XMLMaxID = (XmlValue)XmlAttach.FindAttachment( from, typeof( XmlValue ), "MoveMaxID" );

            offset = Utility.ToInt32( info.GetTextEntry( 1 ).Text );

            if ( info.IsSwitched( 1 ) ) {
                zcheck = true;
                minZ = Utility.ToInt32( info.GetTextEntry( 2 ).Text );
                maxZ = Utility.ToInt32( info.GetTextEntry( 3 ).Text );
            }
            else {
                zcheck = false;
            }

            if ( info.IsSwitched( 2 ) ) {
                IDcheck = true;
                minID = Utility.ToInt32( info.GetTextEntry( 4 ).Text );
                maxID = Utility.ToInt32( info.GetTextEntry( 5 ).Text );
            }
            else {
                IDcheck = false;
            }

            m_State[0] = zcheck;
            m_State[1] = offset;
            m_State[2] = minZ;
            m_State[3] = maxZ;
            m_State[4] = IDcheck;
            m_State[5] = minID;
            m_State[6] = maxID;

            switch ( info.ButtonID ) {
                case 0: { //Dismiss
                        if ( Userdata[from] != null ) {
                            ArrayList worlditems = Userdata[from] as ArrayList;

                            foreach ( Item item in worlditems )
                                ToggleHighlight( item, true );

                            Userdata[from] = null;
                        }
                        break;
                    }
                case 1: { //North
                        Offset( from, -offset, -offset, 0 );

                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 2: { //South
                        Offset( from, offset, offset, 0 );
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 3: { //West
                        Offset( from, -offset, offset, 0 );
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 4: { //East
                        Offset( from, offset, -offset, 0 );
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 5: { //Southeast
                        Offset( from, offset, 0, 0 );
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 6: { //Northeast
                        Offset( from, 0, -offset, 0 );
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 7: { //Northwest
                        Offset( from, -offset, 0, 0 );
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 8: { //Southwest
                        Offset( from, 0, offset, 0 );
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 9: { //Down
                        Offset( from, 0, 0, -offset );
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 10: { //Up
                        Offset( from, 0, 0, offset );
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 20: { //Select
                        BoundingBoxPicker.Begin( from, new BoundingBoxCallback( SelectOnTarget ), new object[] { false, false, zcheck, IDcheck } );
                        from.SendGump( new MoveGump( from, m_State ) );

                        break;
                    }
                case 21: { //Remove
                        if ( Userdata[from] != null )
                            BoundingBoxPicker.Begin( from, new BoundingBoxCallback( SelectOnTarget ), new object[] { false, true, zcheck, IDcheck } );
                        else
                            from.SendMessage( 43, "There is no area selected to remove." );

                        from.SendGump( new MoveGump( from, m_State ) );


                        break;
                    }
                case 22: { //Add

                        BoundingBoxPicker.Begin( from, new BoundingBoxCallback( SelectOnTarget ), new object[] { true, false, zcheck, IDcheck } );
                        from.SendGump( new MoveGump( from, m_State ) );

                        break;
                    }
                case 23: { //Delete
                        if ( Userdata[from] != null )
                            YesNo.SimpleConfirm( new YesNoCallbackState( DeleteSelection ), from, false, m_State );
                        else {
                            from.SendMessage( 43, "There is no area selected to delete." );
							from.SendGump( new MoveGump( from, m_State ) );
                        }
                        break;
                    }
                case 24: { //Copy
                        if ( Userdata[from] != null ) {
                            from.Target = new CopyTarget( m_State );
                            from.SendMessage( 43, "Target the top left corner of where you want the copy to be placed. Esc to cancel." );
                        }
                        else {
                            from.SendMessage( 43, "There is no area selected to copy." );
                            from.SendGump( new MoveGump( from, m_State ) );
                        }
                        break;
                    }
                case 25: { //Move
                        if ( Userdata[from] != null ) {
                            from.Target = new MoveTarget( m_State );
                            from.SendMessage( 43, "Target the top left corner of the new placement. Esc to cancel." );
                        }
                        else {
                            from.SendMessage( 43, "There is no area selected to move." );
                            from.SendGump( new MoveGump( from, m_State ) );
                        }
                        break;
                    }
                case 26: { //Add Target
                        from.Target = new AddTarget( false, m_State );
                        from.SendMessage( 43, "Target the item you want to add to the selection. Esc to cancel." );
                        break;
                    }
                case 27: { //Remove Target					
                        from.Target = new AddTarget( true, m_State );
                        from.SendMessage( 43, "Target the item you want to remove from the selection. Esc to cancel." );
                        break;
                    }
                case 28: { //Clear Area
                        if ( Userdata[from] != null ) {
                            ArrayList worlditems = Userdata[from] as ArrayList;

                            foreach ( Item item in worlditems )
                                ToggleHighlight( item, true );

                            Userdata[from] = null;
                            from.SendMessage( 43, "Selection cleared!" );
                        }
                        from.SendGump( new MoveGump( from, m_State ) );
                        break;
                    }
                case 29: { //MinZ Selector					
                        from.Target = new CheckTarget( "MoveMinZ", m_State );
                        from.SendMessage( 43, "Select the minimum Z you want to use. Esc to cancel." );
                        break;
                    }
                case 30: { //MaxZ Selector					
                        from.Target = new CheckTarget( "MoveMaxZ", m_State );
                        from.SendMessage( 43, "Select the maximum Z you want to use. Esc to cancel." );
                        break;
                    }
                case 31: { //MinID Selector					
                        from.Target = new CheckTarget( "MoveMinID", m_State );
                        from.SendMessage( 43, "Select the minimum ID you want to use. Esc to cancel." );
                        break;
                    }
                case 32: { //MaxID Selector					
                        from.Target = new CheckTarget( "MoveMaxID", m_State );
                        from.SendMessage( 43, "Select the maximum ID you want to use. Esc to cancel." );
                        break;
                    }
                default: break;
            }
        }
        #endregion
    }
}
