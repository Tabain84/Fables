using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    public class MagicalItemGenerator : Item
    {
        private Tier _tier = Tier.Random;
        private ItemType _itemType = ItemType.Random;

        [CommandProperty(AccessLevel.GameMaster)]
        public Tier ItemTier
        {
            get => _tier;
            set => _tier = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemType ItemCategory
        {
            get => _itemType;
            set => _itemType = value;
        }

        [Constructable]
        public MagicalItemGenerator() : base(0xE26)  // Gear icon
        {
            Name = "Magical Item Generator";
            Hue = Utility.RandomList(1150, 1153, 1154, 1175, 1367);
            Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.SendMessage("You are too far away.");
                return;
            }

            from.SendMessage($"Generating a random magical {ItemCategory}...");
            CreateMagicalItem(from);
        }

        private void CreateMagicalItem(Mobile from)
        {
            // Generate a random item based on the selected category
            Item item = GenerateItemByType(from);

            if (item == null)
            {
                from.SendMessage("Failed to create an item.");
                return;
            }

            // Apply magical properties
            var (luckChance, minIntensity, maxIntensity, attributeCount) = GetLootParameters(_tier);

            RandomItemGenerator.GenerateRandomItem(item, luckChance, attributeCount, minIntensity, maxIntensity);

            // Add visual effects
            item.Hue = Utility.RandomList(1150, 1175, 1254, 1367, 1372, 1153);
            item.Name = "Magical " + item.Name;

            // Place the item in the world
            item.MoveToWorld(from.Location, from.Map);
            from.SendMessage($"You have created a magical {ItemCategory}.");
        }

        private Item GenerateItemByType(Mobile from)
        {
            switch (_itemType)
            {
                case ItemType.Weapon:
                    return Loot.RandomWeapon();

                case ItemType.Armor:
                    return Loot.RandomArmor();

                case ItemType.Shield:
                    return Loot.RandomShield();

                case ItemType.Instrument:
                    return Loot.RandomInstrument();    

                case ItemType.Jewelry:
                    return Loot.RandomJewelry();

                default:  // Random
                    return Loot.RandomArmorOrShieldOrWeaponOrJewelry();
            }
        }

        private (int luckChance, int minIntensity, int maxIntensity, int attributeCount) GetLootParameters(Tier tier)
        {
            int luckChance, minIntensity, maxIntensity, attributeCount;

            switch (tier)
            {
                case Tier.Common:
                    luckChance = 0;
                    minIntensity = 10;
                    maxIntensity = 50;
                    attributeCount = Utility.RandomMinMax(1, 3);
                    break;

                case Tier.Uncommon:
                    luckChance = 50;
                    minIntensity = 20;
                    maxIntensity = 60;
                    attributeCount = Utility.RandomMinMax(2, 4);
                    break;

                case Tier.Rare:
                    luckChance = 100;
                    minIntensity = 30;
                    maxIntensity = 80;
                    attributeCount = Utility.RandomMinMax(3, 5);
                    break;

                case Tier.Epic:
                    luckChance = 200;
                    minIntensity = 40;
                    maxIntensity = 100;
                    attributeCount = Utility.RandomMinMax(4, 6);
                    break;

                case Tier.Legendary:
                    luckChance = 300;
                    minIntensity = 50;
                    maxIntensity = 150;
                    attributeCount = Utility.RandomMinMax(5, 7);
                    break;

                default:
                    luckChance = Utility.RandomMinMax(0, 300);
                    minIntensity = 10;
                    maxIntensity = 100;
                    attributeCount = Utility.RandomMinMax(1, 6);
                    break;
            }

            return (luckChance, minIntensity, maxIntensity, attributeCount);
        }

        public MagicalItemGenerator(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);  // version
            writer.Write((int)_tier);
            writer.Write((int)_itemType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            _tier = (Tier)reader.ReadInt();
            _itemType = (ItemType)reader.ReadInt();
        }

        public enum Tier
        {
            Random,
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary
        }

        public enum ItemType
        {
            Random,
            Weapon,
            Armor,
            Shield,
            Instrument,
            Jewelry
        }
    }
}
