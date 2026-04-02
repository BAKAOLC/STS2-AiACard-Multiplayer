using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Necrobinder
{
    /// <summary>尸体说话：复活或治疗队友，为其供能与抽牌，并在其手牌置入回合末爆发伤害。</summary>
    public sealed class MpCorpseSpeaks() : MpOnlyModCardTemplate(3, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new HealVar(5m),
            new EnergyVar(2),
            new CardsVar(3),
        ];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<MpCorpseCurseToken>(false);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            var heal = DynamicVars.Heal.BaseValue;
            if (target.Creature.IsDead)
                await CreatureCmd.SetCurrentHp(target.Creature, heal);
            else
                await CreatureCmd.Heal(target.Creature, heal);

            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, target);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, target);
            var curse = MpHelpers.CreateCard<MpCorpseCurseToken>(CombatState, target, false);
            await MpHelpers.AddToHand(choiceContext, curse);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Heal.UpgradeValueBy(5m);
            DynamicVars.Energy.UpgradeValueBy(1m);
            DynamicVars.Cards.UpgradeValueBy(2m);
        }
    }
}
