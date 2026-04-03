using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer.Cards.Colorless
{
    public sealed class MpCoolSummerDoom() : MpOnlyModCardTemplate(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<DoomPower>(20m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Const.PlaceholderCardArt;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips => ModelDb.Power<DoomPower>().HoverTips;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            foreach (var side in new[] { CombatSide.Enemy, CombatSide.Player })
            {
                var floor = VfxCmd.GetSideCenterFloor(side, CombatState);
                if (!floor.HasValue) continue;

                var vfx = NLargeMagicMissileVfx.Create(floor.Value, new("8c2447"));
                if (vfx == null) continue;

                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
                await Cmd.Wait(vfx.WaitTime);
            }

            var targets = CombatState.Creatures.Where(c => c.IsAlive).ToList();
            var add = DynamicVars.Doom.BaseValue;
            foreach (var c in targets)
                await PowerCmd.Apply<DoomPower>(c, add, Owner.Creature, this);

            await DoomPower.DoomKill(DoomPower.GetDoomedCreatures(targets));
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
