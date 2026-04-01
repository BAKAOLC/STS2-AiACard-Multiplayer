using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    /// <summary>传火：消耗目标手牌；每消耗一张，你获得 1 力量并抽 1 张牌。</summary>
    public sealed class MpPassTheFlame()
        : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.AnyAlly)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var hand = MpHelpers.SnapshotHand(target);
            foreach (var c in hand) await CardCmd.Exhaust(choiceContext, c);

            foreach (var _ in hand)
            {
                await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, this);
                await CardPileCmd.Draw(choiceContext, 1, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
