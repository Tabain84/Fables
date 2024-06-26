namespace Server.Items
{
    public enum SnowGlobeTypeOne
    {
        Britain,
        Moonglow,
        Minoc,
        Magincia,
        BuccaneersDen,
        Trinsic,
        Yew,
        SkaraBrae,
        Jhelom,
        Nujelm,
        Papua,
        Delucia,
        Cove,
        Ocllo,
        SerpentsHold,
        EmpathAbbey,
        TheLycaeum,
        Vesper,
        Wind
    }

    public enum SnowGlobeTypeTwo
    {
        AncientCitadel,
        BlackthornesCastle,
        CityofMontor,
        CityofMistas,
        ExodusLair,
        LakeofFire,
        Lakeshire,
        PassofKarnaugh,
        TheEtherealFortress,
        TwinOaksTavern,
        ChaosShrine,
        ShrineofHumility,
        ShrineofSacrifice,
        ShrineofCompassion,
        ShrineofHonor,
        ShrineofHonesty,
        ShrineofSpirituality,
        ShrineofJustice,
        ShrineofValor
    }

    public enum SnowGlobeTypeThree
    {
        Luna,
        Umbra,
        Zento,
        Heartwood,
        Covetous,
        Deceit,
        Destard,
        Hythloth,
        Khaldun,
        Shame,
        Wrong,
        Doom,
        TheCitadel,
        ThePalaceofParoxysmus,
        TheBlightedGrove,
        ThePrismofLight
    }

    public class SnowGlobe : BaseItem
    {
        public SnowGlobe()
            : base(0xE2F)
        {
            LootType = LootType.Blessed;
            Light = LightType.Circle150;
        }

        public SnowGlobe(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight => 1.0;
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

    public class SnowGlobeOne : SnowGlobe
    {
        private SnowGlobeTypeOne m_Type;
        [Constructable]
        public SnowGlobeOne()
            : this((SnowGlobeTypeOne)Utility.Random(19))
        {
        }

        [Constructable]
        public SnowGlobeOne(SnowGlobeTypeOne type)
        {
            m_Type = type;
        }

        public SnowGlobeOne(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SnowGlobeTypeOne Place
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }
        public override int LabelNumber => 1041454 + (int)m_Type;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.WriteEncodedInt((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Type = (SnowGlobeTypeOne)reader.ReadEncodedInt();
                        break;
                    }
            }
        }
    }

    public class SnowGlobeTwo : SnowGlobe
    {
        /* Oddly, these are not localized. */
        private static readonly string[] m_PlaceNames = new string[]
        {
            /* AncientCitadel */ 		"Ancient Citadel",
            /* BlackthornesCastle */ 	"Blackthorne's Castle",
            /* CityofMontor */ 			"City of Montor",
            /* CityofMistas */ 			"City of Mistas",
            /* ExodusLair */ 			"Exodus' Lair",
            /* LakeofFire */ 			"Lake of Fire",
            /* Lakeshire */ 			"Lakeshire",
            /* PassofKarnaugh */ 		"Pass of Karnaugh",
            /* TheEtherealFortress */ 	"The Ethereal Fortress",
            /* TwinOaksTavern */ 		"Twin Oaks Tavern",
            /* ChaosShrine */ 			"Chaos Shrine",
            /* ShrineofHumility */ 		"Shrine of Humility",
            /* ShrineofSacrifice */ 	"Shrine of Sacrifice",
            /* ShrineofCompassion */ 	"Shrine of Compassion",
            /* ShrineofHonor */ 		"Shrine of Honor",
            /* ShrineofHonesty */ 		"Shrine of Honesty",
            /* ShrineofSpirituality */ 	"Shrine of Spirituality",
            /* ShrineofJustice */ 		"Shrine of Justice",
            /* ShrineofValor */ 		"Shrine of Valor"
        };
        private SnowGlobeTypeTwo m_Type;
        [Constructable]
        public SnowGlobeTwo()
            : this((SnowGlobeTypeTwo)Utility.Random(19))
        {
        }

        [Constructable]
        public SnowGlobeTwo(SnowGlobeTypeTwo type)
        {
            m_Type = type;
        }

        public SnowGlobeTwo(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SnowGlobeTypeTwo Place
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }
        public override string DefaultName
        {
            get
            {
                int idx = (int)m_Type;

                if (idx < 0 || idx >= m_PlaceNames.Length)
                    return "a snowy scene";

                return string.Format("a snowy scene of {0}", m_PlaceNames[idx]);
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.WriteEncodedInt((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Type = (SnowGlobeTypeTwo)reader.ReadEncodedInt();
                        break;
                    }
            }
        }
    }

    public class SnowGlobeThree : SnowGlobe
    {
        private SnowGlobeTypeThree m_Type;
        [Constructable]
        public SnowGlobeThree()
            : this((SnowGlobeTypeThree)Utility.Random(16))
        {
        }

        [Constructable]
        public SnowGlobeThree(SnowGlobeTypeThree type)
        {
            m_Type = type;
        }

        public SnowGlobeThree(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SnowGlobeTypeThree Place
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }
        public override int LabelNumber
        {
            get
            {
                if (m_Type >= SnowGlobeTypeThree.Covetous)
                    return 1075440 + ((int)m_Type - 4);

                return 1075294 + (int)m_Type;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.WriteEncodedInt((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Type = (SnowGlobeTypeThree)reader.ReadEncodedInt();
                        break;
                    }
            }
        }
    }
}
