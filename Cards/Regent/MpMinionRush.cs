using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Regent
{
    /// <summary>仆从出击：所有玩家手牌中的仆从打击变为已升级版本。</summary>
    public sealed class MpMinionRush() : ModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead)
                {
                    continue;
                }

                foreach (var c in MpHelpers.SnapshotHand(p))
                {
                    if (c is not MinionStrike)
                    {
                        continue;
                    }

                    var rep = CombatState.CreateCard<MinionStrike>(p);
                    CardCmd.Upgrade(rep);
                    await CardCmd.Transform(c, rep);
                }
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
