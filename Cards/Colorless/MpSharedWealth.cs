using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>有福同享：打出时为每名玩家追加本场战斗的额外金币奖励。</summary>
    public sealed class MpSharedWealth() : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new GoldVar(15)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpSharedWealth, Const.Paths.CardPortraits.MpSharedWealth);

        protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            if (Owner.RunState.CurrentRoom is not CombatRoom combatRoom)
                return Task.CompletedTask;

            var amount = DynamicVars.Gold.IntValue;
            foreach (var p in CombatState.Players)
                combatRoom.AddExtraReward(p, new GoldReward(amount, p));

            return Task.CompletedTask;
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Gold.UpgradeValueBy(2m);
        }
    }
}
