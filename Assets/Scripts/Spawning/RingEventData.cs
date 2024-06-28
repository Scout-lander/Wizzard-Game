using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring Event Data", menuName = "2D Top-down Rogue-like/Event Data/Ring")]
public class RingEventData : EventData
{
    [Header("Mob Data")]
    public GameObject spawnEffectPrefab;
    public Vector2 scale = new Vector2(1, 1);
    public float spawnRadius = 10f, lifespan = 15f;

    public override bool Activate(PlayerStats player = null, bool alwaysFires = false)
    {
        // Run the probability to see if we should activate this event.
        if (!CheckIfWillHappen(player)) return false;

        // Only activate this if the player is present.
        if (player)
        {
            GameObject[] spawns = GetSpawns();
            float angleOffset = 2 * Mathf.PI / Mathf.Max(1, spawns.Length);
            float currentAngle = 0;
            foreach(GameObject g in spawns)
            {
                // Stop spawning if we have exceeded maximum enemies.
                if (SpawnManager.HasExceededMaxEnemies()) break;

                GameObject s = Instantiate(g, player.transform.position + new Vector3(
                    spawnRadius * Mathf.Cos(currentAngle),
                    spawnRadius * Mathf.Sin(currentAngle)
                ), Quaternion.identity);

                // If there is a lifespan on the mob, set them to be destroyed.
                if (lifespan > 0) Destroy(s, lifespan);

                currentAngle += angleOffset;
            }
        }

        return false;
    }
}
