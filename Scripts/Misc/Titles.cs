using Server.Accounting;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Server.Misc
{
    public class Titles
    {
        public const int MinFame = 0;
        public const int MaxFame = 100000;

        public static void AwardFame(Mobile m, int offset, bool message)
        {
            int fame = m.Fame;
            offset = Math.Abs(offset);

            if (offset > 0)
            {
                if (fame >= MaxFame)
                    return;

                offset -= fame / 500;

                if (offset < 0)
                    offset = 0;
            }

            int addFame =0;

            if (message)
            {
                if (offset > 300)
                {
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a massive amount of fame!");
                    addFame += 8;
                }
                if (offset > 250)
                {
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a vast amount of fame!");
                    addFame += 7;
                }
                if (offset > 200)
                {
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a great amount of fame!");
                    addFame += 6;
                }
                else if (offset > 150)
                {
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a large amount of fame!");
                    addFame += 5;
                }
                else if (offset > 100)
                {
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a good amount of fame!");
                    addFame += 4;
                }
                else if (offset > 50)
                {
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained some fame!");
                    addFame += 3;
                }
                else if (offset > 25)
                {
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a little fame!");
                    addFame += 2;
                }
                else if (offset > 5)
                {
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a tiny amount of fame!");
                    addFame += 1;
                }
            }

            if ((fame + addFame) > MaxFame)
                addFame = MaxFame - fame;

            m.Fame += addFame * Settings.fameGainModifier;
        }

        public const int MinKarma = -32000;
        public const int MaxKarma = 32000;

        public static void AwardKarma(Mobile m, int offset, bool message)
        {
            int karma = m.Karma;

            if (m.Talisman is BaseTalisman)
            {
                BaseTalisman talisman = (BaseTalisman)m.Talisman;

                if (talisman.KarmaLoss > 0)
                    offset *= (1 + (int)(((double)talisman.KarmaLoss) / 100));
                else if (talisman.KarmaLoss < 0)
                    offset *= (1 - (int)(((double)-talisman.KarmaLoss) / 100));
            }

            int karmaLoss = AosAttributes.GetValue(m, AosAttribute.IncreasedKarmaLoss);

            if (karmaLoss != 0 && offset < 0)
            {
                offset -= (int)(offset * (karmaLoss / 100.0));
            }

            if (offset > 0)
            {
                if (m is PlayerMobile && ((PlayerMobile)m).KarmaLocked)
                    return;

                if (karma >= MaxKarma)
                    return;

                offset -= karma / 100;

                if (offset < 0)
                    offset = 0;
            }
            else if (offset < 0)
            {
                if (karma <= MinKarma)
                    return;

                offset -= karma / 100;

                if (offset > 0)
                    offset = 0;
            }

            if ((karma + offset) > MaxKarma)
                offset = MaxKarma - karma;
            else if ((karma + offset) < MinKarma)
                offset = MinKarma - karma;

            m.Karma += offset;

            if (message)
            {
                if (offset > 40)
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a lot of karma.");
                else if (offset > 20)
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a good amount of karma."); // You have gained a good amount of karma.
                else if (offset > 10)
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained some karma."); // You have gained some karma.
                else if (offset > 0)
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have gained a little karma."); // You have gained a little karma.
                else if (offset < -40)
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have have lost a lot of karma."); // You have lost a lot of karma.
                else if (offset < -20)
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have lost a good amount of karma."); // You have lost a good amount of karma.
                else if (offset < -10)
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have lost some karma."); // You have lost some karma.
                else if (offset < 0)
                    m.SendMessage(MessageUtil.MessageColorPlayer, "You have lost a little karma."); // You have lost a little karma.
            }
        }

        public static List<string> GetFameKarmaEntries(Mobile m)
        {
            List<string> list = new List<string>();
            int fame = m.Fame;
            int karma = m.Karma;

            for (int i = 0; i < m_FameEntries.Length; ++i)
            {
                FameEntry fe = m_FameEntries[i];

                if (fame >= fe.m_Fame)
                {
                    KarmaEntry[] karmaEntries = fe.m_Karma;

                    for (int j = 0; j < karmaEntries.Length; ++j)
                    {
                        KarmaEntry ke = karmaEntries[j];
                        StringBuilder title = new StringBuilder();

                        if ((karma >= 0 && ke.m_Karma >= 0 && karma >= ke.m_Karma) || (karma < 0 && ke.m_Karma < 0 && karma < ke.m_Karma))
                        {
                            list.Add(title.AppendFormat(ke.m_Title, m.Name, m.Female ? "Lady" : "Lord").ToString());
                        }
                    }
                }
            }

            return list;
        }

        public static string[] HarrowerTitles = new string[] { "Spite", "Opponent", "Hunter", "Venom", "Executioner", "Annihilator", "Champion", "Assailant", "Purifier", "Nullifier" };

        public static string ComputeFameTitle(Mobile beheld)
        {
            int fame = beheld.Fame;
            int karma = beheld.Karma;

			string[] titles;
            StringBuilder title = new StringBuilder();

            if ( beheld is PlayerMobile ) {
				PlayerMobile bh = beheld as PlayerMobile;
				if ( bh != null ) {

					if ( bh.Karma < 0)
						titles = NegativeLevels;
					else
						titles = PositiveLevels;

                    if (fame >= 50000)
                        title.AppendFormat(titles[11], beheld.Female ? "Lady" : "Lord", beheld.Name);
                    else if (fame > 40000)
                        title.AppendFormat(titles[10], beheld.Name);
                    else if (fame > 30000)
                        title.AppendFormat(titles[9], beheld.Name);
                    else if (fame > 20000)
                        title.AppendFormat(titles[8], beheld.Name);
                    else if (fame > 15000)
                        title.AppendFormat(titles[7], beheld.Name);
                    else if (fame > 10000)
                        title.AppendFormat(titles[6], beheld.Name);
                    else if (fame > 5000)
                        title.AppendFormat(titles[5], beheld.Name);
                    else if (fame > 3200)
                        title.AppendFormat(titles[4], beheld.Name);
                    else if (fame > 1600)
                        title.AppendFormat(titles[3], beheld.Name);
                    else if (fame > 800)
                        title.AppendFormat(titles[2], beheld.Name);
                    else if (fame > 400)
                        title.AppendFormat(titles[1], beheld.Name);
                    else if (fame > 200)
                        title.AppendFormat(titles[0], beheld.Name);
                    else
                        title.AppendFormat(beheld.Name);
				}	
			}
            return title.ToString();

            /*for (int i = 0; i < m_FameEntries.Length; ++i)
            {
                FameEntry fe = m_FameEntries[i];

                if (fame <= fe.m_Fame || i == (m_FameEntries.Length - 1))
                {
                    KarmaEntry[] karmaEntries = fe.m_Karma;

                    for (int j = 0; j < karmaEntries.Length; ++j)
                    {
                        KarmaEntry ke = karmaEntries[j];

                        if (karma <= ke.m_Karma || j == (karmaEntries.Length - 1))
                        {
                            return string.Format(ke.m_Title, beheld.Name, beheld.Female ? "Lady" : "Lord");
                        }
                    }

                    return string.Empty;
                }
            }
            return string.Empty;*/
        }

        public static string ComputeTitle(Mobile beholder, Mobile beheld)
        {
            StringBuilder title = new StringBuilder();

            if (beheld.ShowFameTitle && beheld is PlayerMobile && ((PlayerMobile)beheld).FameKarmaTitle != null)
            {
                title.AppendFormat(((PlayerMobile)beheld).FameKarmaTitle, beheld.Name, beheld.Female ? "Lady" : "Lord");
            }
            else if (beheld.ShowFameTitle || (beholder == beheld))
            {
                title.Append(ComputeFameTitle(beheld));
            }
            else
            {
                title.Append(beheld.Name);
            }

            if (beheld is PlayerMobile && (((PlayerMobile)beheld).CurrentChampTitle != null) && ((PlayerMobile)beheld).DisplayChampionTitle)
            {
                title.AppendFormat(((PlayerMobile)beheld).CurrentChampTitle);
            }

            string customTitle = beheld.Title;

            if (beheld is PlayerMobile && ((PlayerMobile)beheld).PaperdollSkillTitle != null)
                title.Append(", ").Append(((PlayerMobile)beheld).PaperdollSkillTitle);
            else if (beheld is BaseVendor)
                title.AppendFormat(" {0}", customTitle);

            return title.ToString();
        }

        public static string GetSkillTitle(Mobile mob)
        {
            Skill highest = GetHighestSkill(mob);// beheld.Skills.Highest;

            if (highest != null && highest.BaseFixedPoint >= 300)
            {
                string skillLevel = GetSkillLevel(highest);
                string skillTitle = highest.Info.Title;

                if (mob.Female && skillTitle.EndsWith("man"))
                    skillTitle = skillTitle.Substring(0, skillTitle.Length - 3) + "woman";

                return string.Concat(skillLevel, " ", skillTitle);
            }

            return null;
        }

        public static string GetSkillTitle(Mobile mob, Skill skill)
        {
            if (skill != null && skill.BaseFixedPoint >= 300)
            {
                string skillLevel = GetSkillLevel(skill);
                string skillTitle = skill.Info.Title;

                if (mob.Female && skillTitle.EndsWith("man"))
                    skillTitle = skillTitle.Substring(0, skillTitle.Length - 3) + "woman";

                return string.Concat(skillLevel, " ", skillTitle);
            }

            return null;
        }

        private static Skill GetHighestSkill(Mobile m)
        {
            Skill highest = null;

            for (int i = 0; i < m.Skills.Length; ++i)
            {
                Skill check = m.Skills[i];

                if (highest == null || check.BaseFixedPoint > highest.BaseFixedPoint)
                    highest = check;
                else if (highest.Lock != SkillLock.Up && check.Lock == SkillLock.Up && check.BaseFixedPoint == highest.BaseFixedPoint)
                    highest = check;
            }

            return highest;
        }
		public static string[] PositiveLevels
		{
			get
			{
				string[] PosLevelArray = 
				{
					"The Recognized {0}",
					"The Noted {0}",
					"The Honorable {0}",
					"The Influential {0}",
                    "The Admirable {0}",
					"The Famed {0}",
					"The Extraordinary {0}",
					"The Acclaimed {0}",
                    "The Great {0}",
					"The Exalted {0}",
					"The Illustrious {0}",
					"The Glorious {0} {1}",
					};

				return PosLevelArray;
			}
		}

		public static string[] NegativeLevels
		{
			get
			{
				string[] NegLevelArray = 
				{
					"The Questionable {0}",
					"The Contemptible {0}",
					"The Despicable {0}",
                    "The Disreputable {0}",
					"The Outcast {0}",
                    "The Infamous {0}",
					"The Wicked {0}",
					"The Atrocious {0}",
					"The Wretched {0}",
					"The Vicious {0}",
					"The Vile {0}",
					"The Dread {0} {1}",
					};

				return NegLevelArray;
			}
		}
        private static readonly string[,] m_Levels = new string[,]
        {
            { "Neophyte", "Neophyte", "Neophyte" },
            { "Novice", "Novice", "Novice" },
            { "Apprentice", "Apprentice", "Apprentice" },
            { "Journeyman", "Journeyman", "Journeyman" },
            { "Expert", "Expert", "Expert" },
            { "Adept", "Adept", "Adept" },
            { "Master", "Master", "Master" },
            { "Grandmaster", "Grandmaster", "Grandmaster" },
            { "Elder", "Tatsujin", "Shinobi" },
            { "Legendary", "Kengo", "Ka-ge" }
        };

        private static string GetSkillLevel(Skill skill)
        {
            return m_Levels[GetTableIndex(skill), GetTableType(skill)];
        }

        private static int GetTableType(Skill skill)
        {
            switch (skill.SkillName)
            {
                default:
                    return 0;
                case SkillName.Bushido:
                    return 1;
                case SkillName.Ninjitsu:
                    return 2;
            }
        }

        private static int GetTableIndex(Skill skill)
        {
            int fp = skill == null ? 300 : skill.BaseFixedPoint;

            fp = Math.Min(fp, 1200);

            return (fp - 300) / 100;
        }

        private static readonly FameEntry[] m_FameEntries = new FameEntry[]
        {
            new FameEntry(1249, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Outcast {0}"),
                new KarmaEntry(-5000, "The Despicable {0}"),
                new KarmaEntry(-2500, "The Scoundrel {0}"),
                new KarmaEntry(-1250, "The Unsavory {0}"),
                new KarmaEntry(-625, "The Rude {0}"),
                new KarmaEntry(624, "{0}"),
                new KarmaEntry(1249, "The Fair {0}"),
                new KarmaEntry(2499, "The Kind {0}"),
                new KarmaEntry(4999, "The Good {0}"),
                new KarmaEntry(9999, "The Honest {0}"),
                new KarmaEntry(10000, "The Trustworthy {0}")
            }),
            new FameEntry(2499, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Wretched {0}"),
                new KarmaEntry(-5000, "The Dastardly {0}"),
                new KarmaEntry(-2500, "The Malicious {0}"),
                new KarmaEntry(-1250, "The Dishonorable {0}"),
                new KarmaEntry(-625, "The Disreputable {0}"),
                new KarmaEntry(624, "The Notable {0}"),
                new KarmaEntry(1249, "The Upstanding {0}"),
                new KarmaEntry(2499, "The Respectable {0}"),
                new KarmaEntry(4999, "The Honorable {0}"),
                new KarmaEntry(9999, "The Commendable {0}"),
                new KarmaEntry(10000, "The Estimable {0}")
            }),
            new FameEntry(4999, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Nefarious {0}"),
                new KarmaEntry(-5000, "The Wicked {0}"),
                new KarmaEntry(-2500, "The Vile {0}"),
                new KarmaEntry(-1250, "The Ignoble {0}"),
                new KarmaEntry(-625, "The Notorious {0}"),
                new KarmaEntry(624, "The Prominent {0}"),
                new KarmaEntry(1249, "The Reputable {0}"),
                new KarmaEntry(2499, "The Proper {0}"),
                new KarmaEntry(4999, "The Admirable {0}"),
                new KarmaEntry(9999, "The Famed {0}"),
                new KarmaEntry(10000, "The Great {0}")
            }),
            new FameEntry(9999, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Dread {0}"),
                new KarmaEntry(-5000, "The Evil {0}"),
                new KarmaEntry(-2500, "The Villainous {0}"),
                new KarmaEntry(-1250, "The Sinister {0}"),
                new KarmaEntry(-625, "The Infamous {0}"),
                new KarmaEntry(624, "The Renowned {0}"),
                new KarmaEntry(1249, "The Distinguished {0}"),
                new KarmaEntry(2499, "The Eminent {0}"),
                new KarmaEntry(4999, "The Noble {0}"),
                new KarmaEntry(9999, "The Illustrious {0}"),
                new KarmaEntry(10000, "The Glorious {0}")
            }),
            new FameEntry(10000, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Dread {1} {0}"),
                new KarmaEntry(-5000, "The Evil {1} {0}"),
                new KarmaEntry(-2500, "The Dark {1} {0}"),
                new KarmaEntry(-1250, "The Sinister {1} {0}"),
                new KarmaEntry(-625, "The Dishonored {1} {0}"),
                new KarmaEntry(624, "{1} {0}"),
                new KarmaEntry(1249, "The Distinguished {1} {0}"),
                new KarmaEntry(2499, "The Eminent {1} {0}"),
                new KarmaEntry(4999, "The Noble {1} {0}"),
                new KarmaEntry(9999, "The Illustrious {1} {0}"),
                new KarmaEntry(10000, "The Glorious {1} {0}")
            })
        };

        public static VeteranTitle[] VeteranTitles { get; set; }

        public static void Initialize()
        {
            VeteranTitles = new VeteranTitle[9];

            for (int i = 0; i < 9; i++)
            {
                VeteranTitles[i] = new VeteranTitle(1154341 + i, 2 * (i + 1));
            }
        }

        public static List<VeteranTitle> GetVeteranTitles(Mobile m)
        {
            Account a = m.Account as Account;

            if (a == null)
                return null;

            int years = (int)(DateTime.UtcNow - a.Created).TotalDays;
            years /= 365;

            if (years < 2)
                return null;

            List<VeteranTitle> titles = new List<VeteranTitle>();

            foreach (VeteranTitle title in VeteranTitles)
            {
                if (years >= title.Years)
                    titles.Add(title);
            }

            return titles;
        }
    }

    public class FameEntry
    {
        public int m_Fame;
        public KarmaEntry[] m_Karma;

        public FameEntry(int fame, KarmaEntry[] karma)
        {
            m_Fame = fame;
            m_Karma = karma;
        }
    }

    public class KarmaEntry
    {
        public int m_Karma;
        public string m_Title;

        public KarmaEntry(int karma, string title)
        {
            m_Karma = karma;
            m_Title = title;
        }
    }

    public class VeteranTitle
    {
        public int Title { get; set; }
        public int Years { get; set; }

        public VeteranTitle(int title, int years)
        {
            Title = title;
            Years = years;
        }
    }
}
