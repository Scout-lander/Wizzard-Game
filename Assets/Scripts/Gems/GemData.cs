using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GemRarity { Common, Uncommon, Rare, Epic, Legendary, Mithic }

public class GemData : ScriptableObject
{
    public Sprite icon;
    public string gemName;
    public string description;
    public GemRarity rarity;
    public bool isInitialized = false;

    [Header("Materials")]
    public Material epicMaterial; // Material for Epic gems
    public Material legendaryMaterial; // Material for Legendary gems
    public Material mithicMaterial; // Material for Mithic gems

    [Header("Probabilities")]
    public float commonProbabilities = 0.5f;
    public float uncommonProbabilities = 0.3f;
    public float rareProbabilities = 0.1f;
    public float epicProbabilities = 0.07f;
    public float legendaryProbabilities = 0.03f;
    public float mithicProbabilities = 0.00f;

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

    public GemData Clone()
    {
        GemData clone = Instantiate(this);
        clone.isInitialized = false;
        clone.InitializeRandomValues();
        return clone;
    }
}
