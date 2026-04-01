using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Ironclad
{
    /// <summary>大伙都区了：消耗所有玩家手牌，再各抽等量张牌。</summary>
    public sealed class MpEveryoneRedraw() : ModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var counts = CombatState.Players.ToDictionary(p => p, p => MpHelpers.SnapshotHand(p).Count);
            foreach (var p in CombatState.Players)
            foreach (var c in MpHelpers.SnapshotHand(p))
                await CardCmd.Exhaust(choiceContext, c);

            foreach (var kv in counts) await CardPileCmd.Draw(choiceContext, kv.Value, kv.Key);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
