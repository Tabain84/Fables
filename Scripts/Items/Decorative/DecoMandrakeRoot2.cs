namespace Server.Items
{
    public class DecoMandrakeRoot2 : BaseItem
    {
        [Constructable]
        public DecoMandrakeRoot2()
            : base(0x18DD)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoMandrakeRoot2(Serial serial)
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