namespace Server.Items
{
    public class Shell : BaseItem
    {
        [Constructable]
        public Shell()
            : base(Utility.RandomList(0x3B12, 0x3B13))
        {
        }

        public Shell(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074598;// A shell
        public override double DefaultWeight => 1.0;
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1073634); // An aquarium decoration
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