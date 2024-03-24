namespace Server
{
	public abstract class BaseItem : Item
    {
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

            writer.Write((int)0); //Version #
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt(); //Version #

            switch (version)
            {
                case 0: { } break;
            }
        }
    }
}