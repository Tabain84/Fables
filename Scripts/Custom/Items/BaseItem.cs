namespace Server
{
	public abstract class BaseItem : Item
    {

        private string m_Creator;
        private string m_LookText;

        [CommandProperty( AccessLevel.GameMaster )]
        public string LookText
        {
            get { return m_LookText; }
            set { m_LookText = value; }
        }

        public string Creator
        {
            get { return m_Creator; }
        }
        public BaseItem()
            : base()
        {
        }

        public BaseItem(int itemId)
            : base(itemId)
        {
        }

        public BaseItem(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); //Version #

            writer.Write( m_Creator );
            writer.Write(m_LookText);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt(); //Version #

            switch (version)
            {
                case 0: { } break;
                case 1: { m_LookText = reader.ReadString(); } goto case 0;
                case 2: { m_Creator = reader.ReadString(); } goto case 1;
            }
        }
    }
}