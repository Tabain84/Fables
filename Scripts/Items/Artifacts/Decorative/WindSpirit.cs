namespace Server.Items
{
    public class WindSpirit : BaseItem
    {
        public override bool IsArtifact => true;
        [Constructable]
        public WindSpirit()
            : base(0x1F1F)
        {
        }

        public WindSpirit(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094925;// Wind Spirit [Replica]
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