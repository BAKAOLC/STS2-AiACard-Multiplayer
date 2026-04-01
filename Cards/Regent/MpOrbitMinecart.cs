using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Regent
{
    /// <summary>星际矿车：锻造，然后自动打出一张弹幕。</summary>
    public sealed class MpOrbitMinecart() : ModCardTemplate(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            ArgumentNullException.ThrowIfNull(CombatState);
            await ForgeCmd.Forge(IsUpgraded ? 3m : 2m, Owner, this);
            var barrage = CombatState.CreateCard<Barrage>(Owner);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(barrage);
            }

            await CardCmd.AutoPlay(choiceContext, barrage, cardPlay.Target);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
