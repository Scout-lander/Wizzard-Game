using UnityEngine;
using System.Collections.Generic;

public class ShieldEffect : MonoBehaviour
{
    public float BlockPercentage; // Additional damage block percentage
    public float checkRadius = .13f;  // Set this based on your needs
    private HashSet<GameObject> affectedEnemies = new HashSet<GameObject>();  // Track affected enemies

    private void Start()
    {
        //transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // Rotate the object on the X-axis by -90 degrees
    }

    private void Update()
    {
        Collider2DTDetection();
    }

    void Collider2DTDetection()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius);
        HashSet<GameObject> currentEnemies = new HashSet<GameObject>();

        foreach (var hit in hits)
        {
            if (hit.gameObject.CompareTag("Enemy"))
            {
                currentEnemies.Add(hit.gameObject);
                if (!affectedEnemies.Contains(hit.gameObject))
                {
                    EnemyStats enemyStats = hit.gameObject.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.ApplyShieldEffect(BlockPercentage);
                        affectedEnemies.Add(hit.gameObject);
                    }
                }
            }
        }

        foreach (var enemy in affectedEnemies)
        {
            if (!currentEnemies.Contains(enemy))
            {
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.RemoveShieldEffect();
                }
            }
        }

        affectedEnemies = currentEnemies;
    }

    public void Initialize(float damageBlockPercentage) // Correct the parameter name to avoid confusion
    {
        BlockPercentage = damageBlockPercentage;
    }
}
