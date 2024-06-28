using UnityEngine;
using System.Collections;

public class EnemyAbility : MonoBehaviour
{
    [Header("Ability Chance")]
    public float generalAbilityChance = 70f; // Chance of having any ability, in percentage
    public float lightningAbilityChance = 50f; // Chance of having lightning ability, in percentage
    public float burningAbilityChance = 30f; // Chance of having burning ability, in percentage
    public float iceAbilityChance = 20f; // Chance of having ice ability, in percentage
    public float poisonAbilityChance = 1f; // Chance of having poison ability, in percentage
    public float poisonCloudChance = 50f; // can onlly get this if enemy as poison
    public float voxAbilityChance = 10f; // Chance of having Vox ability, in percentage
    public float shieldAbilityChance = 25f; // Chance of having Shield ability, in percentage

    [Header("Materials")]
    public Material lightningMaterial;
    public Material burningMaterial;
    public Material iceMaterial;
    public Material poisonMaterial;
    public Material shieldMaterial;

    [Header("Lightning")]
    public bool hasLightningAbility = false;
    public GameObject lightningEffectPrefab; // Prefab for the lightning effect
    public GameObject lightningProjectilePrefab;
    public float projectileLightningSpeed = 5f;
    public float projectileDuration = 4f; // Adjust the duration as needed
    public int numLightningProjectiles = 3;
    

    [Header("Burn")]
    public bool hasBurningAbility = false;
    public GameObject fireEffectPrefab; // Prefab for the fire effect
    public float burnDuration = 5f; // Duration of burn effect
    public float burnDamagePerSecond = 1f; // Burn damage per second

    [Header("Ice")]
    public bool hasIceAbility = false;
    public GameObject iceEffectPrefab; // Prefab for the lightning effect
    public GameObject iceTrailPrefab; // Prefab for the ice trail
    public GameObject iceSpikePrefab; // Prefab for the ice spike
    public float iceTrailInterval = 0.5f; // Interval between creating ice trail segments
    public float iceSlowDuration = 1f; // Duration of player slow effect by ice
    public float iceSlowPercentage = .25f;
    public float iceSpikeDuration = 2f; // Duration of ice spike after enemy's death
    public float iceSpikeDamage = 10f; // Damage inflicted by the ice spike
    private GameObject iceTrail;
    private float lastIceTrailTime; // Time when the last ice trail was created

    [Header("Poison")]
    public bool hasPoisonAbility = false;
    public GameObject poisonEffectPrefab;
    public float poisonDuration = 3f; // Duration of poison effect
    public float poisonDamagePerTick = 2f; // Poison damage per tick

    [Header("Poison Cloud")]
    public bool leavesPoisonCloud = false;
    public GameObject poisonCloudPrefab; // Prefab for the poison cloud
    public float poisonCloudDuration = 5f; // Duration of poison cloud
    public float poisonCloudDamagePerTick = 1.5f; // Poison cloud damage per tick

    [Header("Vox")]
    public bool hasVoxAbility = false;
    public GameObject voxPrefab;
    public float voxEffectRadius = 3f;
    public float pullForce = 5f; // Force to pull the player towards the center
    public float slowFactor = 0.5f; // Factor to slow down the player's movement
    public float effectDuration = 5f; // Duration the Vox effect lasts
    public float voxCooldown = 10f; // Cooldown duration for the Vox ability in seconds
    private float lastVoxTime; // Timestamp when the last Vox ability was initiated
    private bool isVoxOnCooldown; // Flag to indicate if Vox ability is on cooldown

    [Header("Shield")]
    public bool hasShieldAbility = false;
    public GameObject shieldPrefab;
    public float shieldDuration = 5f;
    public float shieldCooldown = 10f;
    public float damageBlockPercentage = 50f; // Percentage of damage blocked by the shield

    private float lastShieldTime;
    private bool isShieldOnCooldown;
    private GameObject shieldInstance;


    // Reference to the instantiated effects
    private GameObject lightningEffect;
    private GameObject fireEffect;
    private GameObject iceEffect;
    private GameObject poisonEffect;
    private GameObject shieldEffect;


    void Start()
    {
        lastIceTrailTime = Time.time; // Initialize the last ice trail time to the current time
        lastVoxTime = -voxCooldown; // Start with a cooldown already passed (effectively allowing the first use immediately)


        // Check if the enemy has the capability to possess any ability
        if (Random.Range(0f, 100f) <= generalAbilityChance)
        {
            // Check if the enemy has the lightning ability based on probability
            if (Random.Range(0f, 100f) <= lightningAbilityChance && lightningEffectPrefab != null)
            {
                hasLightningAbility = true;
                lightningEffect = InstantiateEffect(lightningEffectPrefab);
                ApplyMaterial(lightningMaterial);
            }

            // Check if the enemy has the burning ability based on probability
            if (Random.Range(0f, 100f) <= burningAbilityChance && fireEffectPrefab != null)
            {
                hasBurningAbility = true;
                //fireEffect = InstantiateEffect(fireEffectPrefab);
                ApplyMaterial(burningMaterial);
            }

            // Check if the enemy has the ice ability based on probability
            if (Random.Range(0f, 100f) <= iceAbilityChance && iceEffectPrefab != null)
            {
                hasIceAbility = true;
                iceEffect = InstantiateEffect(iceEffectPrefab);
                ApplyMaterial(iceMaterial);
            }

            // Check if the enemy has the Poison ability based on probability
            if (Random.Range(0f, 100f) <= poisonAbilityChance && poisonEffectPrefab != null)
            {
                hasPoisonAbility = true;
                //poisonEffect = InstantiateEffect(poisonEffectPrefab);
                ApplyMaterial(poisonMaterial);

                if (Random.Range(0f, 100f) <= poisonCloudChance && hasPoisonAbility)
                {
                    leavesPoisonCloud = true;
                }
            }

            if (Random.Range(0f, 100f) <= voxAbilityChance && voxPrefab != null)
            {
                hasVoxAbility = true;
                isVoxOnCooldown = false;
                ThrowVox();
                //ApplyMaterial(voxMaterial);
            }

            // Check if the enemy has the shield ability based on probability
            if (Random.Range(0f, 100f) <= shieldAbilityChance && shieldPrefab != null)
            {
                hasShieldAbility = true;
                shieldEffect = InstantiateEffect(shieldPrefab);
                //ApplyMaterial(shieldMaterial);
                //SpawnShield();
            }
        }
    }

    // Method to apply material to the instantiated effect
    private void ApplyMaterial(Material material)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            renderer.material = material;
        }
        else
        {
            Debug.LogWarning("Renderer or Material is missing.");
        }
    }

    // Method to instantiate an effect and return a reference to it
    private GameObject InstantiateEffect(GameObject effectPrefab)
    {
        if (effectPrefab != null)
        {
            return Instantiate(effectPrefab, transform.position, Quaternion.identity, transform);
        }
        else
        {
            //Debug.LogWarning("Effect Prefab is missing.");
            return null;
        }
    }

    void Update()
    {
        // Check if it's time to create a new ice trail
        if (hasIceAbility && Time.time - lastIceTrailTime >= iceTrailInterval)
        {
            CreateIceTrail();
            lastIceTrailTime = Time.time; // Update the last ice trail time
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && hasBurningAbility)
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (player != null && !player.isBurning)
            {
                Debug.Log(" Enemy Burnt Player.");
                // Call the method to apply fire damage
                player.CatchFire(burnDamagePerSecond, burnDuration);
            }
        }

        if (collision.gameObject.CompareTag("Player") && hasPoisonAbility)
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (player != null && !player.isPoisoned)
            {
                Debug.Log("Enemy Poisoned Player.");
                player.ApplyPoison(poisonDuration, poisonDamagePerTick);
            }
        }
    }

    // Method to instantiate a lightning projectile in a given direction
    public void InstantiateLightningProjectile(Vector2 origin, Vector2 direction)
    {
        if (lightningProjectilePrefab != null)
        {
            for (int i = 0; i < numLightningProjectiles; i++)
            {
                // Generate a random spread angle for each projectile
                float randomSpreadAngle = Random.Range(0f, 360f);

                // Calculate direction based on random angle
                Vector2 projectileDirection = Quaternion.Euler(0, 0, randomSpreadAngle) * Vector2.right;

                // Instantiate the projectile
                GameObject lightningProjectile = Instantiate(lightningProjectilePrefab, origin, Quaternion.identity);
                Rigidbody2D rb = lightningProjectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Set initial velocity
                    rb.velocity = projectileDirection.normalized * projectileLightningSpeed; // Adjust projectile speed as needed

                    // Destroy the lightning projectile after the specified duration
                    Destroy(lightningProjectile, projectileDuration);
                }
                else
                {
                    Debug.LogWarning("Rigidbody2D component not found in the lightning projectile prefab.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Lightning Projectile Prefab not assigned.");
        }
    }

    // Method to create an ice trail
    private void CreateIceTrail()
    {
        if (iceTrailPrefab != null)
        {
            // Instantiate the ice trail at the enemy's current position
            iceTrail = Instantiate(iceTrailPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Ice Trail Prefab not assigned.");
        }
    }

    // Method to spawn an ice spike at the enemy's position
    public void SpawnIceSpike()
    {
        if (iceSpikePrefab != null)
        {
            GameObject iceSpike = Instantiate(iceSpikePrefab, transform.position, Quaternion.identity);
            Destroy(iceSpike, iceSpikeDuration); // Destroy the ice spike after the specified duration
        }
        else
        {
            Debug.LogWarning("Ice Spike Prefab not assigned.");
        }
    }

    // Method to destroy the effects
    public void DestroyEffects()
    {
        if (lightningEffect != null)
        {
            Destroy(lightningEffect);
        }

        if (fireEffect != null)
        {
            Destroy(fireEffect);
        }

        if (iceEffect != null)
        {
            Destroy(iceEffect);
        }

        if (iceTrail != null)
        {
            Destroy(iceTrail);
        }
    }

    public void CreatePoisonCloud()
    {
        if (leavesPoisonCloud && poisonCloudPrefab != null)
        {
            // Instantiate the poison cloud at the enemy's position
            GameObject poisonCloud = Instantiate(poisonCloudPrefab, transform.position, Quaternion.identity);

            // Set the duration and damage per tick of the poison cloud
            PoisonCloud poisonCloudScript = poisonCloud.GetComponent<PoisonCloud>();
            if (poisonCloudScript != null)
            {
                poisonCloudScript.duration = poisonCloudDuration;
                poisonCloudScript.damagePerTick = poisonCloudDamagePerTick;
            }
            else
            {
                Debug.LogWarning("PoisonCloud script not found on poison cloud prefab.");
            }
        }
    }

    private void ThrowVox()
{
    if (Time.time - lastVoxTime >= voxCooldown)
    {
        if (voxPrefab != null)
        {
            // Calculate random position around the enemy within a radius
            Vector3 randomPosition = transform.position + (Vector3)Random.insideUnitCircle.normalized * voxEffectRadius;

            // Instantiate Vox prefab at the calculated position
            GameObject voxInstance = Instantiate(voxPrefab, randomPosition, Quaternion.identity);

            // Initialize the VoxEffect script with the parameters from Ability
            VoxEffect voxEffect = voxInstance.GetComponent<VoxEffect>();
            if (voxEffect != null)
            {
                voxEffect.Initialize(voxEffectRadius, pullForce, slowFactor, effectDuration);
            }
            else
            {
                Debug.LogWarning("VoxEffect script not found on Vox prefab.");
            }

            // Update the last Vox time to current time
            lastVoxTime = Time.time;
            isVoxOnCooldown = true;
            StartCoroutine(VoxCooldown());
        }
        else
        {
            Debug.LogWarning("Vox Prefab not assigned.");
        }
    }
    else
    {
        Debug.Log("Vox ability is on cooldown.");
    }
}

    // Vox cooldown
    private IEnumerator VoxCooldown()
    {
        yield return new WaitForSeconds(voxCooldown);
        isVoxOnCooldown = false;
        ThrowVox();
    }

    public void SpawnShield()
    {
        if (Time.time - lastShieldTime >= shieldCooldown && hasShieldAbility && shieldPrefab != null)
        {
            // Instantiate the shield prefab at the enemy's position
            shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);
            ShieldEffect shieldEffect = shieldInstance.GetComponent<ShieldEffect>();
            if (shieldEffect != null) {
                shieldEffect.Initialize(damageBlockPercentage);
            } else {
                Debug.LogWarning("ShieldEffect component not found on the instantiated shield prefab.");
            }            
            lastShieldTime = Time.time;
            isShieldOnCooldown = true;
            StartCoroutine(ShieldDuration());
        }
        else
        {
            Debug.Log("Shield ability is on cooldown.");
        }
    }

    private IEnumerator ShieldDuration()
    {
        yield return new WaitForSeconds(shieldDuration);
        if (shieldInstance != null)
        {
            Destroy(shieldInstance);
        }
        StartCoroutine(ShieldCooldown());
    }

    private IEnumerator ShieldCooldown()
    {
        yield return new WaitForSeconds(shieldCooldown);
        isShieldOnCooldown = false;
    }
}