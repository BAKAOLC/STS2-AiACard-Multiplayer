using STS2RitsuLib.Scaffolding.Content;

namespace STS2_AiACard_Multiplayer
{
    public static class Const
    {
        /// <summary>
        ///     与原版 <c>CardPileCmd.Draw</c> / <c>Dredge</c> 一致的单场战斗手牌张数上限。
        /// </summary>
        public const int CombatHandMax = 10;

        public const string ModId = "STS2-AiACard-Multiplayer";
        public const string Name = "AiACard-Multiplayer";
        public const string Version = "0.1.3";

        public static readonly CardAssetProfile PlaceholderCardArt =
            new(Paths.PlaceholderPortrait, Paths.PlaceholderPortrait);

        public static readonly PowerAssetProfile PlaceholderPowerIcon =
            new(Paths.PlaceholderPortrait, Paths.PlaceholderPortrait);

        public static class Paths
        {
            public const string Root = "res://STS2_AiACard_Multiplayer";
            public const string PlaceholderPortrait = "res://icon.svg";

            /// <summary>卡面 <c>STS2_AiACard_Multiplayer/cards/</c>，文件名与简中牌名一致（含编号前缀）。</summary>
            public static class CardPortraits
            {
                private const string C = Root + "/cards/";

                public const string MpYourBodySlam = C + "001_你来撞击.png";
                public const string MpPassTheFlame = C + "002_传火.png";
                public const string MpDrainOthersDry = C + "003_放干诸血.png";
                public const string MpEveryoneRedraw = C + "004_大伙都区了.png";
                public const string MpTooMuchTalk = C + "005_你话太多了.png";
                public const string MpSerpentSaysStrong = C + "006_我说蛇咬很强.png";
                public const string MpDeployInfiniteBlades = C + "007_开把刀出来.png";
                public const string MpNoCardsNoEnergy = C + "008_你没牌，我没费.png";
                public const string MpSharedStarWall = C + "009_辉星同享.png";
                public const string MpBrothersChopCard = C + "010_是兄弟就来砍他.png";
                public const string MpMinionRush = C + "011_仆从出击.png";
                public const string MpOrbitMinecart = C + "012_星际矿车.png";
                public const string MpGroupSacrifice = C + "013_群体献祭.png";
                public const string MpBloodMistCard = C + "014_血雾弥漫.png";
                public const string MpCorpseSpeaks = C + "015_尸体说话.png";
                public const string MpSharedDoomSlow = C + "016_一人叠灾厄太慢.png";
                public const string MpShockGift = C + "017_有没有电摸一下.png";
                public const string MpStatusVacuum = C + "018_不要就给我.png";
                public const string MpBrainDock = C + "019_外接大脑.png";
                public const string MpBiasedPartyCard = C + "020_你们都有偏差认知.png";
                public const string MpCheckYourDpsCard = C + "021_查你DPS.png";
                public const string MpWriggle = C + "022_蠕动.png";
                public const string MpAllOnMe = C + "023_全靠我了.png";
                public const string MpTigerStudyCard = C + "024_俺要学猛虎下山.png";
                public const string MpSerpentBrothersCard = C + "025_群蛇兄弟.png";
                public const string MpGamblingCard = C + "026_赌怪.png";
                public const string MpFourWardBrothers = C + "027_四区兄弟.png";
                public const string MpSharedWealth = C + "028_有福同享.png";
                public const string MpCoolSummerDoom = C + "029_死是凉爽的夏夜.png";
                public const string MpChaosEchoParty = C + "030_混沌形态.png";
                public const string MpCorpseCurseToken = C + "031_尸语诅咒占位.png";
            }

            public static class PowerIcons
            {
                private const string P = Root + "/powers/";

                public const string MpBountyMarkPower = P + "001_赏金标记.png";
                public const string MpBrothersBladePower = P + "002_兄弟借剑.png";
                public const string MpCheckDpsPower = P + "003_查询DPS.png";
                public const string MpDoubleDamageTakenPower = P + "004_承伤倍增.png";
                public const string MpHangoverPower = P + "005_力竭.png";
                public const string MpLoseEnergyNextTurnPower = P + "006_下回合失能.png";
                public const string MpPerEnergySelfChannelPower = P + "007_耗能偏差.png";
                public const string MpSerpentBrothersPower = P + "008_群蛇兄弟.png";
                public const string MpSharedFortunePower = P + "009_有福同享.png";
                public const string MpTigerStudyFlowPower = P + "010_猛虎流转.png";
                public const string MpPassTheFlameTemporaryStrengthPower = P + "011_传火.png";
            }
        }
    }
}
