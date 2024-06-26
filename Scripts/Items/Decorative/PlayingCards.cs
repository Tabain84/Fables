namespace Server.Items
{
    public class PlayingCards : BaseItem
    {
        [Constructable]
        public PlayingCards()
            : base(0xFA3)
        {
            Movable = true;
            Stackable = false;
        }

        public PlayingCards(Serial serial)
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