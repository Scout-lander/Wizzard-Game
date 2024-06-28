// Meteor.cs
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float damage = 20f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>().TakeDamage(damage);
            Destroy(gameObject); // Destroy the meteor after hitting the player
        }
    }
}
