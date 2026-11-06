using UnityEngine;

/// <summary>Procedural soldier animation: run, jump, slide, shoot recoil, hit flinch, death.</summary>
public class SoldierAnimator : MonoBehaviour
{
    public enum Pose { Idle, Run, Jump, Slide, Dead }
    public Transform model, legL, legR, armL, armR, head, rifle, pistol, rifleMuzzle, pistolMuzzle;
    public bool pistolMode;
    Vector3 pistolBase;
    public Pose pose = Pose.Run;
    public float lean, speedFactor = 14f;
    float phase, shootT, hitT, deadT;
    Vector3 rifleBase;
    bool baseSet;

    public void Shoot() { shootT = 1f; }
    public void Hit() { hitT = 1f; }
    public void SetWeapon(bool usePistol)
    {
        pistolMode = usePistol;
        if (rifle != null) rifle.gameObject.SetActive(!usePistol);
        if (pistol != null) pistol.gameObject.SetActive(usePistol);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f || model == null) return;
        if (!baseSet && rifle != null) { rifleBase = rifle.localPosition; if (pistol != null) pistolBase = pistol.localPosition; baseSet = true; }
        phase += dt * (8f + speedFactor * 0.18f);
        shootT = Mathf.Max(0f, shootT - dt * 7f);
        hitT = Mathf.Max(0f, hitT - dt * 3.5f);
        float s = Mathf.Sin(phase);
        float legA = 0f, legB = 0f, mx = 0f, my = 0f, armSwing = 0f;
        switch (pose)
        {
            case Pose.Run: legA = s * 55f; legB = -s * 55f; my = Mathf.Abs(Mathf.Cos(phase)) * 0.09f; mx = 9f; armSwing = s * 5f; break;
            case Pose.Jump: legA = -40f; legB = 30f; mx = 4f; break;
            case Pose.Slide: legA = -5f; legB = -12f; mx = -72f; my = 0.14f; break;
            case Pose.Dead:
                deadT = Mathf.Min(1f, deadT + dt * 2.2f);
                mx = Mathf.Lerp(0f, -88f, deadT); my = Mathf.Lerp(0f, 0.12f, deadT);
                legA = Mathf.Lerp(0f, -10f, deadT); legB = Mathf.Lerp(0f, 15f, deadT);
                break;
            default: mx = Mathf.Sin(Time.time * 2f) * 2f; break;
        }
        legL.localRotation = Quaternion.Euler(legA, 0, 0);
        legR.localRotation = Quaternion.Euler(legB, 0, 0);
        float armBase = pose == Pose.Dead ? 5f : -80f;
        if (pistolMode && pose != Pose.Dead)
        {
            // one-handed pistol: right arm aims forward, left arm swings freely while running
            armR.localRotation = Quaternion.Euler(-85f + armSwing * 0.4f - shootT * 14f, 0, 0);
            float freeSwing = pose == Pose.Run ? -s * 50f : (pose == Pose.Jump ? -30f : -10f);
            armL.localRotation = Quaternion.Euler(freeSwing, 0, 0);
            if (pistol != null) pistol.localPosition = pistolBase + new Vector3(0, 0.02f * shootT, -0.08f * shootT);
        }
        else
        {
            armR.localRotation = Quaternion.Euler(armBase + armSwing - shootT * 9f, 0, 0);
            armL.localRotation = Quaternion.Euler(armBase - 4f - armSwing, 12f, 0);
            if (rifle != null) rifle.localPosition = rifleBase + new Vector3(0, 0, -0.12f * shootT);
        }
        float shake = Mathf.Sin(Time.time * 70f) * hitT * 7f;
        var target = Quaternion.Euler(mx - hitT * 22f - shootT * 2f, 0, lean * -6f + shake);
        float k = 1f - Mathf.Exp(-20f * dt);
        model.localRotation = Quaternion.Slerp(model.localRotation, target, k);
        model.localPosition = Vector3.Lerp(model.localPosition, new Vector3(0, my, 0), k);
    }
}
