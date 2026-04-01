using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Necrobinder
{
    /// <summary>血雾弥漫：你与目标各获得血肉戏法。</summary>
    public sealed class MpBloodMistCard()
        : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<SleightOfFleshPower>(9m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var amt = DynamicVars["SleightOfFleshPower"].BaseValue;
            await PowerCmd.Apply<SleightOfFleshPower>(Owner.Creature, amt, Owner.Creature, this);
            await PowerCmd.Apply<SleightOfFleshPower>(target.Creature, amt, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["SleightOfFleshPower"].UpgradeValueBy(4m);
        }
    }
}
