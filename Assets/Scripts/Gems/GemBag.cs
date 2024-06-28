using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GemBag", menuName = "Inventory/GemBag")]
public class GemBag : ScriptableObject
{
    public int maxCapacity;
    public List<GemData> gems = new List<GemData>();
    private string assetFolder = "Assets/ClonedGems";

    public bool AddGem(GemData gem)
    {
        if (gems.Count >= maxCapacity) return false;

#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(assetFolder))
        {
            AssetDatabase.CreateFolder("Assets", "ClonedGems");
        }

        string path = $"{assetFolder}/{gem.gemName}_{System.Guid.NewGuid()}.asset";
        GemData newGem = gem.Clone(path);
        newGem.InitializeRandomValues();
#else
        GemData newGem = Instantiate(gem);
        newGem.InitializeRandomValues();
#endif
        gems.Add(newGem);
        return true;
    }

    public void RemoveGem(GemData gem)
    {
        gems.Remove(gem);
#if UNITY_EDITOR
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(gem));
        AssetDatabase.SaveAssets();
#endif
    }
}
