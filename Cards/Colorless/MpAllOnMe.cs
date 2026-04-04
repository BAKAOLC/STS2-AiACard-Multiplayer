using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_AiACard_Multiplayer.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    /// <summary>全靠我了：吸取队友能量，下回合力竭。</summary>
    public sealed class MpAllOnMe() : MpOnlyModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<MpHangoverPower>(1)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile =>
            new(Const.Paths.CardPortraits.MpAllOnMe, Const.Paths.CardPortraits.MpAllOnMe);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips => ModelDb.Power<MpHangoverPower>().HoverTips;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var total = 0;
            foreach (var p in CombatState.Players.Where(x => x != Owner && !x.Creature.IsDead))
            {
                var e = p.PlayerCombatState?.Energy ?? 0;
                if (e <= 0) continue;

                await PlayerCmd.LoseEnergy(e, p);
                total += e;
            }

            if (total > 0) await PlayerCmd.GainEnergy(total, Owner);

            await PowerCmd.Apply<MpHangoverPower>(Owner.Creature, DynamicVars["MpHangoverPower"].BaseValue,
                Owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
