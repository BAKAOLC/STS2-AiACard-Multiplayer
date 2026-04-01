using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>俺要学猛虎下山：全体受到的强化攻击伤害按层数倍乘（同拦截：敌方回合结束时清除）；施法者获得猛虎流转。</summary>
    public sealed class MpTigerStudyCard() : ModCardTemplate(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(1)];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead)
                {
                    continue;
                }

                await PowerCmd.Apply<MpDoubleDamageTakenPower>(p.Creature, 1, Owner.Creature, this);
            }

            await PowerCmd.Apply<MpTigerStudyFlowPower>(Owner.Creature, DynamicVars.Cards.BaseValue, Owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1m);
        }
    }
}
