using MegaCrit.Sts2.Core.Random;

namespace STS2_AiACard_Multiplayer.Utils
{
    internal static class MpSharedWealthGoldDistribution
    {
        public static int[] Distribute(int totalGold, int playerCount, int minEach, Rng rng)
        {
            if (playerCount <= 0)
                return [];

            if (totalGold <= 0)
                return new int[playerCount];

            var shares = new int[playerCount];
            for (var i = 0; i < playerCount; i++)
                shares[i] = minEach;

            var remaining = totalGold - minEach * playerCount;
            if (remaining < 0)
            {
                Array.Fill(shares, 0);
                for (var i = 0; i < totalGold; i++)
                    shares[rng.NextInt(playerCount)]++;
                return shares;
            }

            for (var i = 0; i < remaining; i++)
                shares[rng.NextInt(playerCount)]++;

            return shares;
        }
    }
}
