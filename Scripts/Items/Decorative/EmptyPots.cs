namespace Server.Items
{
    public class SmallEmptyPot : BaseItem
    {
        [Constructable]
        public SmallEmptyPot()
            : base(0x11C6)
        {
            Weight = 100;
        }

        public SmallEmptyPot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LargeEmptyPot : BaseItem
    {
        [Constructable]
        public LargeEmptyPot()
            : base(0x11C7)
        {
            Weight = 6;
        }

        public LargeEmptyPot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}