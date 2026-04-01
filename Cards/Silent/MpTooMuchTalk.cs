using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Silent
{
    /// <summary>你话太多了：目标玩家抽牌并获得虚弱。</summary>
    public sealed class MpTooMuchTalk() : ModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
    {
        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var target = MpHelpers.RequireTargetPlayer(cardPlay);
            await CardPileCmd.Draw(choiceContext, IsUpgraded ? 5 : 4, target);
            await PowerCmd.Apply<WeakPower>(target.Creature, IsUpgraded ? 3 : 2, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
