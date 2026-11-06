using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType { Coin, Health, Ammo, Magnet, Shield, Speed, DoubleCoins }

/// <summary>All pickups. A single static Tick() animates (spin/bob) and magnet-attracts every active pickup - no per-object Update.</summary>
public class Collectible : MonoBehaviour
{
    public CollectibleType type;
    public Transform spin;
    public static readonly List<Collectible> Active = new List<Collectible>();
    bool attracted;
    float phase;

    void OnEnable() { Active.Add(this); attracted = false; phase = Random.value * 6f; }
    void OnDisable() { Active.Remove(this); }

    public static void Tick(float dt, Vector3 playerPos)
    {
        var pm = PowerUpManager.I;
        bool magnet = pm != null && pm.Active(PowerUpType.Magnet);
        float rad = GameConfig.I.magnetRadius, r2 = rad * rad;
        float t = Time.time;
        for (int i = Active.Count - 1; i >= 0; i--)
        {
            var c = Active[i];
            if (c == null) { Active.RemoveAt(i); continue; }
            bool coin = c.type == CollectibleType.Coin;
            if (c.spin != null)
            {
                c.spin.Rotate(0f, (coin ? 270f : 150f) * dt, 0f, Space.Self);
                if (!coin) c.spin.localPosition = new Vector3(0, 0.12f * Mathf.Sin(t * 3f + c.phase), 0);
            }
            if (!magnet || !coin) continue;
            Vector3 pos = c.transform.position;
            Vector3 target = playerPos + Vector3.up;
            if (!c.attracted && pos.z > playerPos.z - 1f && (pos - target).sqrMagnitude < r2) c.attracted = true;
            if (c.attracted)
            {
                pos = Vector3.MoveTowards(pos, target, 40f * dt);
                c.transform.position = pos;
                if ((pos - target).sqrMagnitude < 0.6f) c.Collect();
            }
        }
    }

    void OnTriggerEnter(Collider o)
    {
        if (o.GetComponent<PlayerController>() != null) Collect();
    }

    public void Collect()
    {
        var gm = GameManager.I; var cfg = GameConfig.I; var pm = PowerUpManager.I; var ui = UIManager.I;
        if (gm == null || !gameObject.activeSelf) return;
        Vector3 pos = transform.position;
        string msg = null; Color col = Color.white;
        switch (type)
        {
            case CollectibleType.Coin:
                gm.AddCoin();
                AudioManager.I.Play(Sfx.Coin, 0.6f, Random.Range(0.95f, 1.15f));
                break;
            case CollectibleType.Health:
                gm.health.Heal(cfg.healthKitAmount);
                AudioManager.I.Play(Sfx.Health);
                msg = "+" + Mathf.RoundToInt(cfg.healthKitAmount) + " HEALTH"; col = new Color(1f, 0.35f, 0.35f);
                break;
            case CollectibleType.Ammo:
                gm.shooting.AddAmmo(cfg.ammoPickupAmount);
                AudioManager.I.Play(Sfx.Ammo);
                msg = "+" + cfg.ammoPickupAmount + " AMMO"; col = new Color(1f, 0.85f, 0.3f);
                break;
            case CollectibleType.Magnet:
                pm.Activate(PowerUpType.Magnet, cfg.magnetDuration); msg = "COIN MAGNET!"; col = new Color(0.9f, 0.4f, 1f); break;
            case CollectibleType.Shield:
                pm.Activate(PowerUpType.Shield, cfg.shieldDuration); msg = "SHIELD ACTIVE!"; col = new Color(0.4f, 0.75f, 1f); break;
            case CollectibleType.Speed:
                pm.Activate(PowerUpType.Speed, cfg.speedBoostDuration); msg = "SPEED BOOST!"; col = new Color(0.4f, 1f, 0.5f); break;
            case CollectibleType.DoubleCoins:
                pm.Activate(PowerUpType.DoubleCoins, cfg.doubleCoinsDuration); msg = "DOUBLE COINS 2X!"; col = new Color(1f, 0.85f, 0.2f); break;
        }
        if (type != CollectibleType.Coin) Fx.Spawn("fx_pickup", pos);
        if (msg != null && ui != null) ui.Notify(msg, col);
        ObjectPooler.I.Release(gameObject);
    }
}
