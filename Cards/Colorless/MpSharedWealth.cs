using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>有福同享：战斗结束时每名玩家获得金币。</summary>
    public sealed class MpSharedWealth() : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<MpSharedFortunePower>(15)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            ModelDb.Power<MpSharedFortunePower>().HoverTips;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var gold = DynamicVars["MpSharedFortunePower"].BaseValue;
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                await PowerCmd.Apply<MpSharedFortunePower>(p.Creature, gold, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["MpSharedFortunePower"].UpgradeValueBy(2m);
        }
    }
}
