namespace Server.Items
{
    public class HydraScale : BaseItem
    {
        [Constructable]
        public HydraScale()
            : base(0x26B4)
        {
            Hue = 0xC2; // TODO check
        }

        public HydraScale(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074760;// A hydra scale.
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