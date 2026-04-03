using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Regent
{
    /// <summary>星际矿车：[消耗]。每名玩家将一张（升级后为升级）环绕轨道置入手牌。</summary>
    public sealed class MpOrbitMinecart()
        : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpOrbitMinecart, Const.Paths.CardPortraits.MpOrbitMinecart);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<Orbit>(IsUpgraded);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                var orbit = MpHelpers.CreateCard<Orbit>(CombatState, p, IsUpgraded);
                await MpHelpers.AddToHand(choiceContext, orbit);
            }
        }
    }
}
