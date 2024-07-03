using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    protected EnemyStats enemy;
    protected Transform player;

    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;

    public enum OutOfFrameAction { none, respawnAtEdge, despawn }
    public OutOfFrameAction outOfFrameAction = OutOfFrameAction.respawnAtEdge;

    protected bool spawnedOutOfFrame = false;

    protected virtual void Start()
    {
        enemy = GetComponent<EnemyStats>();
        spawnedOutOfFrame = !SpawnManager.IsWithinBoundaries(transform);

        // Picks a random player on the screen, instead of always picking the 1st player.
        PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();
        player = allPlayers[Random.Range(0, allPlayers.Length)].transform;
    }

    protected virtual void Update()
    {
        // If we are currently being knocked back, then process the knockback.
        if(knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            // Constantly move the enemy towards the player.
            Move();

            // Handle the enemy when it is out of frame.
            if (!SpawnManager.IsWithinBoundaries(transform))
            {
                switch (outOfFrameAction)
                {
                    case OutOfFrameAction.none:
                    default:
                        break;
                    case OutOfFrameAction.respawnAtEdge:
                        // If the enemy is outside the camera frame, teleport it back to the edge of the frame.
                        transform.position = SpawnManager.GeneratePosition();
                        break;
                    case OutOfFrameAction.despawn:
                        if (!spawnedOutOfFrame)
                        {
                            // Disable the drops if we despawn the object.
                            DropRateManager drops = GetComponent<DropRateManager>();
                            if(drops) drops.active = false;
                            Destroy(gameObject);
                        }
                        break;
                }
            }
            else spawnedOutOfFrame = false;
        }
    }

    // This is meant to be called from other scripts to create knockback.
    public virtual void Knockback(Vector2 velocity, float duration)
    {
        // Ignore the knockback if the duration is greater than 0.
        if(knockbackDuration > 0) return;

        // Begins the knockback.
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }

    public virtual void Move()
    {
        // Constantly move the enemy towards the player
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.ActualStats.moveSpeed * Time.deltaTime);
    }
}