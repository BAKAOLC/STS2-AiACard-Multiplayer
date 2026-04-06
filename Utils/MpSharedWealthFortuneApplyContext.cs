namespace STS2_AiACard_Multiplayer.Utils
{
    /// <summary>打出「有福同享」并施加能力期间置为 true：玩家单位在战斗中仍可接收能力（含已死亡），供 CanReceivePowers 补丁读取。</summary>
    internal static class MpSharedWealthFortuneApplyContext
    {
        public static bool AllowFortunePowerOnDeadPlayers { get; set; }
    }
}
