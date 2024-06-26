namespace Server.Items
{
    public class EssenceSingularity : BaseItem, ICommodity
    {
        [Constructable]
        public EssenceSingularity()
            : this(1)
        {
        }

        [Constructable]
        public EssenceSingularity(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1109;
        }

        public EssenceSingularity(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113341;// essence of singularity
        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;
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
