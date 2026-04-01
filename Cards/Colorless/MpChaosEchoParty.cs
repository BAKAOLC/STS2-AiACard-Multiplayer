using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>混沌形态：每名玩家获得能量；各玩家手牌加入多种带虚无的升级形态牌。</summary>
    public sealed class MpChaosEchoParty() : MpOnlyModCardTemplate(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            var energy = IsUpgraded ? 2 : 1;
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                await PlayerCmd.GainEnergy(energy, p);
            }

            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                await AddForm<DemonForm>(choiceContext, p);
                await AddForm<SerpentForm>(choiceContext, p);
                await AddForm<ReaperForm>(choiceContext, p);
                await AddForm<EchoForm>(choiceContext, p);
                await AddForm<VoidForm>(choiceContext, p);
            }
        }

        private async Task AddForm<T>(PlayerChoiceContext ctx, Player p) where T : CardModel
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var c = MpHelpers.CreateCard<T>(CombatState, p, true);
            if (!c.Keywords.Contains(CardKeyword.Ethereal)) CardCmd.ApplyKeyword(c, CardKeyword.Ethereal);

            await MpHelpers.AddToHand(ctx, c);
        }
    }
}
