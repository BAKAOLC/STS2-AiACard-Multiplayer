using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2_AiACard_Multiplayer.Content;
using STS2_AiACard_Multiplayer.Patches;
using STS2RitsuLib;
using STS2RitsuLib.Patching.Core;

namespace STS2_AiACard_Multiplayer
{
    [ModInitializer(nameof(Initialize))]
    public static class Main
    {
        public static readonly Logger Logger = RitsuLibFramework.CreateLogger(Const.ModId);

        public static bool IsModActive { get; private set; }

        public static void Initialize()
        {
            if (IsModActive)
            {
                Logger.Debug("Mod already initialized, skipping duplicate initialization.");
                return;
            }

            Logger.Info($"Mod ID: {Const.ModId}");
            Logger.Info($"Version: {Const.Version}");
            Logger.Info("Initializing mod...");

            try
            {
                RitsuLibFramework.EnsureGodotScriptsRegistered(Assembly.GetExecutingAssembly(), Logger);

                var corpsePatcher = RitsuLibFramework.CreatePatcher(Const.ModId, "mp_corpse_speaks", "尸体说话");
                corpsePatcher.RegisterPatch<MpCorpseSpeaksCardModelCanPlayPatch>();
                corpsePatcher.RegisterPatch<MpCorpseSpeaksCardModelIsValidTargetPatch>();
                corpsePatcher.RegisterPatch<MpCorpseSpeaksNTargetManagerAllowedToTargetCreaturePatch>();
                corpsePatcher.RegisterPatch<MpCorpseSpeaksNTargetManagerStartTargetingVector2Patch>();
                corpsePatcher.RegisterPatch<MpCorpseSpeaksNTargetManagerStartTargetingControlPatch>();
                corpsePatcher.RegisterPatch<MpCorpseSpeaksNTargetManagerFinishTargetingPatch>();
                corpsePatcher.RegisterPatch<MpCorpseSpeaksCreatureIsHittablePatch>();
                corpsePatcher.RegisterPatch<MpCreatureCmdKillMarkForceDeathPatch>();
                corpsePatcher.RegisterPatch<MpCombatStartClearForceKillReviveBlockPatch>();
                corpsePatcher.RegisterPatch<MpPlayerDeathSnapshotPatch>();
                corpsePatcher.RegisterPatch<MpPlayerMidCombatRevivePatch>();
                corpsePatcher.RegisterPatch<MpSharedWealthFortuneCanReceivePowersPatch>();

                if (!corpsePatcher.PatchAll())
                {
                    Logger.Error("尸体说话相关 Harmony 补丁未能应用，本 Mod 已禁用。");
                    IsModActive = false;
                    return;
                }

                var autoPlayMoveNext = MpCorpseSpeaksCardCmdAutoPlayMoveNextPatch.TryResolveAutoPlayMoveNext();
                if (autoPlayMoveNext != null)
                    corpsePatcher.ApplyDynamicPatches(
                    [
                        new(
                            $"{Const.ModId}.corpse_speaks.autoplay_move_next",
                            autoPlayMoveNext,
                            transpiler: new(
                                typeof(MpCorpseSpeaksCardCmdAutoPlayMoveNextPatch),
                                nameof(MpCorpseSpeaksCardCmdAutoPlayMoveNextPatch.Transpiler)),
                            isCritical: false,
                            description: "尸体说话 AutoPlay AnyAlly 含尸体"),
                    ]);
                else
                    Logger.Warn("未找到 CardCmd.AutoPlay MoveNext，尸体说话自动选尸未启用。");

                AiACardMultiplayerContentRegistrar.RegisterAll();

                IsModActive = true;
                Logger.Info("Mod initialization complete - Mod is now ACTIVE");
            }
            catch (Exception ex)
            {
                Logger.Error($"Mod initialization failed with exception: {ex.Message}");
                Logger.Error($"Stack trace: {ex.StackTrace}");
                IsModActive = false;
            }
        }
    }
}
