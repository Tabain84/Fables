using System;
using System.IO;
using System.Xml;
using Server.Network;
using System.Collections.Generic;
using Server.Gumps;
using Server.Commands;

namespace Server.Engines.XmlSpawner2
{
	public class XmlQuestLeaders
	{
		private static TimeSpan
			m_QuestLeaderboardSaveInterval =
				TimeSpan.FromMinutes(15); // default time interval between quest leaderboard saves

		private static string
			m_QuestLeaderboardSaveDirectory =
				"Leaderboard"; // default directory for saving quest leaderboard xml information

		private static int
			m_QuestLeaderboardSaveRanks =
				20; // number of ranked players to save to the quest leaderboard.  0 means save all players.

		private static QuestLeaderboardTimer m_QuestLeaderboardTimer;
		private static string m_QuestLeaderboardFile;
		private static List<QuestRankEntry> QuestRankList = new List<QuestRankEntry>();
		private static bool needsupdate = true;

		public class QuestRankEntry : IComparable<QuestRankEntry>
		{
			public Mobile Quester;
			public int Rank;
			public XmlQuestPoints QuestPointsAttachment;

			public QuestRankEntry(Mobile m, XmlQuestPoints attachment)
			{
				Quester = m;
				QuestPointsAttachment = attachment;
			}

			public int CompareTo(QuestRankEntry p)
			{
				if (p.QuestPointsAttachment == null || QuestPointsAttachment == null) return 0;

				// break points ties with quests completed (more quests means higher rank)
				if (p.QuestPointsAttachment.Points - QuestPointsAttachment.Points == 0)
				{
					// if kills are the same then compare previous rank
					if (p.QuestPointsAttachment.QuestsCompleted - QuestPointsAttachment.QuestsCompleted == 0)
						return p.QuestPointsAttachment.Rank - QuestPointsAttachment.Rank;

					return p.QuestPointsAttachment.QuestsCompleted - QuestPointsAttachment.QuestsCompleted;
				}

				return p.QuestPointsAttachment.Points - QuestPointsAttachment.Points;
			}
		}

		private static void RefreshQuestRankList()
		{
			if (needsupdate && QuestRankList != null)
			{
				QuestRankList.Sort();

				var rank = 0;
				//int prevpoints = 0;
				for (var i = 0; i < QuestRankList.Count; i++)
				{
					var p = QuestRankList[i];

					// bump the rank for every change in point level
					// this means that people with the same points score will have the same rank
					/*
					if(p.QuestPointsAttachment.Points != prevpoints)
					{
						rank++;
					}

					prevpoints = p.QuestPointsAttachment.Points;
					*/

					// bump the rank for every successive player in the list.  Players with the same points total will be
					// ordered by quests completed
					rank++;

					p.Rank = rank;
				}

				needsupdate = false;
			}
		}

		public static int GetQuestRanking(Mobile m)
		{
			if (QuestRankList == null || m == null) return 0;

			RefreshQuestRankList();

			// go through the sorted list and calculate rank

			for (var i = 0; i < QuestRankList.Count; i++)
			{
				var p = QuestRankList[i];
				// found the person?
				if (p.Quester == m) return p.Rank;
			}

			// rank 0 means unranked
			return 0;
		}

		public static void UpdateQuestRanking(Mobile m, XmlQuestPoints attachment)
		{
			if (QuestRankList == null) QuestRankList = new List<QuestRankEntry>();

			// flag the rank list for updating on the next attempt to retrieve a rank
			needsupdate = true;

			var found = false;

			// rank the entries
			for (var i = 0; i < QuestRankList.Count; i++)
			{
				var p = QuestRankList[i];

				// found a match
				if (p != null && p.Quester == m)
				{
					// update the entry with the new points value

					p.QuestPointsAttachment = attachment;
					found = true;
					break;
				}
			}

			// a new entry so add it
			if (!found) QuestRankList.Add(new QuestRankEntry(m, attachment));
		}

		public static void Initialize()
		{
			CommandSystem.Register("QuestLeaderboardSave", AccessLevel.Administrator,
				new CommandEventHandler(QuestLeaderboardSave_OnCommand));
			CommandSystem.Register("QuestRanking", AccessLevel.Player, new CommandEventHandler(QuestRanking_OnCommand));
		}


		[Usage("QuestRanking")]
		[Description("Displays the top players in quest points")]
		public static void QuestRanking_OnCommand(CommandEventArgs e)
		{
			if (e == null || e.Mobile == null) return;

			// if this player has an XmlQuestPoints attachment, find it
			var p = (XmlQuestPoints)XmlAttach.FindAttachment(e.Mobile, typeof(XmlQuestPoints));

			e.Mobile.CloseGump(typeof(TopQuestPlayersGump));
			e.Mobile.SendGump(new TopQuestPlayersGump(p));
		}

		public static void WriteQuestLeaderboardXml(string filename, int nranks)
		{
			var dirname = Path.Combine(m_QuestLeaderboardSaveDirectory, filename);

			var sw = new StreamWriter(dirname);

			var xf = new XmlTextWriter(sw);

			if (xf == null)
			{
				Console.WriteLine("Error: unable to save XML quest leaderboard to {0}", dirname);
				return;
			}

			xf.Formatting = Formatting.Indented;

			xf.WriteStartDocument(true);

			xf.WriteStartElement("QuestLeaderboard");

			if (nranks > 0)
				xf.WriteAttributeString("nentries", nranks.ToString());
			else
				xf.WriteAttributeString("nentries", QuestRankList.Count.ToString());

			// go through the sorted list and display the top ranked players

			for (var i = 0; i < QuestRankList.Count; i++)
			{
				if (nranks > 0 && i >= nranks) break;

				var r = QuestRankList[i];
				var a = r.QuestPointsAttachment;


				if (r.Quester != null && !r.Quester.Deleted && r.Rank > 0 && a != null && !a.Deleted)
				{
					string guildname = null;

					if (r.Quester.Guild != null)
						guildname = r.Quester.Guild.Abbreviation;

					// check for any ranking change and update rank date
					if (r.Rank != a.Rank)
					{
						a.WhenRanked = DateTime.Now;
						if (a.Rank > 0)
							a.DeltaRank = a.Rank - r.Rank;
						a.Rank = r.Rank;
					}

					var timeranked = DateTime.Now - a.WhenRanked;

					// write out the entry information

					xf.WriteStartElement("Entry");
					xf.WriteAttributeString("number", i.ToString());

					xf.WriteStartElement("Player");
					xf.WriteString(r.Quester.Name);
					xf.WriteEndElement();

					xf.WriteStartElement("Guild");
					xf.WriteString(guildname);
					xf.WriteEndElement();

					xf.WriteStartElement("Points");
					xf.WriteString(a.Points.ToString());
					xf.WriteEndElement();

					var quests = "???";
					try
					{
						quests = a.QuestsCompleted.ToString();
					}
					catch { }

					xf.WriteStartElement("Quests");
					xf.WriteString(quests);
					xf.WriteEndElement();

					xf.WriteStartElement("Rank");
					xf.WriteString(a.Rank.ToString());
					xf.WriteEndElement();

					xf.WriteStartElement("Change");
					xf.WriteString(a.DeltaRank.ToString());
					xf.WriteEndElement();

					xf.WriteStartElement("Duration");
					xf.WriteString(timeranked.ToString());
					xf.WriteEndElement();

					// end the entry
					xf.WriteEndElement();
				}
			}

			xf.WriteEndElement();

			xf.Close();
		}

		public static void WriteQuestLeaderboardHtml(string filename, int nranks)
		{
			var dirname = Path.Combine(m_QuestLeaderboardSaveDirectory, filename);

			var sw = new StreamWriter(dirname);

			if (sw == null)
			{
				Console.WriteLine("Error: unable to save HTML quest leaderboard to {0}", dirname);
				return;
			}

			sw.WriteLine("<TABLE border=\"1\" summary=\"This table gives quest leaderboard stats\"> ");
			sw.WriteLine("<CAPTION><B>Quest Leaderboard</B></CAPTION>");
			sw.WriteLine(
				"<TR><TH><TH>Player Name<TH>Guild<TH>Points<TH>Quests<TH>Rank<TH>Change<TH>Time at current rank");

			// go through the sorted list and display the top ranked players
			for (var i = 0; i < QuestRankList.Count; i++)
			{
				if (nranks > 0 && i >= nranks) break;

				var r = QuestRankList[i];
				var a = r.QuestPointsAttachment;

				if (r.Quester != null && !r.Quester.Deleted && r.Rank > 0 && a != null && !a.Deleted)
				{
					string guildname = null;

					if (r.Quester.Guild != null)
						guildname = r.Quester.Guild.Abbreviation;

					// check for any ranking change and update rank date
					if (r.Rank != a.Rank)
					{
						a.WhenRanked = DateTime.Now;
						if (a.Rank > 0)
							a.DeltaRank = a.Rank - r.Rank;
						a.Rank = r.Rank;
					}

					var timeranked = DateTime.Now - a.WhenRanked;

					var quests = "???";
					try
					{
						quests = a.QuestsCompleted.ToString();
					}
					catch { }

					// write out the entry information
					sw.WriteLine("<TR><TH><TD>{0}<TD>{1}<TD>{2}<TD>{3}<TD>{4}<TD>{5}<TD>{6}",
						r.Quester.Name,
						guildname,
						a.Points,
						quests,
						a.Rank,
						a.DeltaRank,
						timeranked
					);
				}
			}

			sw.WriteLine("</TABLE>");
			sw.Close();
		}


		public static void WriteQuestLeaderboard(string filename, int nranks)
		{
			if (QuestRankList == null) return;

			// force an update of the quest leaderboard rankings
			needsupdate = true;
			RefreshQuestRankList();

			if (!Directory.Exists(m_QuestLeaderboardSaveDirectory))
				Directory.CreateDirectory(m_QuestLeaderboardSaveDirectory);

			WriteQuestLeaderboardXml(filename + ".xml", nranks);

			WriteQuestLeaderboardHtml(filename + ".html", nranks);
		}


		[Usage("QuestLeaderboardSave [filename [minutes[nentries]]][off]")]
		[Description("Periodically save .xml quest leaderboard information to the specified file")]
		public static void QuestLeaderboardSave_OnCommand(CommandEventArgs e)
		{
			if (e.Arguments.Length > 0)
			{
				if (m_QuestLeaderboardTimer != null) m_QuestLeaderboardTimer.Stop();

				if (e.Arguments[0].ToLower() != "off")
				{
					m_QuestLeaderboardFile = e.Arguments[0];

					if (e.Arguments.Length > 1)
						try
						{
							m_QuestLeaderboardSaveInterval = TimeSpan.FromMinutes(Double.Parse(e.Arguments[1]));
						}
						catch { }

					if (e.Arguments.Length > 2)
						try
						{
							m_QuestLeaderboardSaveRanks = Int32.Parse(e.Arguments[2]);
						}
						catch { }


					m_QuestLeaderboardTimer = new QuestLeaderboardTimer(m_QuestLeaderboardFile,
						m_QuestLeaderboardSaveInterval, m_QuestLeaderboardSaveRanks);
					m_QuestLeaderboardTimer.Start();
				}
			}


			if (m_QuestLeaderboardTimer != null && m_QuestLeaderboardTimer.Running)
				e.Mobile.SendMessage("Quest Leaderboard is saving to {0} every {1} minutes. Nranks = {2}",
					m_QuestLeaderboardFile, m_QuestLeaderboardSaveInterval.TotalMinutes, m_QuestLeaderboardSaveRanks);
			else
				e.Mobile.SendMessage("Quest Leaderboard saving is off.");
		}

		public static void QuestLBSSerialize(GenericWriter writer)
		{
			// version
			writer.Write((int)0);

			// version 0
			if (m_QuestLeaderboardTimer != null && m_QuestLeaderboardTimer.Running)
				writer.Write((bool)true);
			else
				writer.Write((bool)false);
			writer.Write(m_QuestLeaderboardSaveInterval);
			writer.Write(m_QuestLeaderboardSaveRanks);
			writer.Write(m_QuestLeaderboardFile);
		}

		public static void QuestLBSDeserialize(GenericReader reader)
		{
			var version = reader.ReadInt();

			switch (version)
			{
				case 0:
					var running = reader.ReadBool();
					m_QuestLeaderboardSaveInterval = reader.ReadTimeSpan();
					m_QuestLeaderboardSaveRanks = reader.ReadInt();
					m_QuestLeaderboardFile = reader.ReadString();

					if (running)
					{
						if (m_QuestLeaderboardTimer != null) m_QuestLeaderboardTimer.Stop();
						m_QuestLeaderboardTimer = new QuestLeaderboardTimer(m_QuestLeaderboardFile,
							m_QuestLeaderboardSaveInterval, m_QuestLeaderboardSaveRanks);
						m_QuestLeaderboardTimer.Start();
					}

					break;
			}
		}

		// added the duration timer that begins on spawning
		private class QuestLeaderboardTimer : Timer
		{
			private string m_filename;
			private int m_nranks;

			public QuestLeaderboardTimer(string filename, TimeSpan delay, int nranks) : base(delay, delay)
			{
				m_filename = filename;
				m_nranks = nranks;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				WriteQuestLeaderboard(m_filename, m_nranks);
			}
		}


		public class TopQuestPlayersGump : Gump
		{
			private XmlQuestPoints m_attachment;

			public TopQuestPlayersGump(XmlQuestPoints attachment) : base(0, 0)
			{
				if (QuestRankList == null) return;

				m_attachment = attachment;

				var numberToDisplay = 20;
				var height = numberToDisplay * 20 + 65;

				// prepare the page
				AddPage(0);

				var width = 740;

				AddBackground(0, 0, width, height, 5054);
				AddAlphaRegion(0, 0, width, height);
				AddImageTiled(20, 20, width - 40, height - 45, 0xBBC);
				AddLabel(20, 2, 55, "Top Quest Player Rankings");

				// guild filter
				AddLabel(40, height - 20, 55, "Filter by Guild");
				string filter = null;
				if (m_attachment != null)
					filter = m_attachment.guildFilter;

				AddImageTiled(140, height - 20, 160, 19, 0xBBC);
				AddTextEntry(140, height - 20, 160, 19, 0, 200, filter);

				AddButton(20, height - 20, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0);

				// name filter
				AddLabel(340, height - 20, 55, "Filter by Name"); // 
				string nfilter = null;
				if (m_attachment != null)
					nfilter = m_attachment.nameFilter;

				AddImageTiled(440, height - 20, 160, 19, 0xBBC);
				AddTextEntry(440, height - 20, 160, 19, 0, 100, nfilter);

				AddButton(320, height - 20, 0x15E1, 0x15E5, 100, GumpButtonType.Reply, 0);

				RefreshQuestRankList();

				var xloc = 23;
				AddLabel(xloc, 20, 0, "Name");
				xloc += 177;
				AddLabel(xloc, 20, 0, "Guild");
				xloc += 50;
				AddLabel(xloc, 20, 0, "Points");
				xloc += 50;
				AddLabel(xloc, 20, 0, "Quests");
				xloc += 50;
				//AddLabel( xloc, 20, 0, "" );
				xloc += 70;
				AddLabel(xloc, 20, 0, "Rank");
				xloc += 45;
				AddLabel(xloc, 20, 0, "Change");
				xloc += 45;
				AddLabel(xloc, 20, 0, "Time at Rank");

				// go through the sorted list and display the top ranked players

				var y = 40;
				var count = 0;
				for (var i = 0; i < QuestRankList.Count; i++)
				{
					if (count >= numberToDisplay) break;

					var r = QuestRankList[i];

					if (r == null) continue;

					var a = r.QuestPointsAttachment;

					if (a == null) continue;

					if (r.Quester != null && !r.Quester.Deleted && r.Rank > 0 && a != null && !a.Deleted)
					{
						string guildname = null;

						if (r.Quester.Guild != null) guildname = r.Quester.Guild.Abbreviation;

						// check for any ranking change and update rank date
						if (r.Rank != a.Rank)
						{
							a.WhenRanked = DateTime.Now;
							if (a.Rank > 0)
								a.DeltaRank = a.Rank - r.Rank;
							a.Rank = r.Rank;
						}

						// check for guild filter
						if (m_attachment != null && m_attachment.guildFilter != null &&
						    m_attachment.guildFilter.Length > 0)
						{
							// parse the comma separated list
							var args = m_attachment.guildFilter.Split(',');
							if (args != null)
							{
								var found = false;
								foreach (var arg in args)
									if (arg != null && guildname == arg.Trim())
									{
										found = true;
										break;
									}

								if (!found) continue;
							}
						}

						// check for name filter
						if (m_attachment != null && m_attachment.nameFilter != null &&
						    m_attachment.nameFilter.Length > 0)
						{
							// parse the comma separated list
							var args = m_attachment.nameFilter.Split(',');

							if (args != null)
							{
								var found = false;
								foreach (var arg in args)
									if (arg != null && r.Quester.Name != null &&
									    r.Quester.Name.ToLower().IndexOf(arg.Trim().ToLower()) >= 0)
									{
										found = true;
										break;
									}

								if (!found) continue;
							}
						}

						count++;

						var timeranked = DateTime.Now - a.WhenRanked;

						var days = (int)timeranked.TotalDays;
						var hours = (int)(timeranked.TotalHours - days * 24);
						var mins = (int)(timeranked.TotalMinutes - (int)timeranked.TotalHours * 60);

						var quests = "???";
						try
						{
							quests = a.QuestsCompleted.ToString();
						}
						catch { }

						xloc = 23;
						AddLabel(xloc, y, 0, r.Quester.Name);
						xloc += 177;
						AddLabel(xloc, y, 0, guildname);
						xloc += 50;
						AddLabel(xloc, y, 0, a.Points.ToString());
						xloc += 50;
						AddLabel(xloc, y, 0, quests);
						xloc += 50;
						//AddLabel( xloc, y, 0, "" );
						xloc += 70;
						AddLabel(xloc, y, 0, a.Rank.ToString());

						string label = null;

						if (days > 0)
							label += String.Format("{0} days ", days);
						if (hours > 0)
							label += String.Format("{0} hours ", hours);
						if (mins > 0)
							label += String.Format("{0} mins", mins);

						if (label == null) label = "just changed";

						var deltalabel = a.DeltaRank.ToString();
						var deltahue = 0;
						if (a.DeltaRank > 0)
						{
							deltalabel = String.Format("+{0}", a.DeltaRank);
							deltahue = 68;
						}
						else if (a.DeltaRank < 0) deltahue = 33;

						xloc += 50;
						AddLabel(xloc, y, deltahue, deltalabel);
						xloc += 40;
						AddLabel(xloc, y, 0, label);

						y += 20;
					}
				}
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				if (state == null || state.Mobile == null || info == null) return;
				// Get the current name
				if (m_attachment != null)
				{
					var entry = info.GetTextEntry(200);
					if (entry != null)
						m_attachment.guildFilter = entry.Text;

					entry = info.GetTextEntry(100);
					if (entry != null)
						m_attachment.nameFilter = entry.Text;
				}

				switch (info.ButtonID)
				{
					case 100:
					case 200:
					{
						// redisplay the gump
						state.Mobile.SendGump(new TopQuestPlayersGump(m_attachment));
						break;
					}
				}
			}
		}
	}
}
