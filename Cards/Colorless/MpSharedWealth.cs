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

            var playerCount = CombatState.Players.Count;
            MpSharedWealthCombatPool.GetOrCreate(CombatState).PendingTotalGold +=
                GoldPerPlayerFactor * playerCount;

            try
            {
                MpSharedWealthFortuneApplyContext.AllowFortunePowerOnDeadPlayers = true;
                foreach (var p in CombatState.Players)
                    await PowerCmd.Apply<MpSharedFortunePower>(p.Creature, 1, Owner.Creature, this);
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
