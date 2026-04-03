using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    public sealed class MpDrainOthersDry()
        : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        private decimal _extraHpLossFromPlays;
        private decimal _extraEnergyFromPlays;
        private decimal _extraCardsFromPlays;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(6m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move),
            new HealVar(4m),
            new EnergyVar(1),
            new CardsVar(1),
            new HpLossVar("DrainRampHp", 3m),
            new EnergyVar("DrainRampEnergy", 1),
            new CardsVar("DrainRampCards", 1),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpDrainOthersDry, Const.Paths.CardPortraits.MpDrainOthersDry);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            await MpHelpers.DealHpLoss(choiceContext, target.Creature, DynamicVars.Damage.BaseValue, this);
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, target);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, target);
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);

            var rh = DynamicVars["DrainRampHp"].BaseValue;
            var re = DynamicVars["DrainRampEnergy"].BaseValue;
            var rc = DynamicVars["DrainRampCards"].BaseValue;
            DynamicVars.Damage.BaseValue += rh;
            _extraHpLossFromPlays += rh;
            DynamicVars.Energy.BaseValue += re;
            _extraEnergyFromPlays += re;
            DynamicVars.Cards.BaseValue += rc;
            _extraCardsFromPlays += rc;

            var copy = MpHelpers.CreateCard<MpDrainOthersDry>(CombatState, target, IsUpgraded);
            copy.InheritScaledStatsFrom(this);
            await MpHelpers.AddToHand(choiceContext, copy);
        }

        private void InheritScaledStatsFrom(MpDrainOthersDry source)
        {
            DynamicVars.Damage.BaseValue = source.DynamicVars.Damage.BaseValue;
            DynamicVars.Energy.BaseValue = source.DynamicVars.Energy.BaseValue;
            DynamicVars.Cards.BaseValue = source.DynamicVars.Cards.BaseValue;
            _extraHpLossFromPlays = source._extraHpLossFromPlays;
            _extraEnergyFromPlays = source._extraEnergyFromPlays;
            _extraCardsFromPlays = source._extraCardsFromPlays;
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Energy.UpgradeValueBy(1m);
            DynamicVars.Cards.UpgradeValueBy(1m);
        }

        protected override void AfterDowngraded()
        {
            base.AfterDowngraded();
            DynamicVars.Damage.BaseValue += _extraHpLossFromPlays;
            DynamicVars.Energy.BaseValue += _extraEnergyFromPlays;
            DynamicVars.Cards.BaseValue += _extraCardsFromPlays;
        }
    }
}
