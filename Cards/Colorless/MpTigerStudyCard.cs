using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    public sealed class MpTigerStudyCard()
        : MpOnlyModCardTemplate(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<MpDoubleDamageTakenPower>(1),
            new PowerVar<MpTigerStudyFlowPower>(1),
        ];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpTigerStudyCard, Const.Paths.CardPortraits.MpTigerStudyCard);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            ModelDb.Power<MpDoubleDamageTakenPower>().HoverTips
                .Concat(ModelDb.Power<MpTigerStudyFlowPower>().HoverTips);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            var dmgMul = DynamicVars["MpDoubleDamageTakenPower"].BaseValue;
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                await PowerCmd.Apply<MpDoubleDamageTakenPower>(p.Creature, dmgMul, Owner.Creature, this);
            }

            await PowerCmd.Apply<MpTigerStudyFlowPower>(Owner.Creature,
                DynamicVars["MpTigerStudyFlowPower"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["MpTigerStudyFlowPower"].UpgradeValueBy(1m);
        }
    }
}
