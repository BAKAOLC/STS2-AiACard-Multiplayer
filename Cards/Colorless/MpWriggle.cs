using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>蠕动：按回合数为所有玩家提供能量与抽牌。</summary>
    public sealed class MpWriggle() : MpOnlyModCardTemplate(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpWriggle, Const.Paths.CardPortraits.MpWriggle);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var n = CombatState.RoundNumber / 3;
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                if (n > 0)
                {
                    await PlayerCmd.GainEnergy(n, p);
                    await CardPileCmd.Draw(choiceContext, n, p);
                }
            }
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}
