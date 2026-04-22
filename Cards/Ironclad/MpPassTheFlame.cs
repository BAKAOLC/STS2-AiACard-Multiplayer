using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    /// <summary>传火：消耗目标手牌；每消耗一张，在目标弃牌堆加入 1 张带虚无的灼伤；你获得 1 临时力量并抽 1 张牌。</summary>
    public sealed class MpPassTheFlame()
        : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpPassTheFlame, Const.Paths.CardPortraits.MpPassTheFlame);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            ModelDb.Power<MpPassTheFlameTemporaryStrengthPower>().HoverTips.Concat(ModelDb.Card<Burn>().HoverTips);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var hand = MpHelpers.SnapshotHand(target);
            foreach (var c in hand) await CardCmd.Exhaust(choiceContext, c);

            var n = hand.Count;
            if (n > 0)
            {
                var burns = Enumerable.Range(0, n)
                    .Select(_ =>
                    {
                        var burn = MpHelpers.CreateCard(CombatState, target, ModelDb.Card<Burn>(), false);
                        CardCmd.ApplyKeyword(burn, CardKeyword.Ethereal);
                        return burn;
                    })
                    .ToList();
                await MpHelpers.AddGeneratedCardsToCombatPile(burns, PileType.Discard, previewPileAdd: true);

                await PowerCmd.Apply<MpPassTheFlameTemporaryStrengthPower>(Owner.Creature, n, Owner.Creature, this);
                await CardPileCmd.Draw(choiceContext, n, Owner);
            }
        }

        protected override void OnUpgrade()
        {
        }
    }
}
