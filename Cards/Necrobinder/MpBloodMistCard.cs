using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Necrobinder
{
    /// <summary>血雾弥漫：你与另一名玩家各将一张血肉戏法置入手牌。</summary>
    public sealed class MpBloodMistCard()
        : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);

            var yours = MpHelpers.CreateCard<SleightOfFlesh>(CombatState, Owner, IsUpgraded);
            await MpHelpers.AddToHand(choiceContext, yours);

            var theirs = MpHelpers.CreateCard<SleightOfFlesh>(CombatState, target, IsUpgraded);
            await MpHelpers.AddToHand(choiceContext, theirs);
        }
    }
}
