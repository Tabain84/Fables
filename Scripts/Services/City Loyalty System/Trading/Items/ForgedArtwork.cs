namespace Server.Items
{
    public class ForgedArtwork : BaseItem
    {
        public override int LabelNumber => 1151750;  // Forged Artwork from King Blackthorn's Collection

        [Constructable]
        public ForgedArtwork() : base(Utility.Random(0x1224, 5))
        {
            Weight = 10;
        }

        public ForgedArtwork(Serial serial)
            : base(serial)
        {
        }

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