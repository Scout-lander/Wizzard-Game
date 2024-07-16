using System.Collections.Generic;
using UnityEngine;

public static class RuneMergeRarityCalculator
{
    public static float[] CalculateRarityProbabilities(Rune[] runes)
    {
        float commonProb = 0f;
        float uncommonProb = 0f;
        float rareProb = 0f;
        float epicProb = 0f;
        float legendaryProb = 0f;
        float mythicProb = 0f;

        int filledSlots = 0;
        foreach (var rune in runes)
        {
            if (rune != null)
            {
                switch (rune.rarity)
                {
                    case RuneRarity.Common:
                        commonProb += 0.4f;
                        uncommonProb += 0.3f;
                        rareProb += 0.2f;
                        epicProb += 0.07f;
                        legendaryProb += 0.03f;
                        mythicProb += 0.0f;
                        break;
                    case RuneRarity.Uncommon:
                        commonProb += 0.20f;
                        uncommonProb += 0.4f;
                        rareProb += 0.25f;
                        epicProb += 0.1f;
                        legendaryProb += 0.05f;
                        mythicProb += 0.0f;
                        break;
                    case RuneRarity.Rare:
                        commonProb += 0.5f;
                        uncommonProb += 0.15f;
                        rareProb += 0.5f;
                        epicProb += 0.2f;
                        legendaryProb += 0.1f;
                        mythicProb += 0.0f;
                        break;
                    case RuneRarity.Epic:
                        commonProb += 0.3f;
                        uncommonProb += 0.07f;
                        rareProb += 0.2f;
                        epicProb += 0.5f;
                        legendaryProb += 0.2f;
                        mythicProb += 0.0f;
                        break;
                    case RuneRarity.Legendary:
                        commonProb += 0.0f;
                        uncommonProb += 0.05f;
                        rareProb += 0.15f;
                        epicProb += 0.25f;
                        legendaryProb += 0.47f;
                        mythicProb += 0.03f;
                        break;
                    case RuneRarity.Mythic:
                        commonProb += 0.0f;
                        uncommonProb += 0.0f;
                        rareProb += 0.0f;
                        epicProb += 0.1f;
                        legendaryProb += 0.2f;
                        mythicProb += 0.7f;
                        break;
                }
                filledSlots++;
            }
        }

        if (filledSlots > 0)
        {
            // Calculate average probabilities
            commonProb /= filledSlots;
            uncommonProb /= filledSlots;
            rareProb /= filledSlots;
            epicProb /= filledSlots;
            legendaryProb /= filledSlots;
            mythicProb /= filledSlots;

            // Normalize probabilities to sum up to 100%
            float totalProb = commonProb + uncommonProb + rareProb + epicProb + legendaryProb + mythicProb;
            commonProb = (commonProb / totalProb) * 100;
            uncommonProb = (uncommonProb / totalProb) * 100;
            rareProb = (rareProb / totalProb) * 100;
            epicProb = (epicProb / totalProb) * 100;
            legendaryProb = (legendaryProb / totalProb) * 100;
            mythicProb = (mythicProb / totalProb) * 100;
        }

        return new float[] { commonProb, uncommonProb, rareProb, epicProb, legendaryProb, mythicProb };
    }
}
