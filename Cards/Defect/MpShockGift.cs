using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Defect
{
    /// <summary>有没有电摸一下：随机充能球、内核加速与眩晕。</summary>
    public sealed class MpShockGift() : ModCardTemplate(0, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(2)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var ctx = new ThrowingPlayerChoiceContext();
            var orb = OrbModel.GetRandomOrb(target.RunState.Rng.CombatOrbGeneration).ToMutable();
            await OrbCmd.Channel(ctx, orb, target);
            var boot = CombatState.CreateCard<BootSequence>(target);
            if (IsUpgraded) CardCmd.Upgrade(boot);

            await MpHelpers.AddToHand(choiceContext, boot);
            var dazedCount = DynamicVars.Cards.IntValue;
            for (var i = 0; i < dazedCount; i++)
            {
                var dazed = CombatState.CreateCard<Dazed>(target);
                await CardPileCmd.Add(dazed, PileType.Deck, CardPilePosition.Random);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(-1m);
        }
    }
}
