#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

using Server.Accounting;
using Server.Commands.Generic;
using Server.Network;

using CPA = Server.CommandPropertyAttribute;
#endregion

namespace Server.Gumps
{
	public class PropertiesGump : Gump
	{
		public static readonly bool OldStyle = PropsConfig.OldStyle;

		public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
		public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;
		public static readonly int TextHue = PropsConfig.TextHue;
		public static readonly int TextOffsetX = PropsConfig.TextOffsetX;
		public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
		public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
		public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
		public static readonly int BackGumpID = PropsConfig.BackGumpID;
		public static readonly int SetGumpID = PropsConfig.SetGumpID;
		public static readonly int SetWidth = PropsConfig.SetWidth;
		public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
		public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
		public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;
		public static readonly int PrevWidth = PropsConfig.PrevWidth;
		public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
		public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
		public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;
		public static readonly int NextWidth = PropsConfig.NextWidth;
		public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
		public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;
		public static readonly int OffsetSize = PropsConfig.OffsetSize;
		public static readonly int EntryHeight = PropsConfig.EntryHeight;
		public static readonly int BorderSize = PropsConfig.BorderSize;

		public static string[] m_BoolNames = { "True", "False" };
		public static object[] m_BoolValues = { true, false };

		public static string[] m_PoisonNames =
		{
			"None", "Lesser", "Regular", "Greater", "Deadly", "Lethal", "Darkglow", "Parasitic"
		};

		public static object[] m_PoisonValues =
		{
			null, Poison.Lesser, Poison.Regular, Poison.Greater, Poison.Deadly, Poison.Lethal, Poison.DarkGlow, Poison.Parasitic
		};

		private static readonly bool PrevLabel = OldStyle;
		private static readonly bool NextLabel = OldStyle;
		private static readonly bool TypeLabel = !OldStyle;

		private static readonly int PrevLabelOffsetX = PrevWidth + 1;
		private static readonly int PrevLabelOffsetY = 0;
		private static readonly int NextLabelOffsetX = -29;
		private static readonly int NextLabelOffsetY = 0;

		private static readonly int NameWidth = 150; // 107;
		private static readonly int ValueWidth = 200; // 128;

		private static readonly int EntryCount = 25; // 15;

		private static readonly int TypeWidth = NameWidth + OffsetSize + ValueWidth;

		private static readonly int TotalWidth = OffsetSize + NameWidth + OffsetSize + ValueWidth + OffsetSize + SetWidth + OffsetSize;

		//private static readonly int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		//private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

		private static readonly Type _TypeOfIEntity = typeof(IEntity);
		private static readonly Type _TypeOfMobile = typeof(Mobile);
		private static readonly Type _TypeOfItem = typeof(Item);
		private static readonly Type _TypeOfType = typeof(Type);
		private static readonly Type _TypeOfPoint3D = typeof(Point3D);
		private static readonly Type _TypeOfPoint2D = typeof(Point2D);
		private static readonly Type _TypeOfTimeSpan = typeof(TimeSpan);
		private static readonly Type _TypeOfCustomEnum = typeof(CustomEnumAttribute);
		private static readonly Type _TypeOfIDynamicEnum = typeof(IDynamicEnum);
		private static readonly Type _TypeOfEnum = typeof(Enum);
		private static readonly Type _TypeOfFlags = typeof(FlagsAttribute);
		private static readonly Type _TypeOfFlagsProp = typeof(FlagsPropertyAttribute);
		private static readonly Type _TypeOfBool = typeof(bool);
		private static readonly Type _TypeOfString = typeof(string);
		private static readonly Type _TypeOfText = typeof(TextDefinition);
		private static readonly Type _TypeOfPoison = typeof(Poison);
		private static readonly Type _TypeOfMap = typeof(Map);
		private static readonly Type _TypeOfSkills = typeof(Skills);
		private static readonly Type _TypeOfPropertyObject = typeof(PropertyObjectAttribute);
		private static readonly Type _TypeOfNoSort = typeof(NoSortAttribute);
		private static readonly Type _TypeOfDateTime = typeof(DateTime);
		private static readonly Type _TypeOfColor = typeof(Color);
		private static readonly Type _TypeOfAccount = typeof(IAccount);
		private static readonly Type _TypeOfObject = typeof(object);

		private static readonly Type[] _TypeOfReal =
		{
			typeof(float), typeof(double), typeof(decimal)
		};

		private static readonly Type[] _TypeOfNumeric =
		{
			typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong)
		};

		private readonly Mobile m_Mobile;
		private readonly object m_Object;
		private readonly Type m_Type;
		private readonly Stack m_Stack;
		private readonly ArrayList m_List;

		private int m_Page;

		public PropertiesGump(Mobile mobile, object o)
			: base(GumpOffsetX, GumpOffsetY)
		{
			m_Mobile = mobile;
			m_Object = o;
			m_Type = o.GetType();
			m_List = BuildList();

			Initialize(0);
		}

		public PropertiesGump(Mobile mobile, object o, Stack stack, StackEntry parent)
			: base(GumpOffsetX, GumpOffsetY)
		{
			m_Mobile = mobile;
			m_Object = o;
			m_Type = o.GetType();
			m_Stack = stack;
			m_List = BuildList();

			if (parent != null)
			{
				if (m_Stack == null)
				{
					m_Stack = new Stack();
				}

				m_Stack.Push(parent);
			}

			Initialize(0);
		}

		public PropertiesGump(Mobile mobile, object o, Stack stack, ArrayList list, int page)
			: base(GumpOffsetX, GumpOffsetY)
		{
			m_Mobile = mobile;
			m_Object = o;

			if (o != null)
			{
				m_Type = o.GetType();
			}

			m_List = list;
			m_Stack = stack;

			Initialize(page);
		}

		public static void OnValueChanged(object obj, PropertyInfo prop, Stack stack)
		{
			if (stack == null || stack.Count == 0)
			{
				return;
			}

			if (!prop.PropertyType.IsValueType)
			{
				return;
			}

			var peek = (StackEntry)stack.Peek();

			if (peek.m_Property.CanWrite)
			{
				peek.m_Property.SetValue(peek.m_Object, obj, null);
			}
		}

		public static string ValueToString(object obj, PropertyInfo prop)
		{
			try
			{
				return ValueToString(prop.GetValue(obj, null));
			}
			catch (Exception e)
			{
				return $"!{e.GetType()}!";
			}
		}

		public static string ValueToString(object o)
		{
			if (o == null)
			{
				return "-null-";
			}

			if (o is string)
			{
				return $"\"{o}\"";
			}

			if (o is bool)
			{
				return o.ToString();
			}

			if (o is char ch)
			{
				return $"0x{(int)ch:X} '{ch}'";
			}

			if (o is Serial s)
			{
				if (s.IsValid)
				{
					if (s.IsItem)
					{
						return $"(I) 0x{s.Value:X}";
					}

					if (s.IsMobile)
					{
						return $"(M) 0x{s.Value:X}";
					}
				}

				return $"(?) 0x{s.Value:X}";
			}

			if (o is sbyte || o is byte || o is short || o is ushort || o is int || o is uint || o is long || o is ulong)
			{
				return $"{o} (0x{o:X})";
			}

			if (o is Mobile m)
			{
				return $"(M) 0x{m.Serial.Value:X} \"{m.Name}\"";
			}

			if (o is Item i)
			{
				return $"(I) 0x{i.Serial.Value:X} \"{i.Name}\"";
			}

			if (o is Type t)
			{
				return t.Name;
			}

			if (o is IAccount a)
			{
				return a.Username;
			}

			if (o is Color c)
			{
				if (c.IsEmpty)
				{
					return "---";
				}

				if (c.IsNamedColor)
				{
					return c.Name;
				}

				return $"#{c.ToArgb() & 0x00FFFFFF:X6}";
			}

			if (o is TextDefinition td)
			{
				return td.Format(true);
			}

			if (o is IDynamicEnum de)
			{
				return de.Value;
			}

			return o.ToString();
		}

		public static object GetObjectFromString(Type t, string s)
		{
			if (t == typeof(string))
			{
				return s;
			}

			if (t == typeof(sbyte) || t == typeof(byte) || t == typeof(short) || t == typeof(ushort) || t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong))
			{
				if (s.StartsWith("0x"))
				{
					if (t == typeof(ulong) || t == typeof(uint) || t == typeof(ushort) || t == typeof(byte))
					{
						return Convert.ChangeType(Convert.ToUInt64(s.Substring(2), 16), t);
					}

					return Convert.ChangeType(Convert.ToInt64(s.Substring(2), 16), t);
				}

				return Convert.ChangeType(s, t);
			}

			if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
			{
				return Convert.ChangeType(s, t);
			}

			if (t == typeof(IAccount) || t == typeof(Account))
			{
				return Accounts.GetAccount(s);
			}

			if (t == typeof(Color))
			{
				if (String.IsNullOrWhiteSpace(s) || s == "---")
				{
					return Color.Empty;
				}

				if (Insensitive.Equals(s, "None") || Insensitive.Equals(s, "Empty"))
				{
					return Color.Empty;
				}

				if (Insensitive.StartsWith(s, "0x"))
				{
					return Color.FromArgb(Convert.ToInt32(s.Substring(2), 16));
				}

				if (Insensitive.StartsWith(s, "#"))
				{
					return Color.FromArgb(Convert.ToInt32(s.Substring(1), 16));
				}

				if (Int32.TryParse(s, out var val))
				{
					return Color.FromArgb(val);
				}

				var rgb = s.Split(',');

				if (rgb.Length >= 3)
				{
					if (Byte.TryParse(rgb[0], out var r) && Byte.TryParse(rgb[1], out var g) && Byte.TryParse(rgb[2], out var b))
					{
						return Color.FromArgb(r, g, b);
					}
				}

				return Color.FromName(s);
			}

			if (t.IsDefined(typeof(ParsableAttribute), false))
			{
				var parseMethod = t.GetMethod("Parse", new[] { typeof(string) });

				return parseMethod.Invoke(null, new object[] { s });
			}

			throw new FormatException();
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			var from = state.Mobile;

			if (!BaseCommand.IsAccessible(from, m_Object))
			{
				from.SendMessage("You may no longer access their properties.");
				return;
			}

			switch (info.ButtonID)
			{
				case 0: // Closed
				{
					if (m_Stack != null && m_Stack.Count > 0)
					{
						var entry = (StackEntry)m_Stack.Pop();

						_ = from.SendGump(new PropertiesGump(from, entry.m_Object, m_Stack, null));
					}

					return;
				}
				case 1: // Previous
				{
					if (m_Page > 0)
					{
						_ = from.SendGump(new PropertiesGump(from, m_Object, m_Stack, m_List, m_Page - 1));
					}

					return;
				}
				case 2: // Next
				{
					if ((m_Page + 1) * EntryCount < m_List.Count)
					{
						_ = from.SendGump(new PropertiesGump(from, m_Object, m_Stack, m_List, m_Page + 1));
					}

					return;
				}
			}

			var index = (m_Page * EntryCount) + (info.ButtonID - 3);

			if (index < 0 || index >= m_List.Count)
			{
				return;
			}

			var prop = m_List[index] as PropertyInfo;

			if (prop == null)
			{
				return;
			}

			var attr = GetCPA(prop);

			if (!prop.CanWrite || attr == null || from.AccessLevel < attr.WriteLevel || attr.ReadOnly)
			{
				return;
			}

			var type = prop.PropertyType;

			if (IsType(type, _TypeOfType))
			{
				from.SendMessage($"Target an object to use its type reference for {prop.Name}...");
				from.Target = new SetObjectTarget(prop, from, m_Object, m_Stack, type, m_Page, m_List);
				return;
			}

			if (IsType(type, _TypeOfMobile) || IsType(type, _TypeOfItem) || IsType(type, _TypeOfIEntity))
			{
				_ = from.SendGump(new SetObjectGump(prop, from, m_Object, m_Stack, type, m_Page, m_List));
				return;
			}

			if (IsType(type, _TypeOfPoint3D))
			{
				_ = from.SendGump(new SetPoint3DGump(prop, from, m_Object, m_Stack, m_Page, m_List));
				return;
			}

			if (IsType(type, _TypeOfPoint2D))
			{
				_ = from.SendGump(new SetPoint2DGump(prop, from, m_Object, m_Stack, m_Page, m_List));
				return;
			}

			if (IsType(type, _TypeOfTimeSpan))
			{
				_ = from.SendGump(new SetTimeSpanGump(prop, from, m_Object, m_Stack, m_Page, m_List));
				return;
			}

			if (IsCustomEnum(type))
			{
				_ = from.SendGump(new SetCustomEnumGump(prop, from, m_Object, m_Stack, m_Page, m_List, GetCustomEnumNames(type)));
				return;
			}

			if (IsType(type, _TypeOfIDynamicEnum))
			{
				_ = from.SendGump(new SetCustomEnumGump(prop, from, m_Object, m_Stack, m_Page, m_List, ((IDynamicEnum)prop.GetValue(m_Object, null)).Values));
				return;
			}

			if (IsType(type, _TypeOfEnum))
			{
				if (HasAttribute(type, _TypeOfFlags, false) && HasAttribute(prop, _TypeOfFlagsProp, false))
				{
					_ = from.SendGump(new SetFlagsEnumGump(prop, from, m_Object, m_Stack, m_Page, m_List));
				}
				else
				{	
					_ = from.SendGump(new SetListOptionGump(prop, from, m_Object, m_Stack, m_Page, m_List, Enum.GetNames(type), GetObjects(Enum.GetValues(type))));
				}
				
				return;
			}

			if (IsType(type, _TypeOfBool))
			{
				_ = from.SendGump(new SetListOptionGump(prop, from, m_Object, m_Stack, m_Page, m_List, m_BoolNames, m_BoolValues));
				return;
			}

			if (IsType(type, _TypeOfString) || IsType(type, _TypeOfReal) || IsType(type, _TypeOfNumeric) || IsType(type, _TypeOfText))
			{
				_ = from.SendGump(new SetGump(prop, from, m_Object, m_Stack, m_Page, m_List));
				return;
			}

			if (IsType(type, _TypeOfPoison))
			{
				_ = from.SendGump(new SetListOptionGump(prop, from, m_Object, m_Stack, m_Page, m_List, m_PoisonNames, m_PoisonValues));
				return;
			}

			if (IsType(type, _TypeOfDateTime))
			{
				_ = from.SendGump(new SetDateTimeGump(prop, from, m_Object, m_Stack, m_Page, m_List));
				return;
			}

			if (IsType(type, _TypeOfMap))
			{
				_ = from.SendGump(new SetListOptionGump(prop, from, m_Object, m_Stack, m_Page, m_List, Map.GetMapNames(), Map.GetMapValues()));
				return;
			}

			if (IsType(type, _TypeOfSkills) && m_Object is Mobile m)
			{
				_ = from.SendGump(new PropertiesGump(from, m_Object, m_Stack, m_List, m_Page));
				_ = from.SendGump(new SkillsGump(from, m));
				return;
			}

			if (IsType(type, _TypeOfColor))
			{
				_ = from.SendGump(new SetColorGump(prop, from, m_Object, m_Stack, m_Page, m_List));
				return;
			}

			if (IsType(type, _TypeOfAccount))
			{
				_ = from.SendGump(new PropertiesGump(from, m_Object, m_Stack, m_List, m_Page));
				return;
			}

			if (HasAttribute(type, _TypeOfPropertyObject, true))
			{
				var obj = prop.GetValue(m_Object, null);

				if (obj != null)
				{
					_ = from.SendGump(new PropertiesGump(from, obj, m_Stack, new StackEntry(m_Object, prop)));
				}
				else
				{
					_ = from.SendGump(new PropertiesGump(from, m_Object, m_Stack, m_List, m_Page));
				}

				return;
			}

			_ = from.SendGump(new PropertiesGump(from, m_Object, m_Stack, m_List, m_Page));
		}

		public static object[] GetObjects(Array a)
		{
			var list = new object[a.Length];

			for (var i = 0; i < list.Length; ++i)
			{
				list[i] = a.GetValue(i);
			}

			return list;
		}

		public static T[] GetObjects<T>(Array a)
		{
			var list = new T[a.Length];

			for (var i = 0; i < list.Length; ++i)
			{
				list[i] = (T)a.GetValue(i);
			}

			return list;
		}

		public static bool IsCustomEnum(Type type)
		{
			return type.IsDefined(_TypeOfCustomEnum, false);
		}

		public static string[] GetCustomEnumNames(Type type)
		{
			var attrs = type.GetCustomAttributes(_TypeOfCustomEnum, false);

			if (attrs.Length == 0)
			{
				return new string[0];
			}

			if (!(attrs[0] is CustomEnumAttribute ce))
			{
				return new string[0];
			}

			return ce.Names;
		}

		public static bool HasAttribute(PropertyInfo prop, Type check, bool inherit)
		{
			return prop.GetCustomAttribute(check, inherit) != null;
		}

		public static bool HasAttribute(Type type, Type check, bool inherit)
		{
			return type.IsDefined(check, inherit);
		}

		public static bool IsType(Type type, Type check)
		{
			return type == check || (check.IsInterface ? check.IsAssignableFrom(type) : type.IsSubclassOf(check));
		}

		public static bool IsType(Type type, IEnumerable<Type> check)
		{
			return check.Any(t => IsType(type, t));
		}

		public static CPA GetCPA(PropertyInfo prop)
		{
			return prop.GetCustomAttribute<CPA>(false);
		}

		private void Initialize(int page)
		{
			m_Page = page;

			var count = m_List.Count - (page * EntryCount);

			if (count < 0)
			{
				count = 0;
			}
			else if (count > EntryCount)
			{
				count = EntryCount;
			}

			var lastIndex = (page * EntryCount) + count - 1;

			if (lastIndex >= 0 && lastIndex < m_List.Count && m_List[lastIndex] == null)
			{
				--count;
			}

			var totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

			AddPage(0);

			AddBackground(0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID);
			AddImageTiled(
				BorderSize,
				BorderSize,
				TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0),
				totalHeight,
				OffsetGumpID);

			var x = BorderSize + OffsetSize;
			var y = BorderSize + OffsetSize;

			var emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

			if (OldStyle)
			{
				AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
			}
			else
			{
				AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);
			}

			if (page > 0)
			{
				AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0);

				if (PrevLabel)
				{
					AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
				}
			}

			x += PrevWidth + OffsetSize;

			if (!OldStyle)
			{
				AddImageTiled(x, y, emptyWidth, EntryHeight, HeaderGumpID);
			}

			if (TypeLabel && m_Type != null)
			{
				AddHtml(x, y, emptyWidth, EntryHeight, $"<BASEFONT COLOR=#FAFAFA><CENTER>{m_Type.Name}</CENTER>", false, false);
			}

			x += emptyWidth + OffsetSize;

			if (!OldStyle)
			{
				AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);
			}

			if ((page + 1) * EntryCount < m_List.Count)
			{
				AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1);

				if (NextLabel)
				{
					AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
				}
			}

			for (int i = 0, index = page * EntryCount; i < count && index < m_List.Count; ++i, ++index)
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				var o = m_List[index];

				if (o is Type type)
				{
					AddImageTiled(x, y, TypeWidth, EntryHeight, EntryGumpID);
					AddLabelCropped(x + TextOffsetX, y, TypeWidth - TextOffsetX, EntryHeight, TextHue, type.Name);
					x += TypeWidth + OffsetSize;

					if (SetGumpID != 0)
					{
						AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
					}
				}
				else if (o is PropertyInfo prop)
				{
					AddImageTiled(x, y, NameWidth, EntryHeight, EntryGumpID);
					AddLabelCropped(x + TextOffsetX, y, NameWidth - TextOffsetX, EntryHeight, TextHue, prop.Name);
					x += NameWidth + OffsetSize;

					var value = ValueToString(prop);

					if (value.Length * 6 >= ValueWidth - TextOffsetX)
					{
						value = value.Substring(0, (ValueWidth - TextOffsetX) / 6);
					}

					AddImageTiled(x, y, ValueWidth, EntryHeight, EntryGumpID);
					AddLabelCropped(x + TextOffsetX, y, ValueWidth - TextOffsetX, EntryHeight, TextHue, value);
					x += ValueWidth + OffsetSize;

					if (SetGumpID != 0)
					{
						AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
					}

					var cpa = GetCPA(prop);

					if (prop.CanWrite && cpa != null && m_Mobile.AccessLevel >= cpa.WriteLevel && !cpa.ReadOnly)
					{
						AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 3, GumpButtonType.Reply, 0);
					}
				}
				else
				{
					AddImageTiled(x - OffsetSize, y, TotalWidth, EntryHeight, BackGumpID + 4);
				}
			}
		}

		private string ValueToString(PropertyInfo prop)
		{
			return ValueToString(m_Object, prop);
		}

		private ArrayList BuildList()
		{
			var list = new ArrayList();

			if (m_Type == null)
			{
				return list;
			}

			var props = m_Type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

			var groups = GetGroups(m_Type, props);

			for (var i = 0; i < groups.Count; ++i)
			{
				var de = (DictionaryEntry)groups[i];
				var groupList = (ArrayList)de.Value;

				if (!HasAttribute((Type)de.Key, _TypeOfNoSort, false))
				{
					groupList.Sort(PropertySorter.Instance);
				}

				if (i != 0)
				{
					_ = list.Add(new { });
				}

				_ = list.Add(de.Key);
				list.AddRange(groupList);
			}

			return list;
		}

		private ArrayList GetGroups(Type objectType, IEnumerable<PropertyInfo> props)
		{
			var groups = new Hashtable();

			foreach (var prop in props)
			{
				if (!prop.CanRead)
				{
					continue;
				}

				var attr = GetCPA(prop);

				if (attr == null || m_Mobile.AccessLevel < attr.ReadLevel)
				{
					continue;
				}

				var type = prop.DeclaringType;
				var die = false;

				if (type == null)
				{
					continue;
				}

				while (!die)
				{
					var baseType = type.BaseType;

					if (baseType == null || baseType == _TypeOfObject)
					{
						die = true;
					}
					else if (baseType.GetProperty(prop.Name, prop.PropertyType) != null)
					{
						type = baseType;
					}
					else
					{
						die = true;
					}
				}

				if (!(groups[type] is ArrayList list))
				{
					groups[type] = list = new ArrayList();
				}

				_ = list.Add(prop);
			}

			var sorted = new ArrayList(groups);

			sorted.Sort(new GroupComparer(objectType));

			return sorted;
		}

		public class StackEntry
		{
			public object m_Object;
			public PropertyInfo m_Property;

			public StackEntry(object obj, PropertyInfo prop)
			{
				m_Object = obj;
				m_Property = prop;
			}
		}

		private class PropertySorter : IComparer
		{
			public static readonly PropertySorter Instance = new PropertySorter();

			private PropertySorter()
			{ }

			public int Compare(object x, object y)
			{
				if (x == y)
				{
					return 0;
				}

				if (x == null)
				{
					return -1;
				}

				if (y == null)
				{
					return 1;
				}

				if (!(x is PropertyInfo) || !(y is PropertyInfo))
				{
					throw new ArgumentException();
				}

				var a = (PropertyInfo)x;
				var b = (PropertyInfo)y;

				return String.Compare(a.Name, b.Name, StringComparison.Ordinal);
			}
		}

		private class GroupComparer : IComparer
		{
			private readonly Type m_Start;

			public GroupComparer(Type start)
			{
				m_Start = start;
			}

			public int Compare(object x, object y)
			{
				if (x == y)
				{
					return 0;
				}

				if (x == null)
				{
					return -1;
				}

				if (y == null)
				{
					return 1;
				}

				if (!(x is DictionaryEntry) || !(y is DictionaryEntry))
				{
					throw new ArgumentException();
				}

				var de1 = (DictionaryEntry)x;
				var de2 = (DictionaryEntry)y;

				var a = (Type)de1.Key;
				var b = (Type)de2.Key;

				return GetDistance(a).CompareTo(GetDistance(b));
			}

			private int GetDistance(Type type)
			{
				var current = m_Start;
				int dist;

				for (dist = 0; current != null && current != _TypeOfObject && current != type; ++dist)
				{
					current = current.BaseType;
				}

				return dist;
			}
		}
	}
}
