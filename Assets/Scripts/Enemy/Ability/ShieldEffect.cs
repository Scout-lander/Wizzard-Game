using UnityEngine;
using System.Collections.Generic;  // Import for using List

public class ShieldEffect : MonoBehaviour
{
    public float BlockPercentage; // Additional damage block percentage
    public float checkRadius = 1.0f;  // Set this based on your needs
    private HashSet<GameObject> affectedEnemies = new HashSet<GameObject>();  // Track affected enemies

    private void Start()
    {
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // Rotate the object on the X-axis by -90 degrees
    }

    private void Update()
    {
        Collider2DTDetection();
        RemoveEffectFromOutOfRangeEnemies();
    }

    void Collider2DTDetection()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius);
        HashSet<GameObject> currentEnemies = new HashSet<GameObject>();
        
        foreach (var hit in hits)
        {
            if (hit.gameObject.CompareTag("Enemy"))
            //Debug.Log("Shield is touching an enemy.");
            {
                currentEnemies.Add(hit.gameObject);
                if (!affectedEnemies.Contains(hit.gameObject))
                {
                    EnemyStats enemyStats = hit.gameObject.GetComponent<EnemyStats>();
                    if (enemyStats != null) {
                        enemyStats.ApplyShieldEffect(BlockPercentage);
                        affectedEnemies.Add(hit.gameObject);
                    } else {
                        //Debug.LogError("EnemyStats component not found on the enemy game object!");
                    }
                }
            }
        }
    }

    private void RemoveEffectFromOutOfRangeEnemies()
    {
        // This method could be used if you want to implement additional logic for enemies that leave the radius
    }

    public void Initialize(float damageBlockPercentage) // Correct the parameter name to avoid confusion
    {
        BlockPercentage = damageBlockPercentage;
    }
}
