using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBButcher : SBInfo
    {
        private readonly List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<IBuyItemInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<IBuyItemInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Bacon), 7, 20, 0x979, 0, true));
                Add(new GenericBuyInfo(typeof(Ham), 26, 20, 0x9C9, 0, true));
                Add(new GenericBuyInfo(typeof(Sausage), 18, 20, 0x9C0, 0, true));
                Add(new GenericBuyInfo(typeof(RawChickenLeg), 6, 20, 0x1607, 0, true));
                Add(new GenericBuyInfo(typeof(RawBird), 9, 20, 0x9B9, 0, true));
                Add(new GenericBuyInfo(typeof(RawLambLeg), 9, 20, 0x1609, 0, true));
                Add(new GenericBuyInfo(typeof(RawRibs), 16, 20, 0x9F1, 0, true));
                Add(new GenericBuyInfo(typeof(ButcherKnife), 13, 20, 0x13F6, 0));
                Add(new GenericBuyInfo(typeof(Cleaver), 13, 20, 0xEC3, 0));
                Add(new GenericBuyInfo(typeof(SkinningKnife), 13, 20, 0xEC4, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(RawRibs), 8);
                Add(typeof(RawLambLeg), 4);
                Add(typeof(RawChickenLeg), 3);
                Add(typeof(RawBird), 4);
                Add(typeof(Bacon), 3);
                Add(typeof(Sausage), 9);
                Add(typeof(Ham), 13);
                Add(typeof(ButcherKnife), 7);
                Add(typeof(Cleaver), 7);
                Add(typeof(SkinningKnife), 7);
            }
        }
    }
}
