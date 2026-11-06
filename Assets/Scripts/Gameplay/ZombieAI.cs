using UnityEngine;

public enum ZombieType { Normal, Fast, Armored }

public class ZombieAI : MonoBehaviour, IShootable
{
    public ZombieType type;
    public bool menuMode;
    public Transform model, body, head, armL, armR, legL, legR;
    float health, speed, damage, deadT, attackT, phase;
    bool awake, dead, attacking;
    Collider col;
    static float lastGrowl;

    public bool CanBeShot { get { return !dead; } }

    void OnEnable()
    {
        var c = GameConfig.I;
        dead = false; attacking = false; awake = false; deadT = 0f; attackT = 0f; phase = Random.value * 10f;
        switch (type)
        {
            case ZombieType.Fast: health = c.fastHealth; speed = c.fastSpeed; damage = c.fastDamage; break;
            case ZombieType.Armored: health = c.armoredHealth; speed = c.armoredSpeed; damage = c.armoredDamage; break;
            default: health = c.normalHealth; speed = c.normalSpeed; damage = c.normalDamage; break;
        }
        if (col == null) col = GetComponent<Collider>();
        col.enabled = true;
        model.localRotation = Quaternion.identity;
        var mp = model.localPosition; mp.y = 0; model.localPosition = mp;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;
        if (menuMode) { Idle(dt); return; }
        var gm = GameManager.I;
        if (gm == null || gm.player == null) return;

        if (dead)
        {
            deadT += dt;
            model.localRotation = Quaternion.Euler(Mathf.Lerp(0f, -88f, Mathf.Clamp01(deadT * 3f)), 0, 0);
            if (deadT > 1.6f) ObjectPooler.I.Release(gameObject);
            return;
        }
        if (gm.State != GameState.Playing) { Idle(dt); return; }

        float dist = transform.position.z - gm.player.transform.position.z;
        if (!awake)
        {
            if (dist < GameConfig.I.zombieWakeDistance)
            {
                awake = true;
                if (Time.time - lastGrowl > 1.2f) { lastGrowl = Time.time; AudioManager.I.Play(Sfx.Zombie, 0.5f, Random.Range(0.8f, 1.2f)); }
            }
            else { Idle(dt); return; }
        }
        if (attacking)
        {
            attackT += dt;
            armL.localRotation = Quaternion.Euler(-120f + Mathf.Sin(attackT * 30f) * 20f, 0, 0);
            armR.localRotation = Quaternion.Euler(-120f - Mathf.Sin(attackT * 30f) * 20f, 0, 0);
            if (attackT > 0.55f) ObjectPooler.I.Release(gameObject);
            return;
        }
        transform.position += new Vector3(0, 0, -speed * dt);
        float rate = type == ZombieType.Fast ? 15f : (type == ZombieType.Armored ? 6f : 9f);
        phase += dt * rate;
        float s = Mathf.Sin(phase);
        legL.localRotation = Quaternion.Euler(s * 40f, 0, 0);
        legR.localRotation = Quaternion.Euler(-s * 40f, 0, 0);
        armL.localRotation = Quaternion.Euler(-85f + s * 8f, 0, 0);
        armR.localRotation = Quaternion.Euler(-80f - s * 8f, 0, 0);
        body.localRotation = Quaternion.Euler(14f, 0, Mathf.Sin(phase * 0.5f) * 6f);
        head.localRotation = Quaternion.Euler(-10f, Mathf.Sin(phase * 0.7f) * 15f, Mathf.Sin(phase * 0.5f) * 10f);
    }

    void Idle(float dt)
    {
        phase += dt * 1.3f;
        float s = Mathf.Sin(phase);
        armL.localRotation = Quaternion.Euler(-40f + s * 8f, 0, 0);
        armR.localRotation = Quaternion.Euler(-35f - s * 8f, 0, 0);
        body.localRotation = Quaternion.Euler(10f + s * 2f, 0, s * 3f);
        head.localRotation = Quaternion.Euler(-8f, s * 12f, s * 6f);
        legL.localRotation = Quaternion.identity; legR.localRotation = Quaternion.identity;
    }

    void OnTriggerEnter(Collider o)
    {
        if (dead || attacking || menuMode) return;
        var pc = o.GetComponent<PlayerController>();
        if (pc == null) return;
        attacking = true; attackT = 0f;
        pc.health.TakeDamage(damage, "zombie");
        Fx.Spawn("fx_claw", pc.transform.position + Vector3.up * 1.2f);
        AudioManager.I.Play(Sfx.Zombie, 0.7f, Random.Range(0.8f, 1.2f));
    }

    public void OnShot(float dmg, Vector3 point)
    {
        if (dead) return;
        health -= dmg;
        if (health <= 0f)
        {
            dead = true; deadT = 0f; col.enabled = false;
            GameManager.I.AddKill(type);
            AudioManager.I.Play(Sfx.ZombieDie, 0.8f, Random.Range(0.9f, 1.1f));
            Fx.Spawn("fx_blood", transform.position + Vector3.up);
        }
        else Fx.Spawn("fx_hit", point);
        Fx.Spawn("fx_blood", point);
    }
}
