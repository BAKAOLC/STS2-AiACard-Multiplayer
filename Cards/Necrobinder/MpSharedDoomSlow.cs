using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_AiACard_Multiplayer.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Necrobinder
{
    /// <summary>一人叠灾厄太慢：获得灾厄；各玩家手牌加入带虚无的死神形态且本回合耗能降为 1。</summary>
    public sealed class MpSharedDoomSlow() : ModCardTemplate(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<DoomPower>(20m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await PowerCmd.Apply<DoomPower>(Owner.Creature, DynamicVars.Doom.BaseValue, Owner.Creature, this);
            foreach (var p in CombatState.Players)
            {
                if (p.Creature.IsDead) continue;

                var wf = CombatState.CreateCard<WraithForm>(p);
                if (IsUpgraded) CardCmd.Upgrade(wf);

                MpHelpers.MakeEtherealEnergyOneThisTurn(wf);
                await MpHelpers.AddToHand(choiceContext, wf);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Doom.UpgradeValueBy(-5m);
        }
    }
}
