using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBSEBowyer : SBInfo
    {
        private readonly List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<IBuyItemInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<IBuyItemInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Yumi), 53, 20, 0x27A5, 0));
                Add(new GenericBuyInfo(typeof(Fukiya), 20, 20, 0x27AA, 0));
                Add(new GenericBuyInfo(typeof(Nunchaku), 35, 20, 0x27AE, 0));
                Add(new GenericBuyInfo(typeof(FukiyaDarts), 3, 20, 0x2806, 0));
                Add(new GenericBuyInfo(typeof(Bokuto), 21, 20, 0x27A8, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Yumi), 26);
                Add(typeof(Fukiya), 10);
                Add(typeof(Nunchaku), 17);
                Add(typeof(FukiyaDarts), 1);
                Add(typeof(Bokuto), 10);
            }
        }
    }
}
