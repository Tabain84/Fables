namespace Server.Items
{
    public class GoldBricks : BaseItem
    {
        public override bool IsArtifact => true;
        [Constructable]
        public GoldBricks()
            : base(0x1BEB)
        {
        }

        public GoldBricks(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063489;
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