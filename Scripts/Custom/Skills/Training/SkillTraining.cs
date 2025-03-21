using System;
using System.Collections.Generic;
using System.Text;

using Server.Mobiles;
using Server.Commands;

namespace Server.Training
{
    class SkillTraining
    {
        public static void Initialize()
        {
            CommandSystem.Register( "Training", AccessLevel.Player, new CommandEventHandler( OnCommand_Training ) );
        }

        [Usage( ".training" )]
        [Description( "Displays what you're currently training, for how long, and how many rested points you have." )]
        private static void OnCommand_Training( CommandEventArgs e )
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;


            foreach ( KeyValuePair<SkillName, int> kvp in pm.TrainingPoints ) {
                if ( kvp.Value > 6 ) {
                    pm.SendMessage( MessageUtil.MessageColorPlayer, "Training {0} for {1}.", pm.Skills[kvp.Key].Name, TrainMaster.GetTrainingTimeString( pm, kvp.Key ) );
                }
            }

            pm.SendMessage( MessageUtil.MessageColorPlayer, "Total Rested Points Remaining: {0}", pm.RestedTraining );
        }

        public static void TrainSkill( PlayerMobile pm, SkillName skill, int points )
        {
            if ( pm.Skills[skill].Base > pm.GetSkillCap( skill ) && pm.SpecialSkills[0] != SkillName.TasteID ) {
                pm.Skills[skill].Base = pm.GetSkillCap( skill );
                return;
            }
            else if ( pm.Skills[skill].Base == pm.GetSkillCap( skill ) ) {
                pm.TrainingPoints.Remove( skill );
                return;
            }
            else if ( !pm.IsSpecced( skill ) && pm.TrainingPoints[skill] > 20 ) {
                pm.TrainingPoints.Remove( skill );
                return;
            }

            if ( pm.Skills[skill].Lock != SkillLock.Up )
                return;

            if ( pm.RestedTraining > 0 && pm.Skills[skill].Base > PlayerMobile.NonSpecCap ) {
                points++;
                pm.RestedTraining--;
            }

            if ( pm.SkillPoints.ContainsKey( skill ) ) {
                pm.SkillPoints[skill] += points;
                pm.TrainingPoints[skill] = pm.TrainingPoints[skill] - points;
            }
            else {
                pm.SkillPoints.Add( skill, points );
                pm.TrainingPoints[skill] = pm.TrainingPoints[skill] - points;
            }

            int gainCost = (int)TrainMaster.GetTrainingTimeTenth( pm.Skills[skill].Base );

            int toRaise = pm.SkillPoints[skill] / gainCost;

            if ( pm.SkillPoints[skill] >= gainCost ) {
                pm.Skills[skill].BaseFixedPoint += toRaise;
                pm.SkillPoints[skill] -= gainCost * toRaise;
            }
        }
    }
}
