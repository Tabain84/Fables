using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server
{
    public class BaseMobile : Mobile
    {
        [Constructable]
        public BaseMobile()
            : base()
        {
            Name = "Base Mobile";
        }

        public BaseMobile(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //Version #
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt(); //Version #

            switch (version)
            {
                case 0: { } break;
            }
        }
    }
}