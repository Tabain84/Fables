using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
    public class BlanketOfDarkness : BaseItem
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1152304;  // Blanket Of Darkness	

        [Constructable]
        public BlanketOfDarkness()
            : base(0xA57)
        {
            Hue = 2076;
            Weight = 10.0;
        }

        public BlanketOfDarkness(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Parent != null || !VerifyMove(from))
                return;

            if (!from.InRange(this, 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (ItemID == 0xA57) // rolled
            {
                Direction dir = PlayerMobile.GetDirection4(from.Location, Location);

                if (dir == Direction.North || dir == Direction.South)
                    ItemID = 0xA55;
                else
                    ItemID = 0xA56;
            }
            else // unrolled
            {
                ItemID = 0xA57;

                if (!from.HasGump(typeof(LogoutGump)))
                {
                    CampfireEntry entry = Campfire.GetEntry(from);

                    if (entry != null && entry.Safe)
                        from.SendGump(new LogoutGump(entry, this));
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1061078, "8"); // artifact rarity ~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }

        private class LogoutGump : Gump
        {
            private readonly Timer m_CloseTimer;
            private readonly CampfireEntry m_Entry;
            private readonly BlanketOfDarkness m_BlanketOfDarkness;
            public LogoutGump(CampfireEntry entry, BlanketOfDarkness BlanketOfDarkness)
                : base(100, 0)
            {
                m_Entry = entry;
                m_BlanketOfDarkness = BlanketOfDarkness;

                m_CloseTimer = Timer.DelayCall(TimeSpan.FromSeconds(10.0), CloseGump);

                AddBackground(0, 0, 400, 350, 0xA28);

                AddHtmlLocalized(100, 20, 200, 35, 1011015, false, false); // <center>Logging out via camping</center>

                /* Using a bedroll in the safety of a camp will log you out of the game safely.
                * If this is what you wish to do choose CONTINUE and you will be logged out.
                * Otherwise, select the CANCEL button to avoid logging out at this time.
                * The camp will remain secure for 10 seconds at which time this window will close
                * and you not be logged out.
                */
                AddHtmlLocalized(50, 55, 300, 140, 1011016, true, true);

                AddButton(45, 298, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 300, 110, 35, 1011011, false, false); // CONTINUE

                AddButton(200, 298, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(235, 300, 110, 35, 1011012, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                PlayerMobile pm = m_Entry.Player;

                m_CloseTimer.Stop();

                if (Campfire.GetEntry(pm) != m_Entry)
                    return;

                if (info.ButtonID == 1 && m_Entry.Safe && m_BlanketOfDarkness.Parent == null && m_BlanketOfDarkness.IsAccessibleTo(pm) &&
                    m_BlanketOfDarkness.VerifyMove(pm) && m_BlanketOfDarkness.Map == pm.Map && pm.InRange(m_BlanketOfDarkness, 2))
                {
                    pm.PlaceInBackpack(m_BlanketOfDarkness);

                    pm.BlanketOfDarknessLogout = true;
                    sender.Dispose();
                }

                Campfire.RemoveEntry(m_Entry);
            }

            private void CloseGump()
            {
                Campfire.RemoveEntry(m_Entry);
                m_Entry.Player.CloseGump(typeof(LogoutGump));
            }
        }
    }
}
