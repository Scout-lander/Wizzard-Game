using UnityEngine;

public static class GemMergeRarityCalculator
{
    public static float[] CalculateRarityProbabilities(GemData[] gems)
    {
        float commonProb = 0f;
        float uncommonProb = 0f;
        float rareProb = 0f;
        float epicProb = 0f;
        float legendaryProb = 0f;
        float mithicProb = 0f;

        int filledSlots = 0;
        foreach (var gem in gems)
        {
            if (gem != null)
            {
                switch (gem.rarity)
                {
                    case GemRarity.Common:
                        commonProb += 0.4f;
                        uncommonProb += 0.3f;
                        rareProb += 0.2f;
                        epicProb += 0.07f;
                        legendaryProb += 0.03f;
                        mithicProb += 0.0f;
                        break;
                    case GemRarity.Uncommon:
                        commonProb += 0.20f;
                        uncommonProb += 0.4f;
                        rareProb += 0.25f;
                        epicProb += 0.1f;
                        legendaryProb += 0.05f;
                        mithicProb += 0.0f;
                        break;
                    case GemRarity.Rare:
                        commonProb += 0.5f;
                        uncommonProb += 0.15f;
                        rareProb += 0.5f;
                        epicProb += 0.2f;
                        legendaryProb += 0.1f;
                        mithicProb += 0.0f;
                        break;
                    case GemRarity.Epic:
                        commonProb += 0.3f;
                        uncommonProb += 0.07f;
                        rareProb += 0.2f;
                        epicProb += 0.5f;
                        legendaryProb += 0.2f;
                        mithicProb += 0.0f;
                        break;
                    case GemRarity.Legendary:
                        commonProb += 0.0f;
                        uncommonProb += 0.05f;
                        rareProb += 0.15f;
                        epicProb += 0.25f;
                        legendaryProb += 0.47f;
                        mithicProb += 0.03f;
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
            mithicProb /= filledSlots;

            // Normalize probabilities to sum up to 100%
            float totalProb = commonProb + uncommonProb + rareProb + epicProb + legendaryProb;
            commonProb = (commonProb / totalProb) * 100;
            uncommonProb = (uncommonProb / totalProb) * 100;
            rareProb = (rareProb / totalProb) * 100;
            epicProb = (epicProb / totalProb) * 100;
            legendaryProb = (legendaryProb / totalProb) * 100;
            mithicProb = (mithicProb / totalProb) * 100;
        }

        return new float[] { commonProb, uncommonProb, rareProb, epicProb, legendaryProb, mithicProb };
    }
}
