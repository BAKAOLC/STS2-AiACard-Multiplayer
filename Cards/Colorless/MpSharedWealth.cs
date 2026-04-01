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
    /// <summary>有福同享：战斗结束时每名玩家获得金币。</summary>
    public sealed class MpSharedWealth() : ModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new GoldVar(15)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead)
                {
                    continue;
                }

                await PowerCmd.Apply<MpSharedFortunePower>(p.Creature, DynamicVars.Gold.BaseValue, Owner.Creature,
                    this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Gold.UpgradeValueBy(2m);
        }
    }
}
