namespace Server.Items
{
    public class SlithEye : BaseItem
    {
        [Constructable]
        public SlithEye()
            : this(1)
        {
        }

        [Constructable]
        public SlithEye(int amount)
            : base(0x5749)
        {
            Stackable = true;
            Amount = amount;
        }

        public SlithEye(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112396;// slith's eye
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