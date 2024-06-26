using Server.Network;

namespace Server.Gumps
{
	public class GumpSpriteImage : GumpEntry
	{
		private int m_X, m_Y, m_SX, m_SY;
		private int m_Width, m_Height;
		private int m_GumpID;

		public GumpSpriteImage(int x, int y, int gumpID, int width, int height, int sx, int sy)
		{
			m_X = x;
			m_Y = y;
			m_GumpID = gumpID;
			m_Width = width;
			m_Height = height;
			m_SX = sx;
			m_SY = sy;
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

		public int Width
		{
			get => m_Width;
			set => Delta(ref m_Width, value);
		}

		public int Height
		{
			get => m_Height;
			set => Delta(ref m_Height, value);
		}

		public int GumpID
		{
			get => m_GumpID;
			set => Delta(ref m_GumpID, value);
		}

		public int SX
		{
			get => m_SX;
			set => Delta(ref m_SX, value);
		}

		public int SY
		{
			get => m_SY;
			set => Delta(ref m_SY, value);
		}

		public override string Compile()
		{
			return $"{{ picinpic {m_X} {m_Y} {m_GumpID} {m_Width} {m_Height} {m_SX} {m_SY} }}";
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("picinpic");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_X);
			disp.AppendLayout(m_Y);
			disp.AppendLayout(m_GumpID);
			disp.AppendLayout(m_Width);
			disp.AppendLayout(m_Height);
			disp.AppendLayout(m_SX);
			disp.AppendLayout(m_SY);
		}
	}
}
