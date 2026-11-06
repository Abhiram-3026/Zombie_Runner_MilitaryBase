using System.Collections.Generic;
using UnityEngine;

/// <summary>Procedural asset factory: materials, primitives, particles, FX and pool registration.</summary>
public static partial class Gen
{
    static readonly Dictionary<string, Material> cache = new Dictionary<string, Material>();
    static Shader stdShader, spriteShader;
    static Texture2D glowTex;
    static bool inited;

    public static Material Asphalt, Concrete, ConcreteDark, Olive, OliveDark, Khaki, Sand, Rust, Metal, MetalDark, Wood, Black,
        Skin, ZSkin, ZSkinFast, Cloth, ClothDark, White, RedPaint, YellowPaint, Gold, Blood, Dirt, Brass, Canvas, Vest,
        WindowGlow, RedGlow, GreenGlow, CyanGlow, OrangeGlow, Glass;

    public static void Init()
    {
        if (inited) return;
        inited = true;
        stdShader = Shader.Find("Standard");
        if (stdShader == null) stdShader = Shader.Find("Legacy Shaders/Diffuse");
        spriteShader = Shader.Find("Sprites/Default");
        Asphalt = M(new Color(0.10f, 0.10f, 0.11f), 0f, 0.2f);
        Concrete = M(new Color(0.36f, 0.36f, 0.34f), 0f, 0.1f);
        ConcreteDark = M(new Color(0.22f, 0.22f, 0.21f), 0f, 0.1f);
        Olive = M(new Color(0.27f, 0.33f, 0.19f), 0f, 0.15f);
        OliveDark = M(new Color(0.16f, 0.21f, 0.12f), 0f, 0.15f);
        Khaki = M(new Color(0.45f, 0.40f, 0.27f), 0f, 0.1f);
        Sand = M(new Color(0.55f, 0.48f, 0.32f), 0f, 0.05f);
        Rust = M(new Color(0.42f, 0.2f, 0.1f), 0.3f, 0.2f);
        Metal = M(new Color(0.45f, 0.47f, 0.5f), 0.8f, 0.5f);
        MetalDark = M(new Color(0.15f, 0.16f, 0.18f), 0.7f, 0.4f);
        Wood = M(new Color(0.33f, 0.22f, 0.12f), 0f, 0.1f);
        Black = M(new Color(0.04f, 0.04f, 0.045f), 0f, 0.2f);
        Skin = M(new Color(0.72f, 0.55f, 0.42f), 0f, 0.3f);
        ZSkin = M(new Color(0.40f, 0.5f, 0.36f), 0f, 0.15f);
        ZSkinFast = M(new Color(0.62f, 0.66f, 0.55f), 0f, 0.15f);
        Cloth = M(new Color(0.25f, 0.22f, 0.18f), 0f, 0.05f);
        ClothDark = M(new Color(0.13f, 0.12f, 0.12f), 0f, 0.05f);
        White = M(new Color(0.85f, 0.85f, 0.82f), 0f, 0.2f);
        RedPaint = M(new Color(0.7f, 0.08f, 0.06f), 0.1f, 0.4f);
        YellowPaint = M(new Color(0.85f, 0.65f, 0.05f), 0.1f, 0.3f);
        Gold = M(new Color(1f, 0.78f, 0.15f), 0.8f, 0.7f, new Color(0.6f, 0.4f, 0.05f));
        Brass = M(new Color(0.8f, 0.6f, 0.2f), 0.8f, 0.6f, new Color(0.3f, 0.2f, 0.03f));
        Blood = M(new Color(0.35f, 0.02f, 0.02f), 0f, 0.5f);
        Dirt = M(new Color(0.2f, 0.17f, 0.13f), 0f, 0.05f);
        Canvas = M(new Color(0.36f, 0.35f, 0.24f), 0f, 0.05f);
        Vest = M(new Color(0.12f, 0.14f, 0.1f), 0f, 0.1f);
        Glass = M(new Color(0.05f, 0.08f, 0.1f), 0.5f, 0.8f);
        WindowGlow = M(new Color(0.9f, 0.75f, 0.35f), 0f, 0.2f, new Color(0.8f, 0.6f, 0.2f));
        RedGlow = M(new Color(1f, 0.1f, 0.05f), 0f, 0.2f, new Color(2f, 0.1f, 0.05f));
        GreenGlow = M(new Color(0.2f, 1f, 0.35f), 0f, 0.2f, new Color(0.2f, 1.6f, 0.4f));
        CyanGlow = M(new Color(0.3f, 0.9f, 1f), 0f, 0.2f, new Color(0.3f, 1.4f, 2f));
        OrangeGlow = M(new Color(1f, 0.5f, 0.1f), 0f, 0.2f, new Color(2f, 0.8f, 0.2f));
    }

    public static Material M(Color c, float metal = 0.05f, float smooth = 0.25f, Color? emis = null)
    {
        string key = c.ToString("F2") + metal + "_" + smooth + (emis.HasValue ? emis.Value.ToString("F2") : "");
        Material m;
        if (cache.TryGetValue(key, out m) && m != null) return m;
        m = new Material(stdShader);
        m.color = c;
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metal);
        if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", smooth);
        if (emis.HasValue)
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", emis.Value);
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        m.enableInstancing = true;
        cache[key] = m;
        return m;
    }

    public static Texture2D GlowTexture()
    {
        if (glowTex != null) return glowTex;
        int n = 64;
        glowTex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        glowTex.wrapMode = TextureWrapMode.Clamp;
        var ctr = new Vector2(n / 2f, n / 2f);
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), ctr) / (n / 2f);
                float a = Mathf.Clamp01(1f - d); a *= a;
                glowTex.SetPixel(x, y, new Color(1, 1, 1, a));
            }
        glowTex.Apply();
        return glowTex;
    }

    /// <summary>Alpha-blended soft glow material (sprite shader + radial texture).</summary>
    public static Material Glow(Color c)
    {
        string key = "glow" + c.ToString("F2");
        Material m;
        if (cache.TryGetValue(key, out m) && m != null) return m;
        m = new Material(spriteShader);
        m.mainTexture = GlowTexture();
        m.color = c;
        cache[key] = m;
        return m;
    }

    /// <summary>Flat translucent colour (no texture) - shields, light beams, tracers.</summary>
    public static Material Flat(Color c)
    {
        string key = "flat" + c.ToString("F2");
        Material m;
        if (cache.TryGetValue(key, out m) && m != null) return m;
        m = new Material(spriteShader);
        m.color = c;
        cache[key] = m;
        return m;
    }

    public static GameObject NewRoot(string n) { var g = new GameObject(n); g.SetActive(false); return g; }

    public static GameObject P(PrimitiveType type, Transform parent, Vector3 pos, Vector3 scale, Material m, Vector3? euler = null)
    {
        var g = GameObject.CreatePrimitive(type);
        var c = g.GetComponent<Collider>();
        if (c != null) { c.enabled = false; Object.Destroy(c); }
        g.transform.SetParent(parent, false);
        g.transform.localPosition = pos;
        g.transform.localScale = scale;
        if (euler.HasValue) g.transform.localEulerAngles = euler.Value;
        g.GetComponent<Renderer>().sharedMaterial = m;
        return g;
    }

    public static GameObject B(Transform p, float x, float y, float z, float sx, float sy, float sz, Material m, Vector3? e = null)
    { return P(PrimitiveType.Cube, p, new Vector3(x, y, z), new Vector3(sx, sy, sz), m, e); }

    public static GameObject Sp(Transform p, float x, float y, float z, float sx, float sy, float sz, Material m)
    { return P(PrimitiveType.Sphere, p, new Vector3(x, y, z), new Vector3(sx, sy, sz), m, null); }

    /// <summary>Cylinder by radius and height (Unity's cylinder is 2 units tall, hence h/2).</summary>
    public static GameObject Cy(Transform p, float x, float y, float z, float r, float h, Material m, Vector3? e = null)
    { return P(PrimitiveType.Cylinder, p, new Vector3(x, y, z), new Vector3(2f * r, h / 2f, 2f * r), m, e); }

    public static Transform Pv(Transform p, string n, float x, float y, float z)
    {
        var g = new GameObject(n);
        g.transform.SetParent(p, false);
        g.transform.localPosition = new Vector3(x, y, z);
        return g.transform;
    }

    public static Transform Holder(Transform p, string n, float x, float y, float z, float yaw)
    {
        var t = Pv(p, n, x, y, z);
        t.localEulerAngles = new Vector3(0, yaw, 0);
        return t;
    }

    /// <summary>Camera-facing glow quad (the gameplay camera looks along +Z, quads face -Z).</summary>
    public static GameObject GlowQuad(Transform p, Vector3 pos, float size, Color c)
    {
        var g = P(PrimitiveType.Quad, p, pos, new Vector3(size, size, 1f), Glow(c), null);
        g.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return g;
    }

    public static Light Lamp(Transform p, Vector3 pos, Color c, float range, float intensity)
    {
        var g = new GameObject("Lamp");
        g.transform.SetParent(p, false);
        g.transform.localPosition = pos;
        var l = g.AddComponent<Light>();
        l.type = LightType.Point; l.color = c; l.range = range; l.intensity = intensity; l.shadows = LightShadows.None;
        return l;
    }

    public static GameObject Label(Transform p, string s, float x, float y, float z, float yaw, float size, Color c)
    {
        var g = new GameObject("Label");
        g.transform.SetParent(p, false);
        g.transform.localPosition = new Vector3(x, y, z);
        g.transform.localEulerAngles = new Vector3(0, yaw, 0);
        var tm = g.AddComponent<TextMesh>();
        tm.text = s; tm.fontSize = 48; tm.characterSize = size;
        tm.anchor = TextAnchor.MiddleCenter; tm.alignment = TextAlignment.Center;
        tm.color = c; tm.fontStyle = FontStyle.Bold;
        var f = UIK.GetFont();
        tm.font = f;
        g.GetComponent<MeshRenderer>().sharedMaterial = f.material;
        return g;
    }

    // ---------------- Particles ----------------
    public static ParticleSystem PS(Transform parent, string name, Color c, float rate, float life, float speed, float size, bool loop, Vector3 pos, float radius = 0.2f, bool world = true, int max = 100)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        var ps = go.AddComponent<ParticleSystem>();
        if (go.activeInHierarchy) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.loop = loop; main.playOnAwake = false;
        main.startLifetime = life; main.startSpeed = speed; main.startSize = size; main.startColor = c;
        main.maxParticles = max;
        main.simulationSpace = world ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
        main.gravityModifier = 0f;
        var em = ps.emission; em.rateOverTime = rate;
        var sh = ps.shape; sh.shapeType = ParticleSystemShapeType.Sphere; sh.radius = radius;
        var col = ps.colorOverLifetime; col.enabled = true;
        var g = new Gradient();
        g.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                  new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.15f), new GradientAlphaKey(0f, 1f) });
        col.color = new ParticleSystem.MinMaxGradient(g);
        var r = go.GetComponent<ParticleSystemRenderer>();
        r.sharedMaterial = Glow(Color.white);
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return ps;
    }

    /// <summary>Makes a particle system emit upward in a narrow cone (fire / smoke columns).</summary>
    public static ParticleSystem Up(ParticleSystem ps, float angle = 12f, float radius = 0.2f)
    {
        var sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Cone; sh.angle = angle; sh.radius = radius;
        sh.rotation = new Vector3(-90f, 0f, 0f);
        return ps;
    }

    public static ParticleSystem Burst(Transform parent, Color c, int count, float speed, float size, float life, float gravity, float radius = 0.1f)
    {
        var ps = PS(parent, "Burst", c, 0f, life, speed, size, false, Vector3.zero, radius, true, count + 10);
        var main = ps.main; main.gravityModifier = gravity;
        var em = ps.emission; em.rateOverTime = 0f;
        em.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });
        return ps;
    }

    static System.Func<GameObject> MakeFx(string key, float life, System.Action<Transform> fill)
    {
        return () =>
        {
            var root = NewRoot(key);
            fill(root.transform);
            root.AddComponent<AutoRelease>().life = life;
            return root;
        };
    }

    static GameObject MakeTracer()
    {
        var root = NewRoot("tracer");
        var lr = root.AddComponent<LineRenderer>();
        lr.positionCount = 2; lr.useWorldSpace = true;
        lr.startWidth = 0.07f; lr.endWidth = 0.02f;
        lr.sharedMaterial = Flat(new Color(1f, 0.9f, 0.5f, 0.9f));
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        root.AddComponent<AutoRelease>().life = 0.07f;
        return root;
    }

    public static GameObject MakeMuzzleFlash(Transform muzzle, out Light l)
    {
        var g = Sp(muzzle, 0, 0, 0.1f, 0.25f, 0.25f, 0.35f, OrangeGlow);
        g.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        l = Lamp(g.transform, Vector3.zero, new Color(1f, 0.7f, 0.3f), 7f, 3f);
        return g;
    }

    // ---------------- Pool registration ----------------
    public static void RegisterAll(ObjectPooler pool)
    {
        Init();
        pool.Register("fx_hit", MakeFx("fx_hit", 0.7f, t => Burst(t, new Color(1f, 0.85f, 0.35f), 12, 7f, 0.14f, 0.35f, 0f)), 6);
        pool.Register("fx_blood", MakeFx("fx_blood", 1.0f, t => Burst(t, new Color(0.6f, 0.05f, 0.05f), 18, 4.5f, 0.2f, 0.6f, 2f)), 6);
        pool.Register("fx_explosion", MakeFx("fx_explosion", 1.7f, t =>
        {
            Burst(t, new Color(1f, 0.55f, 0.1f), 45, 9f, 0.9f, 0.7f, 0f, 0.3f);
            Burst(t, new Color(0.2f, 0.2f, 0.2f, 0.8f), 20, 3f, 1.4f, 1.4f, -0.2f, 0.4f);
        }), 3);
        pool.Register("fx_debris", MakeFx("fx_debris", 1.2f, t => Burst(t, new Color(0.5f, 0.45f, 0.38f), 18, 6f, 0.22f, 0.8f, 2.5f, 0.3f)), 4);
        pool.Register("fx_pickup", MakeFx("fx_pickup", 1.0f, t => Burst(t, new Color(1f, 0.95f, 0.5f), 16, 4f, 0.25f, 0.6f, 0f, 0.2f)), 4);
        pool.Register("fx_claw", MakeFx("fx_claw", 0.9f, t => Burst(t, new Color(0.8f, 0.1f, 0.1f), 12, 5f, 0.18f, 0.5f, 1f, 0.2f)), 3);
        pool.Register("fx_shield", MakeFx("fx_shield", 0.9f, t => Burst(t, new Color(0.3f, 0.7f, 1f), 14, 5f, 0.22f, 0.5f, 0f, 0.6f)), 3);
        pool.Register("tracer", MakeTracer, 8);

        pool.Register("z_normal", () => MakeZombie(ZombieType.Normal), 4);
        pool.Register("z_fast", () => MakeZombie(ZombieType.Fast), 3);
        pool.Register("z_armored", () => MakeZombie(ZombieType.Armored), 2);

        string[] pk = { "p_coin", "p_health", "p_ammo", "p_magnet", "p_shield", "p_speed", "p_double" };
        for (int i = 0; i < pk.Length; i++)
        {
            string k = pk[i];
            pool.Register(k, () => MakePickup(k), k == "p_coin" ? 40 : 1);
        }

        RegisterObstacles(pool, BlockKeys); RegisterObstacles(pool, JumpKeys);
        RegisterObstacles(pool, OverheadKeys); RegisterObstacles(pool, HazardKeys);

        for (int z = 0; z < 4; z++)
            for (int v = 0; v < 3; v++)
            {
                int zz = z, vv = v;
                pool.Register("seg" + z + "_" + v, () => BuildSegment(zz, vv), 0);
            }
    }

    static void RegisterObstacles(ObjectPooler pool, string[] keys)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            string k = keys[i];
            pool.Register(k, () => MakeObstacle(k), 0);
        }
    }
}
