using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2_AiACard_Multiplayer.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>「有福同享」：打出后为每名玩家在本场战斗追加一笔额外金币奖励，并为每名玩家施加有福同享能力以显示对应数额。</summary>
    public sealed class MpSharedWealth() : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<MpSharedFortunePower>(15)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpSharedWealth, Const.Paths.CardPortraits.MpSharedWealth);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            ModelDb.Power<MpSharedFortunePower>().HoverTips;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            if (Owner.RunState.CurrentRoom is not CombatRoom combatRoom)
                return;

            var amount = (int)DynamicVars["MpSharedFortunePower"].BaseValue;
            foreach (var p in CombatState.Players)
                combatRoom.AddExtraReward(p, new GoldReward(amount, p));

            try
            {
                MpSharedWealthFortuneApplyContext.AllowFortunePowerOnDeadPlayers = true;
                foreach (var p in CombatState.Players)
                    await PowerCmd.Apply<MpSharedFortunePower>(p.Creature, amount, Owner.Creature, this);
            }
            finally
            {
                MpSharedWealthFortuneApplyContext.AllowFortunePowerOnDeadPlayers = false;
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["MpSharedFortunePower"].UpgradeValueBy(2m);
        }
    }
}
