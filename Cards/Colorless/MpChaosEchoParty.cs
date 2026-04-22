using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>混沌形态：每名玩家获得能量；各玩家手牌加入多张带虚无的形态+。</summary>
    public sealed class MpChaosEchoParty() : MpOnlyModCardTemplate(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new EnergyVar(1)];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpChaosEchoParty, Const.Paths.CardPortraits.MpChaosEchoParty);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            new[] { HoverTipFactory.FromKeyword(CardKeyword.Ethereal) }
                .Concat(UpgradedFormHoverTips<DemonForm>())
                .Concat(UpgradedFormHoverTips<SerpentForm>())
                .Concat(UpgradedFormHoverTips<ReaperForm>())
                .Concat(UpgradedFormHoverTips<EchoForm>())
                .Concat(UpgradedFormHoverTips<VoidForm>());

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            var energy = DynamicVars.Energy.IntValue;
            foreach (var p in CombatState.Players.Where(p => p.Creature.IsAlive))
                await PlayerCmd.GainEnergy(energy, p);

            foreach (var p in CombatState.Players.Where(p => p.Creature.IsAlive))
            {
                await AddForm<DemonForm>(choiceContext, p);
                await AddForm<SerpentForm>(choiceContext, p);
                await AddForm<ReaperForm>(choiceContext, p);
                await AddForm<EchoForm>(choiceContext, p);
                await AddForm<VoidForm>(choiceContext, p);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Energy.UpgradeValueBy(1m);
        }

        private async Task AddForm<T>(PlayerChoiceContext ctx, Player p) where T : CardModel
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var c = MpHelpers.CreateCard<T>(CombatState, p, true);
            if (!c.Keywords.Contains(CardKeyword.Ethereal)) CardCmd.ApplyKeyword(c, CardKeyword.Ethereal);

            await MpHelpers.AddToHand(ctx, c);
        }

        private static IEnumerable<IHoverTip> UpgradedFormHoverTips<T>() where T : CardModel
        {
            var c = (CardModel)ModelDb.Card<T>().MutableClone();
            c.UpgradeInternal();
            c.FinalizeUpgradeInternal();
            return new IHoverTip[] { new CardHoverTip(c) }.Concat(c.HoverTips);
        }
    }
}
