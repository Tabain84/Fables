using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Server.Mobiles;

namespace Server.Gumps
{
	public class XmlPartialCategorizedAddGump : Gump
	{
		private readonly string m_SearchString;
		private readonly ArrayList m_SearchResults;
		private readonly int m_Page;
		private readonly Gump m_Gump;
		private readonly int m_EntryIndex;
		private readonly XmlSpawner m_Spawner;

		public XmlPartialCategorizedAddGump(Mobile from, string searchString, int page, ArrayList searchResults, bool explicitSearch, int entryindex, Gump gump) : base(50, 50)
		{
			if (gump is XmlSpawnerGump spawnerGump)
			{
				// keep track of the spawner for xmlspawnergumps
				m_Spawner = spawnerGump.m_Spawner;
			}

			// keep track of the gump
			m_Gump = gump;


			m_SearchString = searchString;
			m_SearchResults = searchResults;
			m_Page = page;

			m_EntryIndex = entryindex;

			_ = from.CloseGump(typeof(XmlPartialCategorizedAddGump));

			AddPage(0);

			AddBackground(0, 0, 420, 280, 5054);

			AddImageTiled(10, 10, 400, 20, 2624);
			AddAlphaRegion(10, 10, 400, 20);
			AddImageTiled(41, 11, 184, 18, 0xBBC);
			AddImageTiled(42, 12, 182, 16, 2624);
			AddAlphaRegion(42, 12, 182, 16);

			AddButton(10, 9, 4011, 4013, 1, GumpButtonType.Reply, 0);
			AddTextEntry(44, 10, 180, 20, 0x480, 0, searchString);

			AddHtmlLocalized(230, 10, 100, 20, 3010005, 0x7FFF, false, false);

			AddImageTiled(10, 40, 400, 200, 2624);
			AddAlphaRegion(10, 40, 400, 200);

			if (searchResults.Count > 0)
			{
				for (var i = page * 10; i < (page + 1) * 10 && i < searchResults.Count; ++i)
				{
					var index = i % 10;

					var se = (SearchEntry)searchResults[i];

					var labelstr = se.EntryType.Name;

					if (se.Parameters.Length > 0)
					{
						for (var j = 0; j < se.Parameters.Length; j++)
						{
							labelstr += ", " + se.Parameters[j].Name;
						}
					}

					AddLabel(44, 39 + (index * 20), 0x480, labelstr);
					AddButton(10, 39 + (index * 20), 4023, 4025, 4 + i, GumpButtonType.Reply, 0);
				}
			}
			else
			{
				AddLabel(15, 44, 0x480, explicitSearch ? "Nothing matched your search terms." : "No results to display.");
			}

			AddImageTiled(10, 250, 400, 20, 2624);
			AddAlphaRegion(10, 250, 400, 20);

			if (m_Page > 0)
			{
				AddButton(10, 249, 4014, 4016, 2, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(10, 249, 4014);
			}

			AddHtmlLocalized(44, 250, 170, 20, 1061028, m_Page > 0 ? 0x7FFF : 0x5EF7, false, false); // Previous page

			if ((m_Page + 1) * 10 < searchResults.Count)
			{
				AddButton(210, 249, 4005, 4007, 3, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(210, 249, 4005);
			}

			AddHtmlLocalized(244, 250, 170, 20, 1061027, (m_Page + 1) * 10 < searchResults.Count ? 0x7FFF : 0x5EF7, false, false); // Next page
		}

		private static readonly Type typeofItem = typeof(Item), typeofMobile = typeof(Mobile);

		private class SearchEntry
		{
			public Type EntryType;
			public ParameterInfo[] Parameters;
		}
		private static void Match(string match, IReadOnlyList<Type> types, IList results)
		{
			if (match.Length == 0)
			{
				return;
			}

			match = match.ToLower();

			for (var i = 0; i < types.Count; ++i)
			{
				var t = types[i];

				if ((typeofMobile.IsAssignableFrom(t) || typeofItem.IsAssignableFrom(t)) && t.Name.ToLower().IndexOf(match) >= 0 && !results.Contains(t))
				{
					var ctors = t.GetConstructors();

					for (var j = 0; j < ctors.Length; ++j)
					{
						if ( /*ctors[j].GetParameters().Length == 0 && */ ctors[j].IsDefined(typeof(ConstructableAttribute), false))
						{
							var s = new SearchEntry
							{
								EntryType = t,
								Parameters = ctors[j].GetParameters()
							};
							//results.Add( t );
							_ = results.Add(s);
							//break;
						}
					}
				}
			}
		}

		public static ArrayList Match(string match)
		{
			var results = new ArrayList();
			Type[] types;

			var asms = ScriptCompiler.Assemblies;

			for (var i = 0; i < asms.Length; ++i)
			{
				types = ScriptCompiler.GetTypeCache(asms[i]).Types;
				Match(match, types, results);
			}

			types = ScriptCompiler.GetTypeCache(Core.Assembly).Types;
			Match(match, types, results);

			results.Sort(new TypeNameComparer());

			return results;
		}

		private class TypeNameComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				var a = x as SearchEntry;
				var b = y as SearchEntry;

				return a.EntryType.Name.CompareTo(b.EntryType.Name);
			}
		}


		public override void OnResponse(Network.NetState sender, RelayInfo info)
		{
			var from = sender.Mobile;

			switch (info.ButtonID)
			{
				case 1: // Search
				{
					var te = info.GetTextEntry(0);
					var match = te == null ? "" : te.Text.Trim();

					if (match.Length < 3)
					{
						from.SendMessage("Invalid search string.");
						_ = from.SendGump(new XmlPartialCategorizedAddGump(from, match, m_Page, m_SearchResults, false, m_EntryIndex, m_Gump));
					}
					else
					{
						_ = from.SendGump(new XmlPartialCategorizedAddGump(from, match, 0, Match(match), true, m_EntryIndex, m_Gump));
					}

					break;
				}
				case 2: // Previous page
				{
					if (m_Page > 0)
					{
						_ = from.SendGump(new XmlPartialCategorizedAddGump(from, m_SearchString, m_Page - 1, m_SearchResults, true, m_EntryIndex, m_Gump));
					}

					break;
				}
				case 3: // Next page
				{
					if ((m_Page + 1) * 10 < m_SearchResults.Count)
					{
						_ = from.SendGump(new XmlPartialCategorizedAddGump(from, m_SearchString, m_Page + 1, m_SearchResults, true, m_EntryIndex, m_Gump));
					}

					break;
				}
				default:
				{
					var index = info.ButtonID - 4;

					if (index >= 0 && index < m_SearchResults.Count)
					{
						var type = ((SearchEntry)m_SearchResults[index]).EntryType;

						if (m_Gump is XmlAddGump mXmlAddGump && type != null)
						{
							if (mXmlAddGump.defs?.NameList != null && m_EntryIndex >= 0 && m_EntryIndex < mXmlAddGump.defs.NameList.Length)
							{
								mXmlAddGump.defs.NameList[m_EntryIndex] = type.Name;
								XmlAddGump.Refresh(from, true);
							}
						}
						else if (m_Spawner != null && type != null)
						{
							var xg = m_Spawner.SpawnerGump;

							if (xg != null)
							{

								xg.Rentry = new XmlSpawnerGump.ReplacementEntry
								{
									Typename = type.Name,
									Index = m_EntryIndex,
									Color = 0x1436
								};

								_ = Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(XmlSpawnerGump.Refresh_Callback), new object[] { from });
								//from.CloseGump(typeof(XmlSpawnerGump));
								//from.SendGump( new XmlSpawnerGump(xg.m_Spawner, xg.X, xg.Y, xg.m_ShowGump, xg.xoffset, xg.page, xg.Rentry) );
							}
						}
					}

					break;
				}
			}
		}
	}
}
