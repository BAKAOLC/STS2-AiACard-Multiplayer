using STS2_AiACard_Multiplayer.Content.Descriptors;
using STS2RitsuLib;

namespace STS2_AiACard_Multiplayer.Content
{
    internal static class AiACardMultiplayerContentRegistrar
    {
        internal static void RegisterAll()
        {
            RitsuLibFramework.CreateContentPack(Const.ModId)
                .ContentManifest(AiACardMultiplayerContentManifest.ContentEntries)
                .Apply();
        }
    }
}
