using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Silent
{
    /// <summary>你没牌我没费：将杂技放入目标手牌，你获得能量。</summary>
    public sealed class MpNoCardsNoEnergy()
        : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new EnergyVar(2)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var acro = CombatState.CreateCard<Acrobatics>(target);
            if (IsUpgraded) CardCmd.Upgrade(acro);

            await MpHelpers.AddToHand(choiceContext, acro);
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Energy.UpgradeValueBy(1m);
        }
    }
}
