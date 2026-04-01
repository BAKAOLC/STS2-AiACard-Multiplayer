using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Silent
{
    /// <summary>开把刀出来：为每名玩家打出无尽刀刃，并将幻影之刃置入手牌。</summary>
    public sealed class MpDeployInfiniteBlades()
        : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                var ib = CombatState.CreateCard<InfiniteBlades>(p);
                if (IsUpgraded) CardCmd.Upgrade(ib);

                await CardCmd.AutoPlay(choiceContext, ib, null);
                var phantom = CombatState.CreateCard<PhantomBlades>(p);
                if (IsUpgraded) CardCmd.Upgrade(phantom);

                await MpHelpers.AddToHand(choiceContext, phantom);
            }
        }
    }
}
