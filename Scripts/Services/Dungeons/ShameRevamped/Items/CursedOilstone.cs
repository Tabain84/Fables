﻿using System;

namespace Server.Items
{
    public class CursedOilstone : BaseItem
    {
        public override int LabelNumber => 1151810;  // Cursed Oilstone

        [Constructable]
        public CursedOilstone()
            : this(1)
        {
        }

        [Constructable]
        public CursedOilstone(int amount) : base(0x0F8B)
        {
            Weight = 1;

            Stackable = true;
            Amount = amount;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            else if (from.Backpack.GetAmount(typeof(QuartzGrit)) == 0)
                from.SendLocalizedMessage(1151813, "#1151808"); // You do not have a required component: ~1_val~
            else if (from.Backpack.GetAmount(typeof(CorrosiveAsh)) == 0)
                from.SendLocalizedMessage(1151813, "#1151809"); // You do not have a required component: ~1_val~
            else
            {
                from.Backpack.ConsumeTotal(new Type[] { typeof(QuartzGrit), typeof(CorrosiveAsh) },
                                           new int[] { 1, 1 });

                Consume();

                from.AddToBackpack(new WhetstoneOfEnervation());
                from.SendLocalizedMessage(1151812); // You have managed to form the items into a rancid smelling, crag covered, hardened lump. In a moment of prescience, you realize what it must be named. The Whetstone of Enervation!
            }
        }

        public CursedOilstone(Serial serial)
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