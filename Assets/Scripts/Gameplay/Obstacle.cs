using UnityEngine;

public enum ObKind { Block, Jump, Overhead, Hazard }
public enum ObDamage { Block, Jump, Overhead, Mine, Barrel, Fire, Electric }

public class Obstacle : MonoBehaviour, IShootable
{
    public ObKind kind;
    public ObDamage dmg;
    public bool explodes, destructible, persists;
    bool used;
    float nextTick;

    public bool CanBeShot { get { return destructible && !used; } }
    void OnEnable() { used = false; nextTick = 0f; }

    float Damage()
    {
        var c = GameConfig.I;
        switch (dmg)
        {
            case ObDamage.Jump: return c.jumpDamage;
            case ObDamage.Overhead: return c.overheadDamage;
            case ObDamage.Mine: return c.mineDamage;
            case ObDamage.Barrel: return c.barrelDamage;
            case ObDamage.Fire: return c.fireDamagePerTick;
            case ObDamage.Electric: return c.electricDamage;
            default: return c.blockDamage;
        }
    }

    void OnTriggerEnter(Collider o)
    {
        if (used || persists) return;
        var pc = o.GetComponent<PlayerController>();
        if (pc != null) Hit(pc);
    }

    void OnTriggerStay(Collider o)
    {
        if (!persists || Time.time < nextTick) return;
        var pc = o.GetComponent<PlayerController>();
        if (pc == null) return;
        var c = GameConfig.I;
        nextTick = Time.time + (dmg == ObDamage.Fire ? c.fireTickInterval : c.electricTickInterval);
        Hit(pc);
    }

    void Hit(PlayerController pc)
    {
        pc.health.TakeDamage(Damage(), name);
        if (explodes) Explode();
        else if (!persists)
        {
            used = true;
            Fx.Spawn("fx_debris", transform.position + Vector3.up);
            CameraFollow.I.Shake(0.2f);
            ObjectPooler.I.Release(gameObject);
        }
        else Fx.Spawn("fx_hit", pc.transform.position + Vector3.up);
    }

    void Explode()
    {
        used = true;
        Vector3 p = transform.position + Vector3.up * 0.5f;
        Fx.Spawn("fx_explosion", p);
        AudioManager.I.Play(Sfx.Explosion, 0.9f);
        CameraFollow.I.Shake(0.5f);
        ObjectPooler.I.Release(gameObject);
    }

    // Shooting a barrel detonates it and damages nearby zombies.
    public void OnShot(float damage, Vector3 point)
    {
        if (!CanBeShot) return;
        used = true;
        var hits = Physics.OverlapSphere(transform.position, 5f, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < hits.Length; i++)
        {
            var z = hits[i].GetComponent<ZombieAI>();
            if (z != null && z.CanBeShot) z.OnShot(10f, z.transform.position + Vector3.up);
        }
        var pl = GameManager.I.player;
        if (pl != null && Vector3.Distance(pl.transform.position, transform.position) < 3.2f) pl.health.TakeDamage(GameConfig.I.barrelDamage * 0.6f, "blast");
        GameManager.I.AddScore(25);
        Explode();
    }
}
