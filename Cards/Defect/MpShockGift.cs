using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Orbs;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Defect
{
    /// <summary>有没有电摸一下：为目标玩家生成闪电球位。</summary>
    public sealed class MpShockGift() : ModCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var n = IsUpgraded ? 3 : 2;
            for (var i = 0; i < n; i++)
            {
                await OrbCmd.Channel<LightningOrb>(choiceContext, target);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
