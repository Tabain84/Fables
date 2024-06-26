namespace Server.Items
{
    public class ParoxysmusDinner : BaseItem
    {
        [Constructable]
        public ParoxysmusDinner()
            : base(0x1E95)
        {
        }

        public ParoxysmusDinner(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072086;// Paroxysmus' Dinner
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