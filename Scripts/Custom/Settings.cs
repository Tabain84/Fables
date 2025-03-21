using System;

namespace Server
{
    public class Settings
    {
        public const int goldModifier = 1;
        public const int fameGainModifier = 1;

        //How often the timer loops in seconds. Determines how often skills are trained.
        public static double TimerCycle = 5.0;

        //How often there will be a chance to gain a stat point. 50% chance, double the time for average gain time.
        public static TimeSpan StatDelay = TimeSpan.FromMinutes(1.5);

        //How often warning messages will be spammed, for spec and AFK.
        public static TimeSpan MessageDelay = TimeSpan.FromMinutes(1);

        //Idle time until booted for being idle
        public static TimeSpan AFKTime = TimeSpan.FromMinutes(60.0);

        //Time until automatically waking up from KO, incase the death system bugs.
        public static TimeSpan ResTime = TimeSpan.FromMinutes(10.0);

        //Passive experience gained per hour.
        public static int PassiveExpHour = 2000;

        //Modifier for how much experience is gained from killing creatures based on fame.
        public const double FameExpMod = 3;

        //Modifier for how much experience is gained from crafting items, based on difficulty. 
        public const double CraftExpMod = 1.5;
    }
}