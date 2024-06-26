namespace Server.Items
{
    public class MummifiedCorpse : BaseItem
    {
        public override int LabelNumber => 1112400;  //a mummified corpse

        [Constructable]
        public MummifiedCorpse() : base(0x1C20)
        {
        }

        public MummifiedCorpse(Serial serial) : base(serial)
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
