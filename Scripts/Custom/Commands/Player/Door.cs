using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Commands
{
    public class DoorCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register( "Door", AccessLevel.Player, new CommandEventHandler( Door_OnCommand ) );
        }

        [Usage( "Door" )]
        [Description( "Opens all doors within 2 tile range. Locked doors will open only when you have the key." )]
        private static void Door_OnCommand( CommandEventArgs e )
        {
            List<BaseDoor> doors = new List<BaseDoor>();
            IPooledEnumerable eable = e.Mobile.GetObjectsInRange( 2 );
            foreach ( Object o in eable ) {
                if ( o is BaseDoor ) doors.Add( (BaseDoor)o );
            }
            eable.Free();
            for ( int i = 0; i < doors.Count; ++i ) {
                doors[i].Use( e.Mobile );
            }
        }
    }
}