using Server.Network;

namespace Server.Gumps
{
	public class GumpCheck : GumpEntry
	{
		private int m_X, m_Y;
		private int m_ID1, m_ID2;
		private bool m_InitialState;
		private int m_SwitchID;

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

		public int InactiveID
		{
			get => m_ID1;
			set => Delta(ref m_ID1, value);
		}

		public int ActiveID
		{
			get => m_ID2;
			set => Delta(ref m_ID2, value);
		}

		public bool InitialState
		{
			get => m_InitialState;
			set => Delta(ref m_InitialState, value);
		}

		public int SwitchID
		{
			get => m_SwitchID;
			set => Delta(ref m_SwitchID, value);
		}

		public GumpCheck(int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
		{
			m_X = x;
			m_Y = y;
			m_ID1 = inactiveID;
			m_ID2 = activeID;
			m_InitialState = initialState;
			m_SwitchID = switchID;
		}

		public override string Compile()
		{
			return $"{{ checkbox {m_X} {m_Y} {m_ID1} {m_ID2} {(m_InitialState ? 1 : 0)} {m_SwitchID} }}";
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("checkbox");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_X);
			disp.AppendLayout(m_Y);
			disp.AppendLayout(m_ID1);
			disp.AppendLayout(m_ID2);
			disp.AppendLayout(m_InitialState);
			disp.AppendLayout(m_SwitchID);

			disp.Switches++;
		}
	}
}