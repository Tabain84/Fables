using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBScribe : SBInfo
    {
        private readonly List<IBuyItemInfo> m_BuyInfo;
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBScribe(Mobile m)
        {
            if (m != null)
            {
                m_BuyInfo = new InternalBuyInfo(m);
            }
        }

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<IBuyItemInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<IBuyItemInfo>
        {
            public InternalBuyInfo(Mobile m)
            {
                Add(new GenericBuyInfo(typeof(ScribesPen), 8, 20, 0xFBF, 0));
                Add(new GenericBuyInfo(typeof(BlankScroll), 5, 999, 0x0E34, 0));
                Add(new GenericBuyInfo(typeof(BrownBook), 15, 10, 0xFEF, 0));
                Add(new GenericBuyInfo(typeof(TanBook), 15, 10, 0xFF0, 0));
                Add(new GenericBuyInfo(typeof(BlueBook), 15, 10, 0xFF2, 0));

                if (m.Map == Map.Tokuno || m.Map == Map.TerMur)
                {
                    Add(new GenericBuyInfo(typeof(BookOfNinjitsu), 335, 20, 0x23A0, 0));
                    Add(new GenericBuyInfo(typeof(BookOfBushido), 280, 20, 0x238C, 0));
                }
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(ScribesPen), 4);
                Add(typeof(BrownBook), 7);
                Add(typeof(TanBook), 7);
                Add(typeof(BlueBook), 7);
                Add(typeof(BlankScroll), 3);
            }
        }
    }
}
