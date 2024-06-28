using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


public enum GemRarity { Common, Uncommon, Rare, Epic, Legendary }

public class GemData : ScriptableObject
{
    public Sprite icon;
    public string gemName;
    public string description;
    public GemRarity rarity;
    public bool isInitialized = false;

    public virtual void InitializeRandomValues() { }

#if UNITY_EDITOR
    public virtual GemData Clone(string path)
    {
        GemData clone = Instantiate(this);
        clone.isInitialized = false; // Reset initialization flag for the clone
        clone.InitializeRandomValues();
        AssetDatabase.CreateAsset(clone, path);
        AssetDatabase.SaveAssets();
        return clone;
    }
#endif
}
