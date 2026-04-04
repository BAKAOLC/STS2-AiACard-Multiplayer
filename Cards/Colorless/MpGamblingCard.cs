using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>赌怪：敌人获得力量与悬赏标记。</summary>
    public sealed class MpGamblingCard() : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<StrengthPower>(2),
            new PowerVar<RitualPower>(2),
            new("BountyGoldCap", 50m),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpGamblingCard, Const.Paths.CardPortraits.MpGamblingCard);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            ModelDb.Power<StrengthPower>().HoverTips
                .Concat(ModelDb.Power<RitualPower>().HoverTips)
                .Concat(ModelDb.Power<MpBountyMarkPower>().HoverTips);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            var str = DynamicVars["StrengthPower"].BaseValue;
            var rit = DynamicVars["RitualPower"].BaseValue;
            foreach (var e in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<StrengthPower>(e, str, Owner.Creature, this);
                await PowerCmd.Apply<RitualPower>(e, rit, Owner.Creature, this);
                await PowerCmd.Apply<MpBountyMarkPower>(e, 1, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["BountyGoldCap"].UpgradeValueBy(25m);
        }
    }
}
