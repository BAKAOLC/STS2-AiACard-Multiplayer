using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>死是凉爽的夏夜：叠加灾厄并消灭灾厄层数不低于当前生命的单位。</summary>
    public sealed class MpCoolSummerDoom() : ModCardTemplate(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<DoomPower>(20m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var add = DynamicVars.Doom.BaseValue;
            foreach (var c in CombatState.Creatures.Where(x => x.IsAlive).ToList())
            {
                await PowerCmd.Apply<DoomPower>(c, add, Owner.Creature, this);
            }

            var doomed = CombatState.Creatures
                .Where(c => c.IsAlive && c.GetPower<DoomPower>() is { } d && d.Amount >= c.CurrentHp)
                .ToList();
            if (doomed.Count > 0)
            {
                await DoomPower.DoomKill(doomed);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
