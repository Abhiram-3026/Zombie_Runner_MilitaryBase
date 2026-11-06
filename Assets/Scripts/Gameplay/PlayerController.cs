using UnityEngine;

/// <summary>Auto-running soldier: 3 lanes, jump, slide. Collision is handled by triggers on obstacles/zombies/pickups.</summary>
public class PlayerController : MonoBehaviour
{
    public int Lane = 1;
    public PlayerHealth health;
    SoldierAnimator anim;
    CapsuleCollider col;
    GameConfig cfg;
    float x, y, vy, slideT, stepT;
    bool grounded = true, sliding, dead, slideAfterLand;

    public void Init(SoldierAnimator a, PlayerHealth h)
    {
        anim = a; health = h; cfg = GameConfig.I;
        col = GetComponent<CapsuleCollider>();
        SetStanding();
    }

    void SetStanding() { col.direction = 1; col.radius = 0.38f; col.height = 1.8f; col.center = new Vector3(0, 0.9f, 0); }
    void SetSliding() { col.direction = 1; col.radius = 0.38f; col.height = 0.8f; col.center = new Vector3(0, 0.4f, 0); }

    void Update()
    {
        var gm = GameManager.I;
        if (gm == null || gm.State != GameState.Playing || dead) return;
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) Lane = Mathf.Max(0, Lane - 1);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) Lane = Mathf.Min(2, Lane + 1);
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) Jump();
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) Slide();

        float tx = (Lane - 1) * cfg.laneWidth;
        x = Mathf.Lerp(x, tx, 1f - Mathf.Exp(-cfg.laneChangeSharpness * dt));
        if (!grounded)
        {
            vy += cfg.gravity * dt;
            y += vy * dt;
            if (y <= 0f) { y = 0f; vy = 0f; grounded = true; if (slideAfterLand) { slideAfterLand = false; Slide(); } }
        }
        if (sliding) { slideT -= dt; if (slideT <= 0f) EndSlide(); }

        var p = transform.position;
        p.x = x; p.y = y; p.z += gm.CurrentSpeed * dt;
        transform.position = p;

        anim.pose = !grounded ? SoldierAnimator.Pose.Jump : (sliding ? SoldierAnimator.Pose.Slide : SoldierAnimator.Pose.Run);
        anim.lean = tx - x;
        anim.speedFactor = gm.CurrentSpeed;

        if (grounded && !sliding)
        {
            stepT -= dt * gm.CurrentSpeed;
            if (stepT <= 0f) { stepT = 3.4f; AudioManager.I.Play(Sfx.Step, 0.35f, Random.Range(0.9f, 1.1f)); }
        }
    }

    void Jump()
    {
        if (!grounded) return;
        if (sliding) EndSlide();
        vy = cfg.jumpVelocity; grounded = false;
    }

    void Slide()
    {
        if (!grounded) { vy = Mathf.Min(vy, -24f); slideAfterLand = true; return; }
        sliding = true; slideT = cfg.slideDuration; SetSliding();
    }

    void EndSlide() { sliding = false; SetStanding(); }

    public void Die() { dead = true; anim.pose = SoldierAnimator.Pose.Dead; }
}
