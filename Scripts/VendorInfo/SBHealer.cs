using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBHealer : SBInfo
    {
        private readonly List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<IBuyItemInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<IBuyItemInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Bandage), 5, 20, 0xE21, 0, true));
                Add(new GenericBuyInfo(typeof(LesserHealPotion), 15, 20, 0xF0C, 0, true));
                Add(new GenericBuyInfo(typeof(Ginseng), 3, 20, 0xF85, 0));
                Add(new GenericBuyInfo(typeof(Garlic), 3, 20, 0xF84, 0));
                Add(new GenericBuyInfo(typeof(RefreshPotion), 15, 20, 0xF0B, 0, true));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Bandage), 1);
                Add(typeof(LesserHealPotion), 7);
                Add(typeof(RefreshPotion), 7);
                Add(typeof(Garlic), 2);
                Add(typeof(Ginseng), 2);
            }
        }
    }
}
