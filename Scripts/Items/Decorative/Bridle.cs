namespace Server.Items
{
    [TypeAlias("Server.Items.DecoBridle")]
    public class DecoBridleSouth : BaseItem
    {
        [Constructable]
        public DecoBridleSouth()
            : base(0x1374)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoBridleSouth(Serial serial)
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

    [TypeAlias("Server.Items.DecoBridle2")]
    public class DecoBridleEast : BaseItem
    {
        [Constructable]
        public DecoBridleEast()
            : base(0x1375)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoBridleEast(Serial serial)
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