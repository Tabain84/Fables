using System.Collections.Generic;

namespace Server.Mobiles
{
    public abstract class SBInfo
    {
        public abstract IShopSellInfo SellInfo { get; }
        public abstract List<IBuyItemInfo> BuyInfo { get; }
    }
}
