using System.Collections;
using System.Collections.Generic;

using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Items;

using Server.Engines.XmlSpawner2;
using System.IO;
using System;

namespace Server.Commands
{
    public class CopyPaste : Gump
    {
        #region Fields

        private static Hashtable Copydata;
        private object[] m_State;

        private bool zcheck;
        private int minZ;
        private int maxZ;
        private bool IDcheck;
        private int minID;
        private int maxID;
        private bool IncStatics;
        private string fileName;

        #endregion

        #region Static Methods
        public static void Initialize()
        {
            Copydata = new Hashtable();

            CommandSystem.Register( "CopyPaste", AccessLevel.Administrator, new CommandEventHandler( CopyPaste_OnCommand ) );
            CommandSystem.Register( "Paste", AccessLevel.Administrator, new CommandEventHandler( Paste_OnCommand ) );
            EventSink.Disconnected += new DisconnectedEventHandler( EventSink_Disconnect );
        }

        public static void EventSink_Disconnect( DisconnectedEventArgs e )
        {
            if ( Copydata[e.Mobile] != null ) {

                ArrayList worlditems = Copydata[e.Mobile] as ArrayList;

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, true );

                Copydata[e.Mobile] = null;
            }

        }

        [Usage( ".CopyPaste" )]
        [Description( "Allows a staff member to save buildings to file." )]
        private static void CopyPaste_OnCommand( CommandEventArgs e )
        {
            if ( e.Mobile.HasGump( typeof( CopyPaste ) ) )
                e.Mobile.CloseGump( typeof( CopyPaste ) );

            object[] state = new object[8];

            state[0] = false;
            state[1] = false;
            state[2] = 0;
            state[3] = 127;
            state[4] = false;
            state[5] = 2;
            state[6] = 16300;
            state[7] = "";

            e.Mobile.SendGump( new CopyPaste( e.Mobile, state ) );
        }

        [Usage( ".Paste <filename>" )]
        [Description( "Allows a staff member to paste a saved building from file." )]
        private static void Paste_OnCommand( CommandEventArgs e )
        {
            if ( !( e.Arguments.Length > 0 ) ) {
                e.Mobile.SendMessage( MessageUtil.MessageColorPlayer, "Usage: .paste <Filename>" );
                return;
            }

            string text = e.ArgString;//e.GetString(0);

            string path;
           
            path = Path.Combine( Core.BaseDirectory, string.Format( @"Structures\{0}.struct", text ) );

            if ( !File.Exists( path ) ) {
                e.Mobile.SendMessage( 522, "That file does not exist." );
                return;
            }

            e.Mobile.Target = new PasteTarget( path );
            e.Mobile.SendMessage( 522, "Target where you wish to paste the structure." );

        }

        #endregion

        #region Sub routines

        private static void ToggleHighlight( Item item, bool remove )
        {
            XmlHue hue = (XmlHue)XmlAttach.FindAttachment( item, typeof( XmlHue ) );

            if ( item != null ) {
                if ( hue == null && !remove )
                    XmlAttach.AttachTo( item, new XmlHue( 522, 30.0 ) );
                else if ( remove && hue != null )
                    hue.Delete();
            }
        }

        private static void CleanUpTempItems( Item item )
        {
            TemporaryQuestObject questattach = (TemporaryQuestObject)XmlAttach.FindAttachment( item, typeof( TemporaryQuestObject ) );

            if ( questattach != null )
                item.Delete();
        }

        public static void WriteToFile( Mobile from, bool yes, object[] state )
        {
            if ( yes ) {
                ArrayList worlditems = Copydata[from] as ArrayList;
                object[] states = (object[])state;

                string fileName = (string)states[7];

                if ( fileName == null || fileName.Length < 1 ) {
                    from.SendMessage( 522, "You have to enter a file name." );
                    from.SendGump( new CopyPaste( from, states ) );
                    return;
                }

                if ( fileName.Contains( "/" ) || fileName.Contains( "\\" ) || fileName.Contains( "." ) || fileName.Contains( "}" ) || fileName.Contains( "{" ) || fileName.Contains( "|" ) ) {
                    from.SendMessage( 522, "Invalid file name." );
                    from.SendGump( new CopyPaste( from, states ) );
                    return;
                }

                string path;

                path = Path.Combine( Core.BaseDirectory, string.Format( @"Structures\{0}.struct", fileName ) );

                //Create folder if it doesn't exist
                string folder = Path.GetDirectoryName( path );

                if ( !Directory.Exists( folder ) ) {
                    Directory.CreateDirectory( folder );
                }

                // check to see if the file already exists and can be written to by the owner
                if ( System.IO.File.Exists( path ) ) {
                    from.SendMessage( 522, "That file already exists." );
                    from.SendGump( new CopyPaste( from, states ) );
                    return;
                }

                int x = 7144;
                int y = 4096;
                int z = 127;

                foreach ( Item item in worlditems ) {
                    if ( item.X < x )
                        x = item.X;

                    if ( item.Y < y )
                        y = item.Y;

                    if ( item.Z < z )
                        z = item.Z;
                }


                StreamWriter writer = new StreamWriter( path, false );

                if ( writer != null ) {
                    // write the header
                    writer.WriteLine( "{0} - {1}, {2}", from.Name, from.Account, DateTime.Now );
                    writer.WriteLine( "{0} items", worlditems.Count );

                    // write out the items
                    foreach ( Item item in worlditems ) {

                        ToggleHighlight( item, true );

                        // format is x y z visible hue amount name
                        writer.WriteLine( "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", item.ItemID, item.X - x, item.Y - y, item.Z - z, item.Visible ? 1 : 0, item.Hue, item.Amount, item.Name );

                        ToggleHighlight( item, false );
                    }

                    from.SendMessage( 522, "You wrote {0} items to {1}.", worlditems.Count, fileName );
                    from.SendGump( new CopyPaste( from, state ) );
                    writer.Close();
                }
            }

            else {
                from.SendGump( new CopyPaste( from, state ) );
                from.SendMessage( 522, "Cancelled." );
            }
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



            if ( add && Copydata[from] != null ) {
                Rectangle2D rect = new Rectangle2D( start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1 );
                IPooledEnumerable eable = map.GetItemsInBounds( rect );

                ArrayList worlditems = Copydata[from] as ArrayList;

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, false );

                foreach ( Item item in eable ) {
                    TemporaryQuestObject questattach = (TemporaryQuestObject)XmlAttach.FindAttachment( item, typeof( TemporaryQuestObject ) );

                    if ( questattach == null ) {
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
                }

                from.SendMessage( 522, "You have selected {0} items.", worlditems.Count );

                int selected = worlditems.Count;

                if ( IncStatics ) {

                    int xStartBlock = start.X >> 3;
                    int yStartBlock = start.Y >> 3;
                    int xEndBlock = end.X >> 3;
                    int yEndBlock = end.Y >> 3;

                    int xTileStart = start.X, yTileStart = start.Y;
                    int xTileWidth = end.X - start.X + 1, yTileHeight = end.Y - start.Y + 1;

                    TileMatrix matrix = map.Tiles;

                    using ( FileStream idxStream = Statics.OpenWrite( matrix.IndexStream ) ) {
                        using ( FileStream mulStream = Statics.OpenWrite( matrix.DataStream ) ) {
                            if ( idxStream == null || mulStream == null ) {
                                return;
                            }

                            BinaryReader idxReader = new BinaryReader( idxStream );

                            BinaryWriter idxWriter = new BinaryWriter( idxStream );

                            for ( int x = xStartBlock; x <= xEndBlock; ++x ) {
                                for ( int y = yStartBlock; y <= yEndBlock; ++y ) {
                                    int oldTileCount;
                                    StaticTile[] oldTiles = Statics.ReadStaticBlock( idxReader, mulStream, x, y, matrix.BlockWidth, matrix.BlockHeight, out oldTileCount );

                                    if ( oldTileCount < 0 )
                                        continue;

                                    int newTileCount = 0;
                                    StaticTile[] newTiles = new StaticTile[oldTileCount];

                                    int baseX = ( x << 3 ) - xTileStart, baseY = ( y << 3 ) - yTileStart;

                                    for ( int i = 0; i < oldTileCount; ++i ) {
                                        StaticTile oldTile = oldTiles[i];

                                        int px = baseX + oldTile.X;
                                        int py = baseY + oldTile.Y;

                                        if ( px < 0 || px >= xTileWidth || py < 0 || py >= yTileHeight ) {
                                            newTiles[newTileCount++] = oldTile;
                                        }
                                        else {
                                            int itemID = oldTile.ID & 0x3FFF;

                                            if ( countZ && countID && itemID < minID && itemID > maxID && oldTile.Z < minZ && oldTile.Z > maxZ )
                                                continue;

                                            else if ( ( countZ && oldTile.Z < minZ && oldTile.Z > maxZ ) ) {
                                                continue;
                                            }
                                            else if ( ( countID && itemID < minID && itemID > maxID ) ) {
                                                continue;
                                            }
                                            else {

                                                Item item = new Static( itemID );

                                                item.Hue = oldTile.Hue;

                                                item.MoveToWorld( new Point3D( px + xTileStart, py + yTileStart, oldTile.Z ), map );

                                                XmlAttach.AttachTo( item, new TemporaryQuestObject( "Static Item", 30 ) );
                                                worlditems.Add( item );
                                                ToggleHighlight( item, false );


                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    from.SendMessage( 522, "You selected {0} static items.", worlditems.Count - selected );
                }

                Copydata[from] = worlditems;
                worlditems = null;

                return;
            }
            else if ( remove ) {
                Rectangle2D rect = new Rectangle2D( start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1 );
                IPooledEnumerable eable = map.GetItemsInBounds( rect );

                ArrayList worlditems = Copydata[from] as ArrayList;

                ArrayList worlddelete = new ArrayList();

                foreach ( Item item in worlditems )
                    ToggleHighlight( item, false );

                foreach ( Item item in eable ) {
                    if ( worlditems.Contains( item ) == true ) {
                        if ( countZ && countID ) {
                            if ( item.ItemID >= minID && item.ItemID <= maxID && item.Z >= minZ && item.Z <= maxZ ) {
                                worlditems.Remove( item );
                                ToggleHighlight( item, true );
                                worlddelete.Add( item );
                            }
                        }
                        else if ( countZ && item.Z >= minZ && item.Z <= maxZ ) {
                            worlditems.Remove( item );
                            ToggleHighlight( item, true );
                            worlddelete.Add( item );
                        }
                        else if ( countID && item.ItemID >= minID && item.ItemID <= maxID ) {
                            worlditems.Remove( item );
                            ToggleHighlight( item, true );
                            worlddelete.Add( item );
                        }
                        else if ( !countID && !countZ ) {
                            worlditems.Remove( item );
                            ToggleHighlight( item, true );
                            worlddelete.Add( item );
                        }
                    }
                }

                foreach ( Item item in worlddelete )
                    CleanUpTempItems( item );

                Copydata[from] = worlditems;

                from.SendMessage( 522, "You have selected {0} items.", worlditems.Count );

                return;
            }
            else { //Set

                ArrayList worlditems;

                if ( Copydata[from] != null ) {
                    worlditems = Copydata[from] as ArrayList;

                    foreach ( Item item in worlditems )
                        ToggleHighlight( item, true );
                }

                worlditems = new ArrayList();

                Rectangle2D rect = new Rectangle2D( start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1 );
                IPooledEnumerable eable = map.GetItemsInBounds( rect );

                foreach ( Item item in eable ) {
                    TemporaryQuestObject questattach = (TemporaryQuestObject)XmlAttach.FindAttachment( item, typeof( TemporaryQuestObject ) );

                    if ( questattach == null ) {
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
                from.SendMessage( 522, "You have selected {0} items.", worlditems.Count );

                int selected = worlditems.Count;

                if ( IncStatics ) {
                    //start = map.Bound( start );
                    //end = map.Bound( end );

                    int xStartBlock = start.X >> 3;
                    int yStartBlock = start.Y >> 3;
                    int xEndBlock = end.X >> 3;
                    int yEndBlock = end.Y >> 3;

                    int xTileStart = start.X, yTileStart = start.Y;
                    int xTileWidth = end.X - start.X + 1, yTileHeight = end.Y - start.Y + 1;

                    TileMatrix matrix = map.Tiles;

                    using ( FileStream idxStream = Statics.OpenWrite( matrix.IndexStream ) ) {
                        using ( FileStream mulStream = Statics.OpenWrite( matrix.DataStream ) ) {
                            if ( idxStream == null || mulStream == null ) {
                                return;
                            }

                            BinaryReader idxReader = new BinaryReader( idxStream );

                            BinaryWriter idxWriter = new BinaryWriter( idxStream );

                            for ( int x = xStartBlock; x <= xEndBlock; ++x ) {
                                for ( int y = yStartBlock; y <= yEndBlock; ++y ) {
                                    int oldTileCount;
                                    StaticTile[] oldTiles = Statics.ReadStaticBlock( idxReader, mulStream, x, y, matrix.BlockWidth, matrix.BlockHeight, out oldTileCount );

                                    if ( oldTileCount < 0 )
                                        continue;

                                    int newTileCount = 0;
                                    StaticTile[] newTiles = new StaticTile[oldTileCount];

                                    int baseX = ( x << 3 ) - xTileStart, baseY = ( y << 3 ) - yTileStart;

                                    for ( int i = 0; i < oldTileCount; ++i ) {
                                        StaticTile oldTile = oldTiles[i];

                                        int px = baseX + oldTile.X;
                                        int py = baseY + oldTile.Y;

                                        if ( px < 0 || px >= xTileWidth || py < 0 || py >= yTileHeight ) {
                                            newTiles[newTileCount++] = oldTile;
                                        }
                                        else {
                                            int itemID = oldTile.ID & 0x3FFF;

                                            if ( ( countZ && countID ) && ( itemID < minID || itemID > maxID || oldTile.Z < minZ || oldTile.Z > maxZ ) )
                                                continue;

                                            else if ( ( countZ && ( oldTile.Z < minZ || oldTile.Z > maxZ ) ) ) {
                                                continue;
                                            }
                                            else if ( ( countID && ( itemID < minID || itemID > maxID ) ) ) {
                                                continue;
                                            }
                                            else {

                                                Item item = new Static( itemID );

                                                item.Hue = oldTile.Hue;

                                                item.MoveToWorld( new Point3D( px + xTileStart, py + yTileStart, oldTile.Z ), map );

                                                XmlAttach.AttachTo( item, new TemporaryQuestObject( "Static Item", 30 ) );
                                                worlditems.Add( item );
                                                ToggleHighlight( item, false );
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        from.SendMessage( 522, "You selected {0} static items.", worlditems.Count - selected );

                    }
                }
                Copydata[from] = worlditems;

                return;
            }
        }

        #endregion

        #region Paste Target
        private class PasteTarget : Target
        {
            private string path;

            public PasteTarget( string m_path )
                : base( 20, true, TargetFlags.None )
            {
                path = m_path;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                IPoint3D p = o as IPoint3D;

                if ( p == null ) {
                    //from.SendGump( new CopyPaste( from ) );
                    from.SendMessage( 522, "Something went wrong, try again." );
                    return;
                }

                if ( p is Item )
                    p = ( (Item)p ).GetWorldTop();
                else if ( p is Mobile )
                    p = ( (Mobile)p ).Location;

                Point3D point = new Point3D( p );

                StreamReader file = null;
                string line;
                string[] entry;

                ArrayList items = new ArrayList();

                try {
                    file = new StreamReader( path );
                    while ( ( line = file.ReadLine() ) != null ) {

                        entry = line.Split( '|' );

                        if ( entry.Length > 3 ) {
                            Static item = new Static( Utility.ToInt32( entry[0] ) );

                            if ( Utility.ToInt32( entry[4] ) == 1 )
                                item.Visible = true;
                            else
                                item.Visible = false;

                            item.Hue = Utility.ToInt32( entry[5] );

                            if ( Utility.ToInt32( entry[6] ) > 1 ) {
                                item.Stackable = true;
                                item.Amount = Utility.ToInt32( entry[6] );
                            }

                            if ( entry[7] != null && entry[7].Length > 0 )
                                item.Name = entry[7];

                            item.MoveToWorld( new Point3D( Utility.ToInt32( entry[1] ) + point.X, Utility.ToInt32( entry[2] ) + point.Y, Utility.ToInt32( entry[3] ) + point.Z ), from.Map );
                            items.Add( item );
                        }
                    }
                }
                finally {
                    if ( file != null )
                        file.Close();
                }

                //foreach ( Item item in items )
                //	MoveGump.ToggleHighlight( item, false );

                YesNo.SimpleConfirm( new YesNoCallbackState( PasteConfirm ), from, false, new object[] { items } );
            }
            protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
            {
                if ( cancelType == TargetCancelType.Canceled )
                    from.SendMessage( MessageUtil.MessageColorGM, "Cancelled." );
            }
        }

        public static void PasteConfirm( Mobile from, bool yes, object[] state )
        {
            ArrayList worlditems = state[0] as ArrayList;

            if ( yes ) {
                MoveGump.InvokeMassMove( from, worlditems );
                from.SendMessage( 522, "Paste completed." );
            }
            else {
                foreach ( Item item in worlditems )
                    item.Delete();
                from.SendMessage( 522, "Paste cancelled." );

            }

        }
        #endregion

        #region Add Target
        private class AddTarget : Target
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
                    from.SendGump( new CopyPaste( from, m_State ) );
                    return;
                }

                if ( o is Item ) {

                    Item item = o as Item;

                    ArrayList worlditems;

                    if ( Copydata[from] != null )
                        worlditems = Copydata[from] as ArrayList;
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

                    Copydata[from] = worlditems;

                    from.Target = new AddTarget( m_remove, m_State );
                    return;
                }
                else {
                    from.SendMessage( 522, "You can only select items." );
                    from.Target = new AddTarget( m_remove, m_State );
                    return;
                }
            }
            protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
            {
                if ( cancelType == TargetCancelType.Canceled )
                    from.SendGump( new CopyPaste( from, m_State ) );
            }
        }
        #endregion

        #region Check Target
        private class CheckTarget : Target
        {
            //private XmlValue value;
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
                    from.SendGump( new CopyPaste( from, m_State ) );
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
                from.SendGump( new CopyPaste( from, m_State ) );
                from.SendMessage( 43, "Selection property set to: " + propvalue + "." );
                return;

            }
            protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
            {
                if ( cancelType == TargetCancelType.Canceled ) {
                    from.SendGump( new CopyPaste( from, m_State ) );
                    from.SendMessage( 522, "Cancelled." );
                }
            }
        }
        #endregion

        #region Gump Constructor
        public CopyPaste( Mobile from, object[] state )
            : base( 0, 0 )
        {
            m_State = state;

            zcheck = (bool)m_State[0];
            IncStatics = (bool)m_State[1];
            minZ = (int)m_State[2];
            maxZ = (int)m_State[3];
            IDcheck = (bool)m_State[4];
            minID = (int)m_State[5];
            maxID = (int)m_State[6];
            fileName = (string)m_State[7];

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage( 0 );
            AddBackground( 80, 55, 300, 220, 9350 );

            AddButton( 86, 138, 1210, 1209, 20, GumpButtonType.Reply, 0 );
            AddLabel( 105, 136, 55, @"Select Area" );
            AddButton( 86, 116, 1210, 1209, 21, GumpButtonType.Reply, 0 );
            AddLabel( 105, 114, 55, @"Remove Area" );
            AddButton( 86, 94, 1210, 1209, 22, GumpButtonType.Reply, 0 );
            AddLabel( 105, 92, 55, @"Add Area" );
            AddButton( 86, 72, 1210, 1209, 28, GumpButtonType.Reply, 0 );
            AddLabel( 105, 70, 55, @"Clear Area" );

            AddButton( 191, 116, 1210, 1209, 26, GumpButtonType.Reply, 0 );
            AddLabel( 212, 114, 55, @"Add Target" );
            AddButton( 191, 138, 1210, 1209, 27, GumpButtonType.Reply, 0 );
            AddLabel( 212, 136, 55, @"Remove Target" );

            AddCheck( 89, 170, 210, 211, zcheck, 1 );
            AddLabel( 117, 170, 55, @"Define Z" );
            AddButton( 180, 173, 1210, 1209, 29, GumpButtonType.Reply, 0 );
            AddTextEntry( 200, 170, 45, 20, 55, 2, minZ.ToString(), 5 );
            AddImageTiled( 200, 188, 40, 1, 9274 );
            AddLabel( 245, 170, 55, @"Min" );
            AddButton( 275, 173, 1210, 1209, 30, GumpButtonType.Reply, 0 );
            AddTextEntry( 295, 170, 45, 20, 55, 3, maxZ.ToString(), 5 );
            AddImageTiled( 295, 188, 40, 1, 9274 );
            AddLabel( 340, 170, 55, @"Max" );

            AddCheck( 89, 192, 210, 211, IDcheck, 2 );
            AddLabel( 117, 192, 55, @"Define ID" );
            AddButton( 180, 195, 1210, 1209, 31, GumpButtonType.Reply, 0 );
            AddTextEntry( 200, 192, 45, 20, 55, 4, minID.ToString(), 5 );
            AddImageTiled( 200, 210, 40, 1, 9274 );
            AddLabel( 245, 192, 55, @"Min" );
            AddButton( 275, 195, 1210, 1209, 32, GumpButtonType.Reply, 0 );
            AddTextEntry( 295, 192, 45, 20, 55, 5, maxID.ToString(), 5 );
            AddImageTiled( 295, 210, 40, 1, 9274 );
            AddLabel( 340, 192, 55, @"Max" );

            AddCheck( 89, 214, 210, 211, IncStatics, 3 );
            AddLabel( 117, 214, 55, @"Include Statics" );

            //Save
            AddButton( 300, 250, GumpUtil.ButtonGreenOKUp, GumpUtil.ButtonGreenOKDown, 100, GumpButtonType.Reply, 0 );
            //Filename
            AddTextEntry( 150, 246, 125, 20, 55, 6, fileName, 12 );
            AddImageTiled( 150, 264, 120, 1, 9274 );
            AddLabel( 95, 246, 55, @"Filename" );

        }
        #endregion

        #region Gump Response
        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;


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

            if ( info.IsSwitched( 3 ) ) {
                IncStatics = true;
            }
            else
                IncStatics = false;

            fileName = info.GetTextEntry( 6 ).Text;

            m_State[0] = zcheck;
            m_State[1] = IncStatics;
            m_State[2] = minZ;
            m_State[3] = maxZ;
            m_State[4] = IDcheck;
            m_State[5] = minID;
            m_State[6] = maxID;
            m_State[7] = fileName;

            switch ( info.ButtonID ) {
                case 0: { //Dismiss
                        if ( Copydata[from] != null ) {
                            ArrayList worlditems = Copydata[from] as ArrayList;

                            foreach ( Item item in worlditems ) {
                                ToggleHighlight( item, true );
                                CleanUpTempItems( item );
                            }

                            Copydata[from] = null;
                        }
                        break;
                    }
                case 20: { //Select
                        BoundingBoxPicker.Begin( from, new BoundingBoxCallback( SelectOnTarget ), new object[] { false, false, zcheck, IDcheck, IncStatics } );
                        from.SendGump( new CopyPaste( from, m_State ) );

                        break;
                    }
                case 21: { //Remove
                        if ( Copydata[from] != null )
                            BoundingBoxPicker.Begin( from, new BoundingBoxCallback( SelectOnTarget ), new object[] { false, true, zcheck, IDcheck, IncStatics } );
                        else
                            from.SendMessage( 522, "There is no area selected to remove." );

                        from.SendGump( new CopyPaste( from, m_State ) );


                        break;
                    }
                case 22: { //Add

                        BoundingBoxPicker.Begin( from, new BoundingBoxCallback( SelectOnTarget ), new object[] { true, false, zcheck, IDcheck, IncStatics } );
                        from.SendGump( new CopyPaste( from, m_State ) );

                        break;
                    }
                case 26: { //Add Target
                        from.Target = new AddTarget( false, m_State );
                        from.SendMessage( 522, "Target the item you want to add to the selection. Esc to cancel." );
                        break;
                    }
                case 27: { //Remove Target					
                        from.Target = new AddTarget( true, m_State );
                        from.SendMessage( 522, "Target the item you want to remove from the selection. Esc to cancel." );
                        break;
                    }
                case 28: { //Clear Area
                        if ( Copydata[from] != null ) {
                            ArrayList worlditems = Copydata[from] as ArrayList;

                            foreach ( Item item in worlditems ) {
                                ToggleHighlight( item, true );
                                CleanUpTempItems( item );
                            }

                            Copydata[from] = null;
                            from.SendMessage( 522, "Selection cleared!" );
                        }
                        from.SendGump( new CopyPaste( from, m_State ) );
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
                case 100: { //Write to file

                        if ( Copydata[from] == null ) {
                            from.SendMessage( 522, "There is no area selected." );
                            from.SendGump( new CopyPaste( from, m_State ) );
                        }
                        else if ( fileName == null || fileName.Length < 1 ) {
                            from.SendMessage( 522, "You have to enter a file name." );
                            from.SendGump( new CopyPaste( from, m_State ) );
                            return;
                        }
                        else {
                            YesNo.SimpleConfirm( new YesNoCallbackState( WriteToFile ), from, false, m_State );
                        }
                        break;
                    }

                default: break;
            }
        }
        #endregion
    }
}
