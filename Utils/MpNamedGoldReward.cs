using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;

namespace STS2_AiACard_Multiplayer.Utils
{
    internal sealed class MpNamedGoldReward(int amount, Player player, string sourceKey) : GoldReward(amount, player)
    {
        public override LocString Description
        {
            get
            {
                var description = new LocString("powers", sourceKey);
                description.Add("Gold", Amount);
                return description;
            }
        }
    }
}
