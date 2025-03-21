using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class FreeSkillStone : Item
    {
        [Constructable]
        public FreeSkillStone()
            : base(4484)
        {
            Name = "A Free Skill Stone";
            Movable = false;
            Hue = 1161;
        }

        public FreeSkillStone(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 3))
                from.SendLocalizedMessage(502138);
            else
                SetSkills((PlayerMobile)from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private void SetSkills(PlayerMobile p)
        {
            Skills sk = p.Skills;
            for (int i = 0; i < SkillMaster.AllSkills.Length; i++)
            {
                sk[SkillMaster.AllSkills[i]].BaseFixedPoint = p.GetSkillCap(SkillMaster.AllSkills[i]) * 10;
            }
            //p.RawStr = p.StrCap - 1;
            //p.RawDex = p.DexCap - 1;
            //p.RawInt = p.IntCap - 1;
            p.SendMessage("All your skills have been set to their cap.");
        }
    }
}
