using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_AiACard_Multiplayer.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>群蛇兄弟：手牌加入蛇咬，且所有玩家获得“打出蛇咬时自身获得格挡”。</summary>
    public sealed class MpSerpentBrothersCard()
        : MpOnlyModCardTemplate(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new BlockVar(3m, ValueProp.Unpowered)];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpSerpentBrothersCard, Const.Paths.CardPortraits.MpSerpentBrothersCard);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromCardWithCardHoverTips<Snakebite>()
                .Concat(ModelDb.Power<MpSerpentBrothersPower>().HoverTips);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                var bite = MpHelpers.CreateCard<Snakebite>(CombatState, p, false);
                await MpHelpers.AddToHand(choiceContext, bite);
            }

            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                await PowerCmd.Apply<MpSerpentBrothersPower>(p.Creature, DynamicVars.Block.BaseValue, Owner.Creature,
                    this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2m);
        }
    }
}
