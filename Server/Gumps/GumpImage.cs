using Server.Network;

namespace Server.Gumps
{
	public class GumpImage : GumpEntry
	{
		private int m_X, m_Y;
		private int m_GumpID;
		private int m_Hue;

		public GumpImage(int x, int y, int gumpID) : this(x, y, gumpID, 0)
		{
		}

		public GumpImage(int x, int y, int gumpID, int hue)
		{
			m_X = x;
			m_Y = y;
			m_GumpID = gumpID;
			m_Hue = hue;
		}

		public int X
		{
			get => m_X;
			set => Delta(ref m_X, value);
		}

		public int Y
		{
			get => m_Y;
			set => Delta(ref m_Y, value);
		}

		public int GumpID
		{
			get => m_GumpID;
			set => Delta(ref m_GumpID, value);
		}

		public int Hue
		{
			get => m_Hue;
			set => Delta(ref m_Hue, value);
		}

		public override string Compile()
		{
			if (m_Hue == 0)
				return $"{{ gumppic {m_X} {m_Y} {m_GumpID} }}";
			else
				return $"{{ gumppic {m_X} {m_Y} {m_GumpID} hue={m_Hue} }}";
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("gumppic");
		private static readonly byte[] m_HueEquals = Gump.StringToBuffer(" hue=");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_X);
			disp.AppendLayout(m_Y);
			disp.AppendLayout(m_GumpID);

			if (m_Hue != 0)
			{
				disp.AppendLayout(m_HueEquals);
				disp.AppendLayoutNS(m_Hue);
			}
		}
	}
}