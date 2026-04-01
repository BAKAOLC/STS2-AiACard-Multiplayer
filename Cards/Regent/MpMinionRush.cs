using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Regent
{
    /// <summary>仆从出击：目标玩家手牌全部变为仆从打击。</summary>
    public sealed class MpMinionRush() : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            foreach (var c in MpHelpers.SnapshotHand(target).ToList())
            {
                var rep = CombatState.CreateCard<MinionStrike>(target);
                if (IsUpgraded) CardCmd.Upgrade(rep);

                await CardCmd.Transform(c, rep);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
