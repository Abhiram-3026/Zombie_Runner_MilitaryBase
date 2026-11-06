using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerShooting : MonoBehaviour
{
    public int Ammo;
    public int Weapon;               // 0 = rifle, 1 = handgun
    public string WeaponName { get { return Weapon == 0 ? "RIFLE" : "HANDGUN"; } }
    Transform muzzle, rifleMuzzle, pistolMuzzle;
    GameObject flashRifle, flashPistol;
    SoldierAnimator anim;
    GameConfig cfg;
    GameObject flashObj;
    float nextFire, emptyT, flashT;

    public void Init(SoldierAnimator a, Transform m)
    {
        anim = a; rifleMuzzle = m; pistolMuzzle = a.pistolMuzzle; cfg = GameConfig.I; Ammo = cfg.startAmmo;
        Light l;
        flashRifle = Gen.MakeMuzzleFlash(rifleMuzzle, out l); flashRifle.SetActive(false);
        flashPistol = Gen.MakeMuzzleFlash(pistolMuzzle, out l); flashPistol.transform.localScale = new Vector3(0.15f, 0.15f, 0.2f); flashPistol.SetActive(false);
        SetWeapon(0);
    }

    public void SetWeapon(int w)
    {
        if (flashObj != null) flashObj.SetActive(false);
        Weapon = w;
        anim.SetWeapon(w == 1);
        muzzle = w == 0 ? rifleMuzzle : pistolMuzzle;
        flashObj = w == 0 ? flashRifle : flashPistol;
        AudioManager.I.Play(Sfx.Click, 0.5f, 1.3f);
        if (UIManager.I != null) UIManager.I.Notify(WeaponName + " EQUIPPED", new Color(0.9f, 0.9f, 0.7f));
    }

    public void AddAmmo(int n) { Ammo = Mathf.Min(cfg.maxAmmo, Ammo + n); }

    void Update()
    {
        var gm = GameManager.I;
        if (gm == null || gm.State != GameState.Playing || gm.health.IsDead) return;
        if (flashT > 0f) { flashT -= Time.deltaTime; if (flashT <= 0f) flashObj.SetActive(false); }
        if (Input.GetKeyDown(KeyCode.Alpha1) && Weapon != 0) SetWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && Weapon != 1) SetWeapon(1);
        if (Input.GetKeyDown(KeyCode.Q)) SetWeapon(1 - Weapon);
        bool over = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        bool want = Input.GetKey(KeyCode.Space) || (Input.GetMouseButton(0) && !over);
        if (want && Time.time >= nextFire) Fire();
    }

    void Fire()
    {
        nextFire = Time.time + (Weapon == 0 ? cfg.fireInterval : cfg.pistolInterval);
        if (Ammo <= 0)
        {
            if (Time.time >= emptyT)
            {
                emptyT = Time.time + 0.6f;
                AudioManager.I.Play(Sfx.Empty);
                UIManager.I.Notify("OUT OF AMMO!", new Color(1f, 0.3f, 0.2f));
            }
            return;
        }
        Ammo--;
        anim.Shoot();
        AudioManager.I.Play(Sfx.Shoot, Weapon == 0 ? 0.7f : 0.55f, (Weapon == 0 ? 1f : 1.35f) * Random.Range(0.94f, 1.06f));
        flashObj.SetActive(true); flashT = 0.05f;

        Vector3 origin = muzzle.position;
        Vector3 end = origin + Vector3.forward * cfg.shootRange;
        Physics.SyncTransforms();
        var hits = Physics.SphereCastAll(origin, 0.35f, Vector3.forward, cfg.shootRange, ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        for (int i = 0; i < hits.Length; i++)
        {
            var sh = hits[i].collider.GetComponentInParent<IShootable>();
            if (sh == null || !sh.CanBeShot) continue;
            end = hits[i].distance <= 0.001f ? origin + Vector3.forward : hits[i].point;
            sh.OnShot(Weapon == 0 ? cfg.bulletDamage : cfg.pistolDamage, end);
            break;
        }
        var tr = ObjectPooler.I.Get("tracer", origin, Quaternion.identity);
        var lr = tr.GetComponent<LineRenderer>();
        lr.SetPosition(0, origin); lr.SetPosition(1, end);
    }
}
