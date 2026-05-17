using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    /// <summary>传火：消耗目标全部手牌；每消耗 2 张，目标获得 1 力量、你获得 2（3）临时力量并抽 1 张，双方各将 1 张虚无灼伤放入弃牌堆。</summary>
    public sealed class MpPassTheFlame()
        : MpOnlyModCardTemplate(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpPassTheFlame, Const.Paths.CardPortraits.MpPassTheFlame);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            ModelDb.Power<MpPassTheFlameTemporaryStrengthPower>().HoverTips
                .Concat([HoverTipFactory.FromPower<StrengthPower>()])
                .Concat(HoverTipFactory.FromCardWithCardHoverTips<Burn>());

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var hand = MpHelpers.SnapshotHand(target);
            foreach (var c in hand)
                await CardCmd.Exhaust(choiceContext, c);

            var bundles = hand.Count / 2;
            if (bundles <= 0)
                return;

            var selfStrength = IsUpgraded ? 3 : 2;
            await PowerCmd.Apply<StrengthPower>(target.Creature, bundles, Owner.Creature, this);
            await PowerCmd.Apply<MpPassTheFlameTemporaryStrengthPower>(Owner.Creature,
                bundles * selfStrength, Owner.Creature, this);
            await CardPileCmd.Draw(choiceContext, bundles, Owner);

            for (var i = 0; i < bundles; i++)
            {
                var targetBurn = MpHelpers.CreateCard(CombatState, target, ModelDb.Card<Burn>(), false);
                CardCmd.ApplyKeyword(targetBurn, CardKeyword.Ethereal);
                await MpHelpers.AddGeneratedCardsToCombatPile([targetBurn], PileType.Discard, previewPileAdd: true);

                var ownerBurn = MpHelpers.CreateCard(CombatState, Owner, ModelDb.Card<Burn>(), false);
                CardCmd.ApplyKeyword(ownerBurn, CardKeyword.Ethereal);
                await MpHelpers.AddGeneratedCardsToCombatPile([ownerBurn], PileType.Discard, previewPileAdd: true);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
