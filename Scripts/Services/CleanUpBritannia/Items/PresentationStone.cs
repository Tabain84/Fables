﻿namespace Server.Items
{
    public class PresentationStone : BaseItem
    {
        public override int LabelNumber => 1154745;  // Presentation Stone

        [Constructable]
        public PresentationStone()
            : base(0x32F2)
        {
            Weight = 5.0;
        }

        public PresentationStone(Serial serial)
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