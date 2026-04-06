using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Defect
{
    /// <summary>有没有电摸一下：随机充能球、高速脱离与眩晕。</summary>
    public sealed class MpShockGift() : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Common, TargetType.AnyAlly)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(2)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpShockGift, Const.Paths.CardPortraits.MpShockGift);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<BoostAway>(IsUpgraded)
                .Concat(HoverTipFactory.FromCardWithCardHoverTips<Dazed>());

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var ctx = new ThrowingPlayerChoiceContext();
            var orb = OrbModel.GetRandomOrb(target.RunState.Rng.CombatOrbGeneration).ToMutable();
            await OrbCmd.Channel(ctx, orb, target);
            var boostAway = MpHelpers.CreateCard<BoostAway>(CombatState, target, IsUpgraded);
            await MpHelpers.AddToHand(choiceContext, boostAway);
            var dazedCount = DynamicVars.Cards.IntValue;
            var dazedCards = new List<Dazed>(dazedCount);
            for (var i = 0; i < dazedCount; i++)
                dazedCards.Add(MpHelpers.CreateCard<Dazed>(CombatState, target, false));
            await MpHelpers.AddGeneratedCardsToCombatPile(dazedCards, PileType.Draw, CardPilePosition.Random,
                true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(-1m);
        }
    }
}
