using System;
using Server.Mobiles;

namespace Server.Training
{
	public class TrainMaster
	{
		public const double ExpCostModifier = 0.6;
		public const double TrainTimeModifier = 0.5;

		public static double GetExpCostTenth( double currentValue )
		{
			//training costs increase each 10 points
			int skillMagnitude = (int)( currentValue / 10 );
			switch ( skillMagnitude ) {
				case 0:
					return ( 1 * ExpCostModifier );
				case 1:
					return ( 2 * ExpCostModifier );
				case 2:
					return ( 4 * ExpCostModifier );
				case 3:
					return ( 8 * ExpCostModifier );
				case 4:
					return ( 16 * ExpCostModifier );
				case 5:
					return ( 32 * ExpCostModifier );
				case 6:
					return ( 64 * ExpCostModifier );
				case 7:
					return ( 128 * ExpCostModifier );
				case 8:
					return ( 256 * ExpCostModifier );
				case 9:
					return ( 512 * ExpCostModifier );
				case 10:
					return ( 1024 * ExpCostModifier );
				default:
					return ( 2048 * ExpCostModifier );
			}
		}

		public static double GetTrainingTimeTenth( double currentValue )
		{
			//training costs increase each 10 points
			int skillMagnitude = (int)( currentValue / 10 );
			switch ( skillMagnitude ) {
				case 0:
					return ( 2 * TrainTimeModifier );
				case 1:
					return ( 8 * TrainTimeModifier );
				case 2:
					return ( 16 * TrainTimeModifier );
				case 3:
					return ( 32 * TrainTimeModifier );
				case 4:
					return ( 64 * TrainTimeModifier );
				case 5:
					return ( 128 * TrainTimeModifier );
				case 6:
					return ( 256 * TrainTimeModifier );
				case 7:
					return ( 512 * TrainTimeModifier );
				case 8:
					return ( 1024 * TrainTimeModifier );
				case 9:
					return ( 2048 * TrainTimeModifier );
				case 10:
					return ( 4096 * TrainTimeModifier );
				default:
					return ( 8192 * TrainTimeModifier );
			}
		}
		public static int GetTrainingTime( PlayerMobile pm, SkillName theSkill, double amount )
		{
			double currentSkillValue = pm.Skills[theSkill].Base;

			int trainingCost = 0;
			int loopAmount = (int)(amount * 10);

			for ( int i = 0; i < loopAmount; i++ ) {
				trainingCost += (int)GetTrainingTimeTenth( currentSkillValue + ( 0.1 * i ) );
			}

			return trainingCost;

		}

		public static double GetCurrentTraining( PlayerMobile pm, SkillName theSkill )
		{
			double increase = 0.0;
			int points = pm.TrainingPoints[theSkill];

			int i = 0;

			while ( points >= GetTrainingTimeTenth( pm.Skills[theSkill].Base ) ) {
				increase += .1;
				points -= (int)GetTrainingTimeTenth( pm.Skills[theSkill].Base + ( 0.1 * i ) );
				i += 1;
			}

			return increase;
		}

		public static int GetExpCost( PlayerMobile pm, SkillName theSkill, double amount )
		{
			double currentSkillValue = pm.Skills[theSkill].Base;

            if ( amount < 0.1 && amount != 0 )
                amount = 0.1;

			int trainingCost = 0;
            int loopAmount = (int)(amount * 10);
            
			for ( int i = 0; i < loopAmount; i++ ) {
				trainingCost += (int)GetExpCostTenth( currentSkillValue + ( 0.1 * i ) );
			}

			return trainingCost;

		}

        public static double GetTrainingIncrease(PlayerMobile pm, SkillName theSkill, int points)
        {
            double increase = 0.0;
            double decrease = (double)points;

            int i = 0;

            while (decrease >= GetExpCostTenth(pm.Skills[theSkill].Base))
            {
                increase += .1;
                decrease -= GetExpCostTenth(pm.Skills[theSkill].Base + (0.1 * i));
                i += 1;
            }

            return increase;
        }

        public static string GetTrainingTimeString( PlayerMobile pm, SkillName skill )
		{
			int trainingPoints = pm.TrainingPoints[skill];
			int modifier = 5;

			if ( SkillMaster.IsEasyskill( skill ) )
				modifier = 6;
			else if ( SkillMaster.IsHardskill( skill ) )
				modifier = 4;

			TimeSpan time = TimeSpan.FromSeconds( trainingPoints * 5 / modifier );

			string timeString = "";
			if ( time < TimeSpan.FromMinutes( 1 ) )
				timeString = "< 1 minute";
			else {
				if ( time.Days >= 1 ) {
					if ( time.Days == 1 )
						timeString += time.Days + " day, ";
					else
						timeString += time.Days + " days, ";
				}
				if ( time.Hours >= 1 ) {
					if ( time.Hours == 1 )
						timeString += time.Hours + " hour, ";
					else
						timeString += time.Hours + " hours, ";
				}
				if ( time.Minutes >= 1 ) {
					if ( time.Minutes == 1 )
						timeString += time.Minutes + " minute";
					else
						timeString += time.Minutes + " minutes";
				}
			}

			return timeString;
		}

	}
}

