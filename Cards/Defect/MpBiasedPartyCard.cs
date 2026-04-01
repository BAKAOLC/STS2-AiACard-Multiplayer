using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Defect
{
    /// <summary>你们都有认知偏差：其他玩家获得按耗能自充能球的效果。</summary>
    public sealed class MpBiasedPartyCard() : ModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CalculationBaseVar(1m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var orbsPerEnergy = DynamicVars.CalculationBase.BaseValue;
            foreach (var p in CombatState.Players.Where(p => p != Owner && !p.Creature.IsDead))
            {
                await PowerCmd.Apply<MpPerEnergySelfChannelPower>(p.Creature, orbsPerEnergy, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.CalculationBase.UpgradeValueBy(1m);
        }
    }
}
