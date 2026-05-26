using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    public sealed class MpSharedWealth() : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public const int GoldPerPlayerFactor = 25;

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpSharedWealth, Const.Paths.CardPortraits.MpSharedWealth);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            ModelDb.Power<MpSharedFortunePower>().HoverTips;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);

            var players = CombatState.Players.ToList();
            var shares = MpSharedWealthGoldDistribution.Distribute(
                GoldPerPlayerFactor * players.Count, players.Count, 1, Owner.RunState.Rng.Shuffle);

            try
            {
                MpSharedWealthFortuneApplyContext.AllowFortunePowerOnDeadPlayers = true;
                for (var i = 0; i < players.Count; i++)
                    await PowerCmd.Apply<MpSharedFortunePower>(players[i].Creature, shares[i], Owner.Creature, this);
            }
            finally
            {
                MpSharedWealthFortuneApplyContext.AllowFortunePowerOnDeadPlayers = false;
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
