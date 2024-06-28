using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData.Stats Stats
    {
        get { return actualStats; }
        set
        {
            actualStats = value;
        }
    }

    float health;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return health; }

        set
        {
            if (health != value)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }
    #endregion

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
    float invincibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;

    PlayerInventory inventory;
    PlayerStats player;
    PlayerCollector collector;
    public int weaponIndex;
    public int passiveItemIndex;
    public bool isBurning = false;
    public bool isPoisoned = false;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMP_Text levelText;

    public GemBag equippedBag;

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        if (CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();
        playerRenderer = GetComponent<Renderer>();
        playerMovement = GetComponent<PlayerMovement>();

        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;
    }

    void Start()
    {
        inventory.Add(characterData.StartingWeapon);
        player = GetComponent<PlayerStats>();
        ApplyEquippedGemEffects();

        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }

        ApplyEquippedGemEffects();

        collector.SetRadius(actualStats.magnet);
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();
        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();

            GameManager.instance.StartLevelUp();

            if (experience >= experienceCap) LevelUpChecker();
        }
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = level.ToString();
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
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

    void UpdateHealthBar()
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
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;

            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
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

    IEnumerator StopFireEffect(GameObject fireInstance, float duration)
    {
        yield return new WaitForSeconds(duration);
        isBurning = false;

        playerRenderer.material = DefaultMaterial;

        Destroy(fireInstance);
    }

    IEnumerator ApplyFireDamage(float damagePerSecond, float duration)
    {
        float timer = 0f;

        while (timer < duration * player.Stats.BurnReduction)
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

    IEnumerator ApplyPoisonDamage(float damagePerTick, float duration)
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
            if (actualStats.moveSpeed < 0) actualStats.moveSpeed = 0.01f;
        }
        else if (gem is DefenseGem defenseGem)
        {
            actualStats.armor += defenseGem.armorIncrease;
            actualStats.speed -= defenseGem.attackSpeedDecrease;
            if (actualStats.speed < 0) actualStats.speed = 0.01f;
        }
        else if (gem is SpeedGem speedGem)
        {
            actualStats.moveSpeed += speedGem.moveSpeedIncrease;
            actualStats.maxHealth -= speedGem.healthDecrease;
            if (actualStats.maxHealth < 0) actualStats.maxHealth = 0;
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
            if (actualStats.maxHealth < 0) actualStats.maxHealth = 0;
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
}
