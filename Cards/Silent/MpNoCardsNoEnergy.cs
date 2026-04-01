using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Silent
{
    /// <summary>你没牌我没费：你抽牌，每名其他玩家失去能量。</summary>
    public sealed class MpNoCardsNoEnergy() : ModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await CardPileCmd.Draw(choiceContext, IsUpgraded ? 4 : 3, Owner);
            foreach (var p in CombatState.Players.Where(x => x != Owner))
            {
                await PlayerCmd.LoseEnergy(IsUpgraded ? 2 : 1, p);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
