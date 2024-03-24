namespace Server.Items
{
    [Flipable(5353, 5354)]
    public class MouldingBoard : BaseItem
    {
        [Constructable]
        public MouldingBoard()
            : base(5353)
        {
        }

        public MouldingBoard(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DoughBowl : BaseItem
    {
        [Constructable]
        public DoughBowl()
            : base(4323)
        {
        }

        public DoughBowl(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HornedTotemPole : BaseItem
    {
        [Constructable]
        public HornedTotemPole()
            : base(12289)
        {
        }

        public HornedTotemPole(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LargeSquarePillow : BaseItem
    {
        [Constructable]
        public LargeSquarePillow()
            : base(5691)
        {
        }

        public LargeSquarePillow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LargeDiamondPillow : BaseItem
    {
        [Constructable]
        public LargeDiamondPillow()
            : base(5690)
        {
        }

        public LargeDiamondPillow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DustyPillow : BaseItem
    {
        public override int LabelNumber => 1113638;  // dusty pillow

        [Constructable]
        public DustyPillow()
            : base(Utility.RandomList(5690, 5691))
        {
        }

        public DustyPillow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class StatuePedestal : BaseItem
    {
        [Constructable]
        public StatuePedestal()
            : base(13042)
        {
            Weight = 5;
        }

        public StatuePedestal(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FlouredBreadBoard : BaseItem
    {
        public override int LabelNumber => 1113639;  // floured bread board

        [Constructable]
        public FlouredBreadBoard()
            : base(0x14E9)
        {
            Weight = 3.0;
        }

        public FlouredBreadBoard(Serial serial)
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
