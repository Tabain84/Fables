using System;
using System.Collections.Generic;
using System.Text;

using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Exp
{
    class ExpGain
    {
        public static void AwardExperience( PlayerMobile pm, int amount, bool obeyCap )
        {
            AwardExperience( pm, amount, obeyCap, true );
        }

        public static void AwardExperience( PlayerMobile pm, int amount, bool obeyCap, bool message )
        {
            if ( obeyCap ) {
                if ( pm.DailyExpReset < DateTime.Now ) {
                    pm.DailyExpReset = DateTime.Now + TimeSpan.FromDays( 1.0 );
                    pm.DailyExperience = 0;
                }

                if (pm.CurrentExperience + amount > ExpMaster.TotalMax)
                    amount = ExpMaster.TotalMax - pm.CurrentExperience;

                if ( pm.DailyExperience <= ExpMaster.DailyMaxExp ) {

                    if ( pm.DailyExperience + amount <= ExpMaster.DailyMaxExp )
                        pm.DailyExperience += amount;
                    else {
                        amount = ExpMaster.DailyMaxExp - pm.DailyExperience;
                        pm.DailyExperience = ExpMaster.DailyMaxExp;
                    }

                    pm.CurrentExperience += amount;
                    pm.TotalExperience += amount;
                    //pm.DailyExperience += amount;

                    if ( amount > 0 && message )
                        pm.SendMessage( MessageUtil.MessageColorPlayer, "You earned {0} experience!", amount );
                }
            }
            else {
                pm.CurrentExperience += amount;
                pm.TotalExperience += amount;
                if ( message )
                    pm.SendMessage( MessageUtil.MessageColorPlayer, "You earned {0} experience!", amount );
            }
        }

        public static void AwardFameExp( PlayerMobile pm, int fame )
        {
            if ( fame <= 250 )
                return;

            int amount = 5;

            /*else if ( fame < 1000 ) {
                if ( Utility.RandomMinMax(1, 3) == 2 )
                    amount = 1;
            }
            else if ( fame < 1000 ) {
//				if ( Utility.RandomBool() )
                    amount = 5;
            }
            else*/
            amount += fame / 300;

            //if ( amount > 25 )
            //  amount = 25;

            if ( pm.Hunger == 0 )
                amount = (int)( amount * 0.8 );

            amount = (int)(amount * Server.Settings.FameExpMod);

            AwardExperience( pm, amount, true );
        }

        public static void AwardCraftExp( PlayerMobile pm, int skill )
        {
            int amount = skill / 2;

            //if ( amount > 20 )
            //  amount = 20;
            //else if ( amount < 5 )
            //  amount = 5;

            if ( pm.Hunger == 0 )
                amount = (int)( amount * 0.8 );

            amount = (int)(amount * Server.Settings.CraftExpMod);

            AwardExperience( pm, (int)amount, true );
        }
    }
}
