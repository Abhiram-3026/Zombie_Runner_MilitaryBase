using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float hp, max;
    public bool IsDead;
    float iframe;
    SoldierAnimator anim;
    PlayerController pc;

    public void Init(SoldierAnimator a, PlayerController p) { anim = a; pc = p; max = GameConfig.I.maxHealth; hp = max; }
    void Update() { if (iframe > 0f) iframe -= Time.deltaTime; }

    /// <summary>Returns true if damage was actually applied (false when shielded / invulnerable / dead).</summary>
    public bool TakeDamage(float amount, string source)
    {
        var gm = GameManager.I;
        if (IsDead || gm == null || gm.State != GameState.Playing) return false;
        if (PowerUpManager.I != null && PowerUpManager.I.Active(PowerUpType.Shield)) { PowerUpManager.I.ShieldHit(); return false; }
        if (iframe > 0f) return false;
        hp = Mathf.Max(0f, hp - amount * SaveManager.DamageScale);
        iframe = GameConfig.I.invulnerableTime;
        if (anim != null) anim.Hit();
        if (CameraFollow.I != null) CameraFollow.I.Shake(0.35f);
        if (UIManager.I != null) UIManager.I.Flash();
        AudioManager.I.Play(Sfx.Damage);
        if (hp <= 0f) Die();
        return true;
    }

    public void Heal(float a) { hp = Mathf.Min(max, hp + a); }

    void Die()
    {
        IsDead = true;
        pc.Die();
        AudioManager.I.Play(Sfx.GameOver);
        GameManager.I.TriggerGameOver();
    }
}
