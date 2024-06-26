namespace Server.Items
{
    [Flipable(12287, 12288)]
    public class TwoStoryBanner : BaseItem
    {
        [Constructable]
        public TwoStoryBanner() : base(12287)
        {
        }

        public TwoStoryBanner(Serial serial) : base(serial)
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

    public class Stalagmite : BaseItem
    {
        [Constructable]
        public Stalagmite() : base(Utility.RandomList(2272, 2273, 2276, 2277, 2279, 2281, 2282))
        {
        }

        public Stalagmite(Serial serial) : base(serial)
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

    public class Flowstone : BaseItem
    {
        [Constructable]
        public Flowstone() : base(Utility.RandomList(2274, 2275, 2278, 2280))
        {
        }

        public Flowstone(Serial serial) : base(serial)
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

    public class HangingChainmailLegs : BaseItem
    {
        [Constructable]
        public HangingChainmailLegs() : base(5052)
        {
        }

        public HangingChainmailLegs(Serial serial) : base(serial)
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

    public class HangingRingmailTunic : BaseItem
    {
        [Constructable]
        public HangingRingmailTunic() : base(5095)
        {
        }

        public HangingRingmailTunic(Serial serial) : base(serial)
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

    public class PluckedChicken : BaseItem
    {
        [Constructable]
        public PluckedChicken() : base(7819)
        {
        }

        public PluckedChicken(Serial serial) : base(serial)
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

    public class ColorfulTapestry : BaseItem
    {
        [Constructable]
        public ColorfulTapestry() : base(17092)
        {
        }

        public ColorfulTapestry(Serial serial) : base(serial)
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

    public class CanvaslessEasel : BaseItem
    {
        public override int LabelNumber => 123467;

        [Constructable]
        public CanvaslessEasel() : base(Utility.RandomBool() ? 3943 : 3945)
        {
        }

        public CanvaslessEasel(Serial serial) : base(serial)
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
}