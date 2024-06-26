namespace Server.Items
{
    public class TravestysSushiPreparations : BaseItem
    {
        [Constructable]
        public TravestysSushiPreparations()
            : base(Utility.Random(0x1E15, 2))
        {
        }

        public TravestysSushiPreparations(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075093;// Travesty's Sushi Preparations
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