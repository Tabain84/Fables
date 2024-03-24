namespace Server.Items
{
    public class Mushrooms1 : BaseItem
    {
        [Constructable]
        public Mushrooms1()
            : base(0x0D0F)
        {
            Weight = 1.0;
        }

        public Mushrooms1(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Mushrooms2 : BaseItem
    {
        [Constructable]
        public Mushrooms2()
            : base(0x0D12)
        {
            Weight = 1.0;
        }

        public Mushrooms2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Mushrooms3 : BaseItem
    {
        [Constructable]
        public Mushrooms3()
            : base(0x0D10)
        {
            Weight = 1.0;
        }

        public Mushrooms3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Mushrooms4 : BaseItem
    {
        [Constructable]
        public Mushrooms4()
            : base(0x0D13)
        {
            Weight = 1.0;
        }

        public Mushrooms4(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}