namespace Server.Items
{
    public class StatueSouth : BaseItem
    {
        [Constructable]
        public StatueSouth()
            : base(0x139A)
        {
            Weight = 10;
        }

        public StatueSouth(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    public class StatueSouth2 : BaseItem
    {
        [Constructable]
        public StatueSouth2()
            : base(0x1227)
        {
            Weight = 10;
        }

        public StatueSouth2(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    public class StatueNorth : BaseItem
    {
        [Constructable]
        public StatueNorth()
            : base(0x139B)
        {
            Weight = 10;
        }

        public StatueNorth(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    public class StatueWest : BaseItem
    {
        [Constructable]
        public StatueWest()
            : base(0x1226)
        {
            Weight = 10;
        }

        public StatueWest(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    public class StatueEast : BaseItem
    {
        [Constructable]
        public StatueEast()
            : base(0x139C)
        {
            Weight = 10;
        }

        public StatueEast(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    public class StatueEast2 : BaseItem
    {
        [Constructable]
        public StatueEast2()
            : base(0x1224)
        {
            Weight = 10;
        }

        public StatueEast2(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    public class StatueSouthEast : BaseItem
    {
        [Constructable]
        public StatueSouthEast()
            : base(0x1225)
        {
            Weight = 10;
        }

        public StatueSouthEast(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    public class BustSouth : BaseItem
    {
        [Constructable]
        public BustSouth()
            : base(0x12CB)
        {
            Weight = 10;
        }

        public BustSouth(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    public class BustEast : BaseItem
    {
        [Constructable]
        public BustEast()
            : base(0x12CA)
        {
            Weight = 10;
        }

        public BustEast(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    [TypeAlias("Server.Items.StatuePegasus")]
    public class StatuePegasusSouth : BaseItem
    {
        public override int LabelNumber => 1044510;  // pegasus statuette

        [Constructable]
        public StatuePegasusSouth()
            : base(0x139D)
        {
            Weight = 1.0;
        }

        public StatuePegasusSouth(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    [TypeAlias("Server.Items.StatuePegasus2")]
    public class StatuePegasusEast : BaseItem
    {
        public override int LabelNumber => 1044510;  // pegasus statuette

        [Constructable]
        public StatuePegasusEast()
            : base(0x1228)
        {
            Weight = 1.0;
        }

        public StatuePegasusEast(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    public class SmallTowerSculpture : BaseItem
    {
        [Constructable]
        public SmallTowerSculpture()
            : base(0x241A)
        {
            Weight = 1.0;
        }

        public SmallTowerSculpture(Serial serial)
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

    [Flipable(0x494E, 0x494D)]
    public class StatueGargoyleEast : BaseItem
    {
        [Constructable]
        public StatueGargoyleEast()
            : base(0x494E)
        {
            Weight = 1.0;
        }

        public StatueGargoyleEast(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    [Flipable(0x494D, 0x494E)]
    public class StatueGargoyleSouth : BaseItem
    {
        [Constructable]
        public StatueGargoyleSouth()
            : base(0x494D)
        {
            Weight = 1.0;
        }

        public StatueGargoyleSouth(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    [Flipable(0x493C, 0x493B)]
    public class StatueGryphonEast : BaseItem
    {
        [Constructable]
        public StatueGryphonEast()
            : base(0x493C)
        {
            Weight = 1.0;
        }

        public StatueGryphonEast(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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

    [Flipable(0x493B, 0x493C)]
    public class StatueGryphonSouth : BaseItem
    {
        [Constructable]
        public StatueGryphonSouth()
            : base(0x493B)
        {
            Weight = 1.0;
        }

        public StatueGryphonSouth(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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
