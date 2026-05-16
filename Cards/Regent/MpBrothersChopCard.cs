using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Regent
{
    /// <summary>是兄弟就来砍它：全体铸造，且打出君王之剑时他人获得君王之剑。</summary>
    public sealed class MpBrothersChopCard()
        : MpOnlyModCardTemplate(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new ForgeVar(5)];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpBrothersChopCard, Const.Paths.CardPortraits.MpBrothersChopCard);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            HoverTipFactory.FromForge().Concat(ModelDb.Power<MpBrothersBladePower>().HoverTips);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                await ForgeCmd.Forge(DynamicVars.Forge.BaseValue, p, this);
                await PowerCmd.Apply<MpBrothersBladePower>(p.Creature, 1, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
