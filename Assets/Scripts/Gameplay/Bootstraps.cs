using UnityEngine;

/// <summary>Builds the whole Gameplay scene at runtime (player, camera, managers, lighting, pools).</summary>
public class GameBootstrap : MonoBehaviour
{
    void Awake()
    {
        Gen.Init();
        Time.timeScale = 1f;
        SaveManager.ApplyQuality();
        Collectible.Active.Clear();
        new GameObject("GameConfig").AddComponent<GameConfig>();
        AudioManager.Ensure().ApplyVolumes();
        AudioManager.I.PlayMusic();
        Atmosphere.Apply();

        var pool = new GameObject("ObjectPooler").AddComponent<ObjectPooler>();
        Gen.RegisterAll(pool);

        // player
        var pg = new GameObject("Player");
        var rb = pg.AddComponent<Rigidbody>(); rb.isKinematic = true; rb.useGravity = false;
        pg.AddComponent<CapsuleCollider>().isTrigger = true;
        var pc = pg.AddComponent<PlayerController>();
        var ph = pg.AddComponent<PlayerHealth>();
        var ps = pg.AddComponent<PlayerShooting>();
        SoldierAnimator anim; Transform muzzle;
        var soldier = Gen.MakeSoldier(pg.transform, out anim, out muzzle);
        soldier.SetActive(true);
        var gm = new GameObject("GameManager").AddComponent<GameManager>();
        var pm = new GameObject("PowerUpManager").AddComponent<PowerUpManager>();
        gm.Init(pc, ph, ps);
        ph.Init(anim, pc);
        pc.Init(anim, ph);
        ps.Init(anim, muzzle);
        pm.Init(pg.transform);

        // camera
        var cg = new GameObject("Main Camera"); cg.tag = "MainCamera";
        var cam = cg.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = RenderSettings.fogColor;
        cam.nearClipPlane = 0.1f; cam.farClipPlane = 400f; cam.fieldOfView = 62f;
        cg.AddComponent<AudioListener>();
        cg.AddComponent<CameraFollow>().target = pg.transform;
        cg.transform.position = new Vector3(0, 3.4f, -7.2f);

        // world
        var env = new GameObject("EnvironmentGenerator").AddComponent<EnvironmentGenerator>();
        env.Init(pc, cam);
        new GameObject("SpawnDirector").AddComponent<SpawnDirector>().Init(pc);
        var ui = new GameObject("UIManager").AddComponent<UIManager>();
        ui.Build();
    }
}

public static class Atmosphere
{
    static readonly Color[] DayFog = { new Color(0.72f, 0.80f, 0.88f), new Color(0.84f, 0.78f, 0.66f), new Color(0.74f, 0.82f, 0.76f), new Color(0.78f, 0.83f, 0.92f) };
    static readonly Color[] NightFog = { new Color(0.07f, 0.09f, 0.14f), new Color(0.13f, 0.09f, 0.06f), new Color(0.06f, 0.11f, 0.09f), new Color(0.09f, 0.09f, 0.15f) };
    public static Color ZoneFog(int zone) { return (SaveManager.Day ? DayFog : NightFog)[zone % 4]; }

    public static void Apply()
    {
        bool day = SaveManager.Day;
        RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = day ? 55f : 35f; RenderSettings.fogEndDistance = day ? 200f : 175f;
        RenderSettings.fogColor = ZoneFog(0);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = day ? new Color(0.55f, 0.6f, 0.68f) : new Color(0.2f, 0.22f, 0.28f);
        QualitySettings.shadowDistance = 60f;
        var lg = new GameObject(day ? "Sun" : "Moonlight");
        var l = lg.AddComponent<Light>();
        l.type = LightType.Directional; l.shadows = LightShadows.Soft;
        l.color = day ? new Color(1f, 0.95f, 0.82f) : new Color(0.55f, 0.65f, 0.9f);
        l.intensity = day ? 1.25f : 0.9f;
        lg.transform.rotation = Quaternion.Euler(day ? 48f : 50f, -30f, 0f);
    }
}

/// <summary>Main menu: cinematic 3D backdrop (idle soldier facing a zombie crowd) + UI.</summary>
public class MenuBootstrap : MonoBehaviour
{
    Transform cam;
    void Awake()
    {
        Gen.Init();
        Time.timeScale = 1f;
        SaveManager.ApplyQuality();
        new GameObject("GameConfig").AddComponent<GameConfig>();
        AudioManager.Ensure().ApplyVolumes();
        AudioManager.I.PlayMusic();
        Atmosphere.Apply();

        var s1 = Gen.BuildSegment(0, 0); s1.transform.position = new Vector3(0, 0, -10f); s1.SetActive(true);
        var s2 = Gen.BuildSegment(0, 1); s2.transform.position = new Vector3(0, 0, 30f); s2.SetActive(true);
        var s3 = Gen.BuildSegment(0, 2); s3.transform.position = new Vector3(0, 0, 70f); s3.SetActive(true);

        SoldierAnimator anim; Transform muzzle;
        var holder = new GameObject("MenuSoldier");
        holder.transform.position = new Vector3(1.2f, 0, 2f);
        var sol = Gen.MakeSoldier(holder.transform, out anim, out muzzle);
        anim.pose = SoldierAnimator.Pose.Idle; sol.SetActive(true);

        for (int i = 0; i < 12; i++)
        {
            var type = i % 5 == 0 ? ZombieType.Armored : (i % 3 == 0 ? ZombieType.Fast : ZombieType.Normal);
            var z = Gen.MakeZombie(type);
            z.transform.position = new Vector3(Random.Range(-3.5f, 3.5f), 0, 14f + i * 4.5f);
            z.transform.rotation = Quaternion.Euler(0, 180f + Random.Range(-20f, 20f), 0);
            z.GetComponent<ZombieAI>().menuMode = true;
            z.SetActive(true);
        }
        var cg = new GameObject("Main Camera"); cg.tag = "MainCamera";
        var cm = cg.AddComponent<Camera>();
        cm.clearFlags = CameraClearFlags.SolidColor; cm.backgroundColor = RenderSettings.fogColor; cm.farClipPlane = 300f; cm.fieldOfView = 55f;
        cg.AddComponent<AudioListener>();
        cam = cg.transform;
        new GameObject("MenuUI").AddComponent<MainMenuManager>().Build();
    }

    void Update()
    {
        float t = Time.time;
        cam.position = new Vector3(3.4f + Mathf.Sin(t * 0.3f) * 0.5f, 1.9f + Mathf.Sin(t * 0.2f) * 0.1f, -2.2f);
        cam.LookAt(new Vector3(-0.6f, 1.5f, 24f));
    }
}

/// <summary>Safety net: if a scene named MainMenu/Gameplay has no bootstrap object, create one at load.</summary>
public static class SceneAutoBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Run()
    {
        string n = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (n == "Gameplay" && Object.FindObjectOfType<GameBootstrap>() == null) new GameObject("Bootstrap").AddComponent<GameBootstrap>();
        else if (n == "MainMenu" && Object.FindObjectOfType<MenuBootstrap>() == null) new GameObject("Bootstrap").AddComponent<MenuBootstrap>();
    }
}
