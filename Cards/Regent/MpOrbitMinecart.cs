using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Regent
{
    /// <summary>星际矿车：你获得能量，其他玩家失去能量，环绕轨道加入各人手牌。</summary>
    public sealed class MpOrbitMinecart()
        : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(2),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            foreach (var p in CombatState.Players.Where(p => p != Owner && !p.Creature.IsDead))
                await PlayerCmd.LoseEnergy(1, p);

            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                var orbit = CombatState.CreateCard<Orbit>(p);
                if (IsUpgraded) CardCmd.Upgrade(orbit);

                await MpHelpers.AddToHand(choiceContext, orbit);
            }
        }
    }
}
