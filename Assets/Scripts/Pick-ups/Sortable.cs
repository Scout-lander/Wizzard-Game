using UnityEngine;

/// <summary>
/// This is a class that can be subclassed by any other class to make the sprites
/// of the class automatically sort themselves by the y-axis.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Sortable : MonoBehaviour
{
    private SpriteRenderer sorted;
    public bool sortingActive = true; // Allows us to deactivate this on certain objects.
    public float minimumDistance = 0.2f; // Minimum distance before the sorting value updates.
    private int lastSortOrder = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        sorted = GetComponent<SpriteRenderer>();
        if (sorted == null)
        {
            Debug.LogError("SpriteRenderer component not found on this GameObject.");
        }
    }

    // Update is called once per frame
    protected virtual void LateUpdate()
    {
        if (sorted == null) return;

        int newSortOrder = (int)(-transform.position.y / minimumDistance);
        if (lastSortOrder != newSortOrder)
        {
            sorted.sortingOrder = newSortOrder;
            lastSortOrder = newSortOrder;
        }
    }
}
