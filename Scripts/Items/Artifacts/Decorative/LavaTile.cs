namespace Server.Items
{
    public class LavaTile : BaseItem
    {
        [Constructable]
        public LavaTile()
            : base(0x12EE)
        {
        }

        public LavaTile(Serial serial)
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