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
        private int _timesPlayed;

        private int TimesPlayed
        {
            get => _timesPlayed;
            set
            {
                AssertMutable();
                _timesPlayed = value;
            }
        }

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

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpDrainOthersDry, Const.Paths.CardPortraits.MpDrainOthersDry);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            await MpHelpers.DealHpLoss(choiceContext, target.Creature, DynamicVars.Damage.BaseValue, this, cardPlay);

            var targetDied = target.Creature.IsDead;
            if (!targetDied)
            {
                await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, target);
                await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, target);
            }

            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);

            if (!targetDied)
            {
                var transferredCard = MpHelpers.CreateCard<MpDrainOthersDry>(CombatState, target, IsUpgraded);
                transferredCard.SetTimesPlayed(TimesPlayed + 1);
                await MpHelpers.AddToHand(choiceContext, transferredCard);
            }
        }

        protected override CardLocation GetResultLocationForCardPlay()
        {
            return new(Owner, PileType.None, CardPilePosition.Bottom);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Energy.UpgradeValueBy(1m);
            DynamicVars.Cards.UpgradeValueBy(1m);
        }

        protected override void AfterDowngraded()
        {
            base.AfterDowngraded();
            ApplyPlayScaling();
        }

        private void SetTimesPlayed(int timesPlayed)
        {
            TimesPlayed = timesPlayed;
            ApplyPlayScaling();
        }

        private void ApplyPlayScaling()
        {
            DynamicVars.Damage.BaseValue += DynamicVars["DrainRampHp"].BaseValue * TimesPlayed;
            DynamicVars.Energy.BaseValue += DynamicVars["DrainRampEnergy"].BaseValue * TimesPlayed;
            DynamicVars.Cards.BaseValue += DynamicVars["DrainRampCards"].BaseValue * TimesPlayed;
        }
    }
}
