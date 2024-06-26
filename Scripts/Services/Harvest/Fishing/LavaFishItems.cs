namespace Server.Items
{
    public class StoneFootwear : BaseShoes
    {
        public static void Initialize()
        {
            EventSink.Movement += EventSink_Movement;
        }

        [Constructable]
        public StoneFootwear() : this(Utility.Random(5899, 8))
        {
        }

        [Constructable]
        public StoneFootwear(int itemID)
            : base(itemID)
        {
            string name = GetNameInfo(ItemID);

            if (name == "thigh boots")
            {
                Weight = 4.0;
            }
            else if (name == "boots")
            {
                Weight = 3.0;
            }
            else if (name == "shoes")
            {
                Weight = 2.0;
            }
            else if (name == "sandals")
            {
                Weight = 1.0;
            }

            StrRequirement = 10;
        }

        public string GetNameInfo(int itemID)
        {
            string name = "foot wear";
            switch (itemID)
            {
                case 5899:
                case 5900: name = "boots"; break;
                case 5901:
                case 5902: name = "sandals"; break;
                case 5903:
                case 5904: name = "shoes"; break;
                case 5905:
                case 5906: name = "thigh boots"; break;
            }
            return name;
        }

        public override void OnAdded(IEntity parent)
		{
            if (parent is Mobile m)
            {
                if (SpiderWebbing.IsTrapped(m))
                {
                    SpiderWebbing.RemoveEffects(m);
                }

                m.SendLocalizedMessage(1151094, GetNameInfo(ItemID)); // You manage to equip the stone ~1_token~ and find you can no longer move!
			}

			base.OnAdded(parent);
		}

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1151095, GetNameInfo(ItemID)); // stone ~1_token~
        }

        public static void EventSink_Movement(MovementEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from != null && from.Alive)
            {
                Item item = from.FindItemOnLayer(Layer.Shoes);

                if (item is StoneFootwear)
                    e.Blocked = true;
            }
        }

        public StoneFootwear(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                string name = GetNameInfo(ItemID);

                if (name == "thigh boots")
                {
                    Weight = 4.0;
                }
                else if (name == "boots")
                {
                    Weight = 3.0;
                }
                else if (name == "shoes")
                {
                    Weight = 2.0;
                }
                else if (name == "sandals")
                {
                    Weight = 1.0;
                }

                StrRequirement = 10;
            }
        }
    }

    public class CrackedLavaRockSouth : BaseItem
    {
        public override int LabelNumber => 1098151;

        [Constructable]
        public CrackedLavaRockSouth() : base(19279)
        {
        }

        public virtual void OnCrack(Mobile from)
        {
            Item item;

            switch (Utility.Random(5))
            {
                default:
                case 0: item = new GeodeEast(); break;
                case 1: item = new GeodeSouth(); break;
                case 2: item = new GeodeShardEast(); break;
                case 3: item = new GeodeShardSouth(); break;
                case 4: item = new LavaRock(); break;
            }

            if (item != null)
            {
                from.AddToBackpack(item);
                from.SendMessage("You have split the lava rock!");
                Delete();
            }
        }

        public CrackedLavaRockSouth(Serial serial) : base(serial) { }

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

    public class CrackedLavaRockEast : BaseItem
    {
        public override int LabelNumber => 1098151;

        [Constructable]
        public CrackedLavaRockEast() : base(19275)
        {
        }

        public virtual void OnCrack(Mobile from)
        {
            Item item;
            from.SendSound(0x3B3);

            if (from.RawStr < Utility.Random(150))
            {
                from.SendMessage("You swing, but fail to crack the rock any further.");
                return;
            }

            switch (Utility.Random(5))
            {
                default:
                case 0: item = new GeodeEast(); break;
                case 1: item = new GeodeSouth(); break;
                case 2: item = new GeodeShardEast(); break;
                case 3: item = new GeodeShardSouth(); break;
                case 4: item = new LavaRock(); break;
            }

            if (item != null)
            {
                from.AddToBackpack(item);
                from.SendMessage("You have split the lava rock!");
                Delete();
            }
        }

        public CrackedLavaRockEast(Serial serial) : base(serial) { }

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

    public class GeodeSouth : BaseItem
    {
        public override int LabelNumber => 1098145;

        [Constructable]
        public GeodeSouth() : base(Utility.Random(19277, 2))
        {
            switch (Utility.Random(4))
            {
                case 0: Hue = 2658; break;
                case 1: Hue = 2659; break;
                case 2: Hue = 2660; break;
                case 3: Hue = 2654; break;
            }
        }

        public GeodeSouth(Serial serial) : base(serial) { }

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

    public class GeodeEast : BaseItem
    {
        public override int LabelNumber => 1098145;

        [Constructable]
        public GeodeEast() : base(Utility.Random(19273, 2))
        {
            switch (Utility.Random(4))
            {
                case 0: Hue = 2658; break;
                case 1: Hue = 2659; break;
                case 2: Hue = 2660; break;
                case 3: Hue = 2654; break;
            }
        }

        public GeodeEast(Serial serial) : base(serial) { }

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

    public class GeodeShardSouth : BaseItem
    {
        public override int LabelNumber => 1098148;

        [Constructable]
        public GeodeShardSouth() : base(19276)
        {
            switch (Utility.Random(4))
            {
                case 0: Hue = 2658; break;
                case 1: Hue = 2659; break;
                case 2: Hue = 2660; break;
                case 3: Hue = 2654; break;
            }
        }

        public GeodeShardSouth(Serial serial) : base(serial) { }

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

    public class GeodeShardEast : BaseItem
    {
        public override int LabelNumber => 1098148;

        [Constructable]
        public GeodeShardEast() : base(19272)
        {
            switch (Utility.Random(4))
            {
                case 0: Hue = 2658; break;
                case 1: Hue = 2659; break;
                case 2: Hue = 2660; break;
                case 3: Hue = 2654; break;
            }
        }

        public GeodeShardEast(Serial serial) : base(serial) { }

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

    public class LavaRock : BaseItem
    {
        public override int LabelNumber => 1151166;

        [Constructable]
        public LavaRock() : base(Utility.Random(4964, 6))
        {
            Hue = 1175;
        }

        public LavaRock(Serial serial) : base(serial) { }

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

    public class StonePaver : BaseItem
    {
        public override int LabelNumber => 1097277;

        [Constructable]
        public StonePaver()
            : base(Utility.RandomList(18396, 18397, 18398, 18399, 18400, 18405, 18652, 18653, 18654, 18655))
        {
            Weight = 5;
        }

        public StonePaver(Serial serial) : base(serial) { }

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
