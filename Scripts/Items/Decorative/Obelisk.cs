namespace Server.Items
{
    public class Obelisk : BaseItem
    {
        [Constructable]
        public Obelisk()
            : base(0x1184)
        {
            Movable = false;
        }

        public Obelisk(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1016474;// an obelisk
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