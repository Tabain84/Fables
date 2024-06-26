namespace Server.Items
{
    public class SmallFish : BaseItem
    {
        [Constructable]
        public SmallFish()
            : base(Utility.Random(0x0DD6, 2))
        {
        }

        public SmallFish(Serial serial)
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
