using System;

using Server.Network;

namespace Server.Gumps
{
	public class GumpTooltip : GumpEntry
	{
		private int m_Number;
		private string m_Args;

		public GumpTooltip(int number)
			: this(number, null)
		{
		}

		public GumpTooltip(int number, string args)
		{
			m_Number = number;
			m_Args = args;
		}

		public int Number
		{
			get => m_Number;
			set => Delta(ref m_Number, value);
		}

		public string Args
		{
			get => m_Args;
			set => Delta(ref m_Args, value);
		}

		public override string Compile()
		{
			if (String.IsNullOrEmpty(m_Args))
				return $"{{ tooltip {m_Number} }}";

			return $"{{ tooltip {m_Number} @{m_Args}@ }}";
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("tooltip");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_Number);

			if (!String.IsNullOrEmpty(m_Args))
				disp.AppendLayout(m_Args);
		}
	}
}
