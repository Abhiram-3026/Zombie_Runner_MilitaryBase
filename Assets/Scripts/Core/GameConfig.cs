using UnityEngine;

/// <summary>All tunable gameplay values. Editable in the Inspector on the "GameConfig" object (Gameplay scene, play mode).</summary>
public class GameConfig : MonoBehaviour
{
    public static GameConfig I;

    [Header("Running")]
    public float laneWidth = 2.5f;
    public float baseSpeed = 14f;
    public float maxSpeed = 30f;
    public float distanceToMaxDifficulty = 4000f;
    public float laneChangeSharpness = 14f;
    public float jumpVelocity = 12.5f;
    public float gravity = -38f;
    public float slideDuration = 0.9f;

    [Header("Player")]
    public float maxHealth = 100f;
    public float invulnerableTime = 0.9f;
    public int startAmmo = 30;
    public int maxAmmo = 150;

    [Header("Weapon")]
    public float fireInterval = 0.15f;
    public float bulletDamage = 1f;
    public float shootRange = 48f;
    public float pistolInterval = 0.3f;
    public float pistolDamage = 1.5f;

    [Header("Zombies")]
    public float normalHealth = 2f;
    public float fastHealth = 1f;
    public float armoredHealth = 6f;
    public float normalSpeed = 3.5f;
    public float fastSpeed = 8f;
    public float armoredSpeed = 2.5f;
    public float normalDamage = 15f;
    public float fastDamage = 12f;
    public float armoredDamage = 28f;
    public float zombieWakeDistance = 55f;

    [Header("Obstacle damage")]
    public float blockDamage = 35f;
    public float jumpDamage = 20f;
    public float overheadDamage = 20f;
    public float mineDamage = 45f;
    public float barrelDamage = 35f;
    public float fireDamagePerTick = 8f;
    public float fireTickInterval = 0.5f;
    public float electricDamage = 25f;
    public float electricTickInterval = 1f;

    [Header("Pickups / Power-ups")]
    public float healthKitAmount = 35f;
    public int ammoPickupAmount = 20;
    public float magnetDuration = 10f;
    public float magnetRadius = 14f;
    public float shieldDuration = 8f;
    public float speedBoostDuration = 6f;
    public float speedBoostMultiplier = 1.5f;
    public float doubleCoinsDuration = 12f;
    public int coinValue = 1;
    public int coinScore = 10;
    public int killScore = 50;

    [Header("Spawning")]
    public float spawnAhead = 150f;
    public float firstSpawnZ = 70f;
    public float minGapSeconds = 0.8f;
    public float maxGapSeconds = 1.5f;

    void Awake() { I = this; }
}
