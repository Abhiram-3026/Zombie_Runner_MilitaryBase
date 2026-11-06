using UnityEngine;

public enum PowerUpType { Magnet = 0, Shield = 1, Speed = 2, DoubleCoins = 3 }

/// <summary>Timed power-ups: Magnet, Shield, Speed Boost, Double Coins (Health/Ammo/Coins are instant pickups).</summary>
public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager I;
    readonly float[] rem = new float[4];
    readonly float[] dur = new float[4];
    GameObject shield;
    ParticleSystem speedLines, magnetFx, doubleFx;
    TrailRenderer trail;
    float shieldFlash;

    void Awake() { I = this; }

    public void Init(Transform player)
    {
        shield = Gen.MakeShieldBubble(player); shield.SetActive(false);
        speedLines = Gen.PS(player, "SpeedLines", new Color(0.8f, 1f, 0.85f, 0.7f), 70f, 0.5f, -40f, 0.08f, true, new Vector3(0, 1.5f, 8f), 1f, false, 200);
        var sh = speedLines.shape; sh.shapeType = ParticleSystemShapeType.Box; sh.scale = new Vector3(10f, 5f, 1f);
        var sr = speedLines.GetComponent<ParticleSystemRenderer>(); sr.renderMode = ParticleSystemRenderMode.Stretch; sr.lengthScale = 8f;
        magnetFx = Gen.PS(player, "MagnetFx", new Color(0.9f, 0.4f, 1f, 0.9f), 35f, 0.7f, -2.5f, 0.2f, true, new Vector3(0, 1f, 0.5f), 2.2f, false, 80);
        var ms = magnetFx.shape; ms.shapeType = ParticleSystemShapeType.Circle; ms.radius = 2.2f;
        doubleFx = Gen.PS(player, "DoubleFx", new Color(1f, 0.85f, 0.2f, 0.9f), 20f, 1f, 1.2f, 0.18f, true, new Vector3(0, 1f, 0), 0.6f, false, 50);
        Gen.Up(doubleFx, 25f, 0.6f);
        var tg = new GameObject("BoostTrail"); tg.transform.SetParent(player, false); tg.transform.localPosition = new Vector3(0, 1f, -0.4f);
        trail = tg.AddComponent<TrailRenderer>();
        trail.time = 0.4f; trail.widthMultiplier = 0.8f; trail.sharedMaterial = Gen.Flat(Color.white);
        trail.startColor = new Color(0.3f, 1f, 0.4f, 0.7f); trail.endColor = new Color(0.3f, 1f, 0.4f, 0f);
        trail.emitting = false;
    }

    public bool Active(PowerUpType t) { return rem[(int)t] > 0f; }
    public float Remaining(PowerUpType t) { return rem[(int)t]; }
    public float Duration(PowerUpType t) { return dur[(int)t]; }
    public float SpeedMultiplier { get { return Active(PowerUpType.Speed) ? GameConfig.I.speedBoostMultiplier : 1f; } }
    public int CoinMultiplier { get { return Active(PowerUpType.DoubleCoins) ? 2 : 1; } }

    public void Activate(PowerUpType t, float seconds)
    {
        rem[(int)t] = seconds; dur[(int)t] = seconds;
        AudioManager.I.Play(t == PowerUpType.Shield ? Sfx.Shield : Sfx.PowerUp);
        SetFx(t, true);
    }

    public void ShieldHit()
    {
        shieldFlash = 0.3f;
        Fx.Spawn("fx_shield", GameManager.I.player.transform.position + Vector3.up);
        AudioManager.I.Play(Sfx.Shield, 0.6f, 1.4f);
    }

    void SetFx(PowerUpType t, bool on)
    {
        switch (t)
        {
            case PowerUpType.Shield: shield.SetActive(on); break;
            case PowerUpType.Speed: trail.emitting = on; if (on) speedLines.Play(); else speedLines.Stop(); break;
            case PowerUpType.Magnet: if (on) magnetFx.Play(); else magnetFx.Stop(); break;
            case PowerUpType.DoubleCoins: if (on) doubleFx.Play(); else doubleFx.Stop(); break;
        }
    }

    void Update()
    {
        var gm = GameManager.I;
        if (gm == null || gm.State != GameState.Playing) return;
        float dt = Time.deltaTime;
        for (int i = 0; i < 4; i++)
        {
            if (rem[i] <= 0f) continue;
            rem[i] -= dt;
            if (rem[i] <= 0f) { rem[i] = 0f; SetFx((PowerUpType)i, false); }
        }
        if (shield.activeSelf)
        {
            shieldFlash = Mathf.Max(0f, shieldFlash - dt);
            float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.03f + shieldFlash * 0.25f;
            shield.transform.localScale = new Vector3(pulse, pulse, pulse);
            // blink during the last 2 seconds
            var rs = shield.GetComponentsInChildren<Renderer>();
            bool vis = rem[(int)PowerUpType.Shield] > 2f || Mathf.Repeat(Time.time * 6f, 1f) > 0.4f;
            for (int i = 0; i < rs.Length; i++) rs[i].enabled = vis;
        }
    }
}
