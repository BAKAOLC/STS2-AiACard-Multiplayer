using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Necrobinder
{
    /// <summary>群体献祭：全体灾厄、能量与弃抽，本回合攻击伤害翻倍。</summary>
    public sealed class MpGroupSacrifice() : ModCardTemplate(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
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

                await PowerCmd.Apply<DoomPower>(p.Creature, 99m, Owner.Creature, this);
                await PlayerCmd.GainEnergy(3, p);
                foreach (var c in MpHelpers.SnapshotHand(p).ToList())
                {
                    await CardCmd.Discard(choiceContext, c);
                }

                await CardPileCmd.Draw(choiceContext, 10, p);
                await PowerCmd.Apply<DoubleDamagePower>(p.Creature, 1, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}
