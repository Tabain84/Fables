using System;
using System.Collections;
using Server;
using Server.Mobiles;

namespace Server
{
	public class SkillMaster
	{

		public static SkillName[] AllSkills
		{
			get
			{
				SkillName[] skillArray = {
					SkillName.Anatomy,
					SkillName.AnimalLore,
					SkillName.AnimalTaming,
					SkillName.Archery,
                    //SkillName.Camping,
                    //SkillName.Chivalry,
					SkillName.DetectHidden,
					SkillName.Fencing,
					SkillName.Healing,
                    //SkillName.Herding, //druidism
					SkillName.Hiding,
					//SkillName.Invocation,
					//SkillName.Lockpicking,
					SkillName.Macing,
					SkillName.Magery,
					SkillName.MagicResist,
					SkillName.Meditation,
					SkillName.Musicianship,
                    //SkillName.Necromancy,
					SkillName.Parry,
					SkillName.Peacemaking,
					SkillName.Discordance,
					SkillName.Poisoning,
					SkillName.Provocation,
                    SkillName.SpiritSpeak,
                    //SkillName.Stealing,
					//SkillName.Stealth,
					SkillName.Swords,
					SkillName.Tactics,
                    SkillName.Veterinary,
					SkillName.Wrestling,
					//SkillName.TreasureHunting,
					SkillName.Focus,
					//SkillName.Survival,
					//SkillName.RemoveTrap,
					//SkillName.MentalAcuity,

					SkillName.Alchemy,
                    //SkillName.Enchanting,
					SkillName.Blacksmith,
					SkillName.Carpentry,
					SkillName.Cooking,
					SkillName.Fishing,
                    //SkillName.WoodWorking,
                    //SkillName.Butchering, // not used
					//SkillName.Artistry,
                    //SkillName.ItemID,
					SkillName.Lumberjacking,
					SkillName.Mining,
					SkillName.Tailoring,
					SkillName.Tinkering,
					//SkillName.Farming,
				};
				return skillArray;
			}
		}

		public static bool IsValidSkill( SkillName theSkill )
		{
			foreach ( SkillName thisSkill in AllSkills ) {
				if ( thisSkill == theSkill )
					return true;
			}
			return false;
		}

		public static SkillName[] CombatSkills
		{
			get
			{
				SkillName[] skillArray = {
					SkillName.Anatomy,
					SkillName.AnimalLore,
					SkillName.AnimalTaming,
					SkillName.Archery,
                    //SkillName.Camping,
                    //SkillName.Chivalry,
					SkillName.DetectHidden,
					SkillName.Fencing,
					SkillName.Healing,
                    //SkillName.Herding,
					SkillName.Hiding,
					//SkillName.Invocation,
					//SkillName.Lockpicking,
					SkillName.Macing,
					SkillName.Magery,
					SkillName.MagicResist,
					SkillName.Meditation,
					SkillName.Musicianship,
                    //SkillName.Necromancy,
					SkillName.Parry,
					SkillName.Peacemaking,
					SkillName.Discordance,
					SkillName.Poisoning,
					SkillName.Provocation,
                    SkillName.SpiritSpeak,
                   // SkillName.Stealing,
					//SkillName.Stealth,
					SkillName.Swords,
					SkillName.Tactics,
                    SkillName.Veterinary,
					SkillName.Wrestling,
					//SkillName.TreasureHunting,
					SkillName.Focus,
					//SkillName.Survival,
					//SkillName.RemoveTrap,
					//SkillName.MentalAcuity,
				};
				return skillArray;
			}
		}

		public static SkillName[] TradeSkills
		{
			get
			{
				SkillName[] skillArray = {
					SkillName.Alchemy,
                    //SkillName.Enchanting,
					SkillName.Blacksmith,
					SkillName.Cooking,
					SkillName.Fishing,
                    //SkillName.Butchering,
					//SkillName.Artistry,
                    //SkillName.ItemID,
					SkillName.Lumberjacking,
					SkillName.Mining,
					SkillName.Tailoring,
					SkillName.Tinkering,
					SkillName.Carpentry,
					//SkillName.Farming,
					//SkillName.WoodWorking,

				};

				return skillArray;
			}
		}

		public static bool IsTradeskill( SkillName theSkill )
		{
			foreach ( SkillName thisSkill in TradeSkills ) {
				if ( thisSkill == theSkill )
					return true;
			}
			return false;
		}

		public static SkillName[] EasySkills
		{
			get
			{
				SkillName[] skillArray = {
					SkillName.Anatomy,
                    //SkillName.Camping,
					SkillName.DetectHidden,
					//SkillName.Lockpicking,
					SkillName.Wrestling,
					//SkillName.TreasureHunting,
					//SkillName.Tracking,
					//SkillName.RemoveTrap,
					SkillName.Meditation
				};

				return skillArray;
			}
		}

		public static SkillName[] HardSkills
		{
			get
			{
				SkillName[] skillArray = {
					SkillName.AnimalTaming,
					SkillName.Archery,
					SkillName.Fencing,
					SkillName.Macing,
					SkillName.Magery,
					SkillName.Poisoning,
					SkillName.Provocation,
					SkillName.Swords,
					//SkillName.TreasureHunting,
                    //SkillName.Survival,

				};
				return skillArray;
			}
		}

		public static bool IsEasyskill( SkillName theSkill )
		{
			foreach ( SkillName thisSkill in EasySkills ) {
				if ( thisSkill == theSkill )
					return true;
			}
			return false;
		}

		public static bool IsHardskill( SkillName theSkill )
		{
			foreach ( SkillName thisSkill in HardSkills ) {
				if ( thisSkill == theSkill )
					return true;
			}
			return false;
		}

		public static bool IsNormalskill( SkillName theSkill )
		{
			if ( IsEasyskill( theSkill ) )
				return false;
			else if ( IsHardskill( theSkill ) )
				return false;

			return true;
		}
	}
}