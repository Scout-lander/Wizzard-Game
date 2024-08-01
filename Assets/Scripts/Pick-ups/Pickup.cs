using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float lifespan = 0.5f;
    protected PlayerStats target;
    protected float speed;
    protected Vector2 initialPosition;
    protected float initialOffset;

    // To represent the bobbing animation of the object.
    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency;
        public Vector2 direction;
    }
    public BobbingAnimation bobbingAnimation = new BobbingAnimation {
        frequency = 2f, direction = new Vector2(0, 0.3f)
    };

    [Header("Bonuses")]
    public int experience;
    public int health;

    public PickupType pickupType;  // Added pickup type
    public RuneDataNew runeDataNew; // Reference to the RuneDataNew scriptable object

    protected virtual void Start()
    {
        initialPosition = transform.position;
        initialOffset = Random.Range(0, bobbingAnimation.frequency);
        PickupManager.Instance.AddPickup(pickupType);  // Increment pickup count
    }

    protected virtual void Update()
    {
        if (target)
        {
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            else
                Destroy(gameObject);
        }
        else
        {
            transform.position = initialPosition + bobbingAnimation.direction * Mathf.Sin((Time.time + initialOffset) * bobbingAnimation.frequency);
        }
    }

    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        // Ensure that the target is only set if it hasn't already been set.
        if (!this.target)
        {
            this.target = target;
            this.speed = speed;
            if (lifespan > 0) this.lifespan = lifespan;

        if (pickupType == PickupType.Rune)  // Handling rune pickup
            {
                Debug.LogWarning("RunePickup script should handle this case.");
            }

            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));  // Destroy the pickup object
            return true;
        }

        return false;
    }

    protected virtual void OnDestroy()
    {
        if (!target) return;
        target.IncreaseExperience(experience);
        target.RestoreHealth(health);
        PickupManager.Instance.RemovePickup(pickupType);  // Decrement pickup count
    }
}
