using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }

    [SerializeField]
    private new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    private WeaponData startingWeapon;
    public WeaponData StartingWeapon 
    { 
        get => startingWeapon; 
        private set 
        { 
            startingWeapon = value; 
            EquipStartingWeapon(); // Equip the new starting weapon
        } 
    }

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, recovery, armor;
        [Range(-1, 10)] public float moveSpeed, might, area;
        [Range(-1, 5)] public float speed, duration;
        [Range(-1, 10)] public int amount;
        [Range(-1, 1)] public float cooldown;
        [Min(-1)] public float luck, growth, greed, curse;
        public float magnet;
        public int revival;
        [Range(0, 1.5f)] public float dashDuration;
        public float dashCooldown, maxDashes;
        [Range(0, 2)] public float StunReduction, BurnReduction, SlowReduction;
        public float heartRune;

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.armor += s2.armor;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.area += s2.area;
            s1.speed += s2.speed;
            s1.duration += s2.duration;
            s1.amount += (int)s2.amount; // Explicit cast to int
            s1.cooldown += s2.cooldown;
            s1.luck += s2.luck;
            s1.growth += s2.growth;
            s1.greed += s2.greed;
            s1.curse += s2.curse;
            s1.magnet += s2.magnet;
            s1.dashDuration += s2.dashDuration;
            s1.maxDashes += s2.maxDashes;
            s1.dashCooldown += s2.dashCooldown;
            s1.StunReduction += s2.StunReduction;
            s1.SlowReduction += s2.SlowReduction;
            s1.BurnReduction += s2.BurnReduction;
            s1.heartRune += s2.heartRune;
            return s1;
        }
    }

    public Stats baseStats = new Stats
    {
        maxHealth = 100, moveSpeed = 1, might = 1, amount = 0,
        area = 1, speed = 1, duration = 1, cooldown = 1,
        luck = 1, greed = 1, growth = 1, curse = 1,
        dashDuration = 1.2f, dashCooldown = 5, maxDashes = 1,
        StunReduction = 1, SlowReduction = 1, BurnReduction = 1,
        heartRune = 0
    };

    [SerializeField] public Stats actualStats;
    public Stats ActualStats
    {
        get { return actualStats; }
        set { actualStats = value; }
    }

    private float health;
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, actualStats.maxHealth);
                UpdateHealthBar();
            }
        }
    }

    // Visuals, Materials, Experience/Level, I-Frames, UI, etc.
    [Header("Visuals")]
    public ParticleSystem damageEffect;
    public ParticleSystem blockedEffect;
    public GameObject fireEffect;

    [Header("Materials")]
    public Material DefaultMaterial;
    public Material FireMaterial;
    public Material StunMaterial;
    public Material HurtMaterial;
    public Material SlowMaterial;

    private Renderer playerRenderer;
    private PlayerMovement playerMovement;

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    [Header("I-Frames")]
    public float invincibilityDuration;
    private float invincibilityTimer;
    private bool isInvincible;

    public List<LevelRange> levelRanges;

    private PlayerInventory inventory;
    private PlayerCollector collector;
    public int weaponIndex;
    public int passiveItemIndex;
    public bool isBurning = false;
    public bool isPoisoned = false;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMP_Text levelText;

    public GemBag equippedBag;
    public RuneInventory runeInventory; // Reference to the RuneInventory

    private SaveLoadManager saveLoadManager;

    void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();
        playerRenderer = GetComponent<Renderer>();
        playerMovement = GetComponent<PlayerMovement>();
        saveLoadManager = FindObjectOfType<SaveLoadManager>();

        // Initialize actualStats with baseStats
        actualStats = baseStats;
        // Set the collector radius based on the actual stats
        collector.SetRadius(actualStats.magnet);

        // Initialize health with the calculated max health including rune and gem effects
        health = actualStats.maxHealth;

        // Initialize runeInventory if not assigned
        if (runeInventory == null)
        {
            runeInventory = GetComponent<RuneInventory>();
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Base") // Replace "BaseSceneName" with the actual name of your base scene
        {
            UpdateHealthBar();
            UpdateExpBar();
            UpdateLevelText();
            return; // Exit the method if we are in the base scene
        }

        // Retrieve the equipped weapon from the save file
        WeaponData equippedWeapon = saveLoadManager.LoadEquippedWeapon();
        if (equippedWeapon != null)
        {
            ChangeStartingWeapon(equippedWeapon);
        }
        else
        {
            // Use the default starting weapon if no weapon is loaded
            EquipStartingWeapon();
        }

        ApplyEquippedGemEffects();
        ApplyEquippedRuneEffects();

        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.AssignChosenCharacterUI(this);
        RecalculateStats();

        health = actualStats.maxHealth;
        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    void Update()
    {
        HandleInvincibility();
        Recover();
    }

    private void HandleInvincibility()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }
    }

    public void RecalculateStats()
    {
        // Start with base stats
        actualStats = baseStats;

        // Apply equipped passive item boosts
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }

        // Apply gem effects
        ApplyEquippedGemEffects();
        ApplyEquippedRuneEffects();

        // Update collector radius
        collector.SetRadius(actualStats.magnet);

        // Update health if it exceeds maxHealth
        if (CurrentHealth > actualStats.maxHealth)
        {
            CurrentHealth = actualStats.maxHealth;
        }
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
        UpdateExpBar();
    }

    private void LevelUpChecker()
    {
        while (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = GetExperienceCapIncrease();
            experienceCap += experienceCapIncrease;

            UpdateLevelText();
            GameManager.instance.StartLevelUp();
        }
    }

    private int GetExperienceCapIncrease()
    {
        foreach (LevelRange range in levelRanges)
        {
            if (level >= range.startLevel && level <= range.endLevel)
            {
                return range.experienceCapIncrease;
            }
        }
        return 0;
    }

    private void UpdateExpBar()
    {
        expBar.fillAmount = (float)experience / experienceCap;
    }

    private void UpdateLevelText()
    {
        levelText.text = level.ToString();
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            bool damageAbsorbed = false;

            // Check if there's any equipped rune with takesDamage enabled
            foreach (Rune rune in runeInventory.equippedRuneBag.rune)
            {
                if (rune.takesDamage && rune.actualStats.heartRune > 0)
                {
                    // Absorb damage with heartRune
                    rune.actualStats.heartRune -= dmg;
                    if (rune.actualStats.heartRune <= 0)
                    {
                        // Remove the rune if heartRune is depleted
                        rune.actualStats.heartRune = 0;
                        RemoveDamageAbsorbingRune(rune);
                    }
                    damageAbsorbed = true;
                    break; // Exit the loop after absorbing damage
                }
            }

            if (!damageAbsorbed)
            {
                // Check if there's a HealthGem equipped
                HealthGem healthGem = GetEquippedHealthGem();
                if (healthGem != null)
                {
                    healthGem.TakeDamage(dmg);
                    if (healthGem.currentGemHealth <= 0)
                    {
                        equippedBag.RemoveGem(healthGem); // Remove the gem if it's destroyed
                    }
                }
                else
                {
                    dmg -= actualStats.armor;

                    if (dmg > 0)
                    {
                        CurrentHealth -= dmg;

                        if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

                        if (CurrentHealth <= 0)
                        {
                            Kill();
                        }
                    }
                    else
                    {
                        if (blockedEffect) Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
                    }

                    invincibilityTimer = invincibilityDuration;
                    isInvincible = true;
                }
            }
        }
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, actualStats.maxHealth);
    }

    private void Recover()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + actualStats.recovery * Time.deltaTime, 0, actualStats.maxHealth);
    }

    public void CatchFire(float damagePerSecond, float duration)
    {
        StartCoroutine(ApplyFireDamage(damagePerSecond, duration));
        isBurning = true;

        GameObject fireInstance = Instantiate(fireEffect, transform.position, Quaternion.identity);
        fireInstance.transform.parent = transform;

        playerRenderer.material = FireMaterial;

        StartCoroutine(StopFireEffect(fireInstance, duration));
    }

    private IEnumerator StopFireEffect(GameObject fireInstance, float duration)
    {
        yield return new WaitForSeconds(duration);
        isBurning = false;

        playerRenderer.material = DefaultMaterial;

        Destroy(fireInstance);
    }

    private IEnumerator ApplyFireDamage(float damagePerSecond, float duration)
    {
        float timer = 0f;

        while (timer < duration * actualStats.BurnReduction)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(damagePerSecond);
            timer += 1f;
        }
    }

    public void ApplyPoison(float duration, float damagePerTick)
    {
        StartCoroutine(ApplyPoisonDamage(damagePerTick, duration));
        isPoisoned = true;
    }

    private IEnumerator ApplyPoisonDamage(float damagePerTick, float duration)
    {
        float timer = 0f;
        isPoisoned = true;
        while (timer < duration)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(damagePerTick);
            timer += 1f;
        }
        isPoisoned = false;
    }

    public void ApplyMaterial()
    {
        if (playerMovement.isStunned)
        {
            playerRenderer.material = StunMaterial;
        }
        else if (playerMovement.IsIceSlowed)
        {
            playerRenderer.material = SlowMaterial;
        }
        else
        {
            playerRenderer.material = DefaultMaterial;
        }
    }

    public void ApplyEquippedGemEffects()
    {
        foreach (GemData gem in equippedBag.gems)
        {
            ApplyGemEffect(gem);
        }
    }

    public void ApplyGemEffect(GemData gem)
    {
        if (gem is LifeGem lifeGem)
        {
            actualStats.maxHealth += lifeGem.maxHpIncrease;
            actualStats.recovery += lifeGem.lifeRegenPerSecond;
        }
        else if (gem is PowerGem powerGem)
        {
            actualStats.might += powerGem.attackPowerIncrease;
            actualStats.moveSpeed -= powerGem.moveSpeedDecrease;
            actualStats.moveSpeed = Mathf.Max(actualStats.moveSpeed, 0.01f);
        }
        else if (gem is DefenseGem defenseGem)
        {
            actualStats.armor += defenseGem.armorIncrease;
            actualStats.speed -= defenseGem.attackSpeedDecrease;
            actualStats.speed = Mathf.Max(actualStats.speed, 0.01f);
        }
        else if (gem is SpeedGem speedGem)
        {
            actualStats.moveSpeed += speedGem.moveSpeedIncrease;
            actualStats.maxHealth -= speedGem.healthDecrease;
            actualStats.maxHealth = Mathf.Max(actualStats.maxHealth, 0);
        }
        else if (gem is LuckGem luckGem)
        {
            actualStats.luck += luckGem.luckIncrease;
            actualStats.curse += luckGem.curseIncrease;
        }
        else if (gem is RecoveryGem recoveryGem)
        {
            actualStats.recovery += recoveryGem.recoveryIncrease;
            actualStats.maxHealth -= recoveryGem.maxHealthDecrease;
            actualStats.maxHealth = Mathf.Max(actualStats.maxHealth, 0);
        }
        else if (gem is DashGem dashGem)
        {
            actualStats.maxDashes += dashGem.dashCountIncrease;
            actualStats.dashCooldown -= dashGem.dashCooldownIncrease;
        }
        if (CurrentHealth > actualStats.maxHealth)
        {
            CurrentHealth = actualStats.maxHealth;
        }
        UpdateHealthBar();
    }

    private HealthGem GetEquippedHealthGem()
    {
        foreach (GemData gem in equippedBag.gems)
        {
            if (gem is HealthGem healthGem)
            {
                return healthGem;
            }
        }
        return null;
    }

    private void EquipStartingWeapon()
    {
        if (inventory != null && startingWeapon != null && !inventory.Has(startingWeapon))
        {
            inventory.Add(startingWeapon);
        }
    }

    public void ChangeStartingWeapon(WeaponData newWeapon)
    {
        StartingWeapon = newWeapon;
    }

    public void ApplyEquippedRuneEffects()
    {
        // Ensure runeInventory and equippedRuneBag are not null
        if (runeInventory == null || runeInventory.equippedRuneBag == null)
        {
            Debug.LogWarning("RuneInventory or equippedRuneBag is not initialized.");
            return;
        }

        foreach (Rune rune in runeInventory.equippedRuneBag.rune)
        {
            ApplyRuneEffect(rune);
        }
    }

    public void ApplyRuneEffect(Rune rune)
    {
        if (rune != null && rune.actualStats != null)
        {
            actualStats.maxHealth += rune.actualStats.health;
            actualStats.recovery += rune.actualStats.lifeRegen;
            actualStats.armor += rune.actualStats.armor;
            actualStats.moveSpeed += rune.actualStats.moveSpeed;
            actualStats.speed += rune.actualStats.attackSpeed;
            //actualStats.area += rune.actualStats.area;
            //actualStats.duration += rune.actualStats.duration;
            //actualStats.amount += rune.actualStats.amount;
            //actualStats.cooldown += rune.actualStats.cooldown;
            actualStats.luck += rune.actualStats.luck;
            //actualStats.growth += rune.actualStats.growth;
            //actualStats.greed += rune.actualStats.greed;
            actualStats.curse += rune.actualStats.curse;
            //actualStats.magnet += rune.actualStats.magnet;
            //actualStats.revival += rune.actualStats.revival;
            //actualStats.dashDuration += rune.actualStats.dashDuration;
            actualStats.dashCooldown += rune.actualStats.dashCooldown;
            actualStats.maxDashes += rune.actualStats.dashCount;
            //actualStats.StunReduction += rune.actualStats.StunReduction;
            //actualStats.BurnReduction += rune.actualStats.BurnReduction;
            //actualStats.SlowReduction += rune.actualStats.SlowReduction;
            if (rune.actualStats.heartRune > 0 && rune.takesDamage)
            {
                actualStats.heartRune += rune.actualStats.heartRune;
            }
        }
    }

    private Rune GetEquippedDamageAbsorbingRune()
    {
        foreach (Rune rune in runeInventory.equippedRuneBag.rune)
        {
            if (rune.actualStats.heartRune > 0 && rune.takesDamage)
            {
                return rune;
            }
        }
        return null;
    }

    // Method to remove the damage-absorbing rune from the inventory
    private void RemoveDamageAbsorbingRune(Rune rune)
    {
        runeInventory.equippedRuneBag.rune.Remove(rune);
    }
}
