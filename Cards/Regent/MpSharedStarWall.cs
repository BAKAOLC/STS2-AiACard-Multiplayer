using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Regent
{
    /// <summary>辉星同享：按 X 分配辉星，各玩家获得粒子障壁与星位序列。</summary>
    public sealed class MpSharedStarWall()
        : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        public override bool HasStarCostX => true;

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var x = ResolveStarXValue();
            var half = x / 2;
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                if (half > 0) await PlayerCmd.GainStars(half, p);

                var wall = MpHelpers.CreateCard<ParticleWall>(CombatState, p, IsUpgraded);
                await MpHelpers.AddToHand(choiceContext, wall);
                var align = MpHelpers.CreateCard<Alignment>(CombatState, p, IsUpgraded);
                await MpHelpers.AddToHand(choiceContext, align);
            }
        }
    }
}
