using UnityEngine;

[CreateAssetMenu(fileName = "Mob Event Data", menuName = "2D Top-down Rogue-like/Event Data/Mob")]
public class MobEventData : EventData
{
    [Header("Mob Data")]
    public float possibleAngles = 360f;
    public float spawnRadius = 2f;
    public float spawnDistance = 20f;

    public override bool Activate(PlayerStats player = null, bool alwaysFires = false)
    {
        // Run the probability to see if we should activate this event.
        if (!CheckIfWillHappen(player)) return false;

        // Only activate this if the player is present.
        if(player)
        {
            // Otherwise, we spawn a mob outside of the screen and move it towards the player.
            float randomAngle = Random.Range(0, possibleAngles) * Mathf.Deg2Rad;
            foreach (GameObject o in GetSpawns())
            {
                // Stop spawning if we have exceeded maximum enemies.
                if (SpawnManager.HasExceededMaxEnemies()) break;

                Instantiate(o, player.transform.position + new Vector3(
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Cos(randomAngle),
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Sin(randomAngle)
                ), Quaternion.identity);
            }
        }

        return false;
    }
}
