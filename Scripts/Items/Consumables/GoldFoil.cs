namespace Server.Items
{
    public class GoldFoil : BaseItem
    {
        public override int LabelNumber => 1124032;  // foil sheet

        [Constructable]
        public GoldFoil() : this(1)
        {
        }

        [Constructable]
        public GoldFoil(int amount)
            : base(0x9C48)
        {
            Stackable = true;
            Amount = amount;
            Weight = 2.0;
            Hue = 1281;
        }

        public GoldFoil(Serial serial)
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