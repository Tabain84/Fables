namespace Server.Items
{
    [Furniture]
    [Flipable(0x2DD3, 0x2DD4)]
    public class ElvenAlchemyTable : BaseItem
    {
        [Constructable]
        public ElvenAlchemyTable()
            : base(0x2DD3)
        {
            Weight = 15.0;
        }

        public ElvenAlchemyTable(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1032407;// elven alchemy table
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