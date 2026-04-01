using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>全靠我了：吸取所有其他玩家的能量归己有，下回合受到沉重代价。</summary>
    public sealed class MpAllOnMe() : ModCardTemplate(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var total = 0;
            foreach (var p in CombatState.Players.Where(x => x != Owner && !x.Creature.IsDead))
            {
                var e = p.PlayerCombatState?.Energy ?? 0;
                if (e <= 0)
                {
                    continue;
                }

                await PlayerCmd.LoseEnergy(e, p);
                total += e;
            }

            if (total > 0)
            {
                await PlayerCmd.GainEnergy(total, Owner);
            }

            await PowerCmd.Apply<MpHangoverPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
