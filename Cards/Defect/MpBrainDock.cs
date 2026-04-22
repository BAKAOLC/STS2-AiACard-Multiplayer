using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Defect
{
    /// <summary>外接大脑：每名玩家将一张创造性AI置入手牌（升级后为升级版）。</summary>
    public sealed class MpBrainDock() : MpOnlyModCardTemplate(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpBrainDock, Const.Paths.CardPortraits.MpBrainDock);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<CreativeAi>(IsUpgraded);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                var ai = MpHelpers.CreateCard<CreativeAi>(CombatState, p, IsUpgraded);
                await MpHelpers.AddToHand(choiceContext, ai);
            }
        }
    }
}
