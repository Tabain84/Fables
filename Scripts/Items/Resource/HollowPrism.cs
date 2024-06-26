namespace Server.Items
{
    public class HollowPrism : BaseItem
    {
        [Constructable]
        public HollowPrism()
            : base(0x2F5D)
        {
            Weight = 1.0;
        }

        public HollowPrism(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072895;// hollow prism
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