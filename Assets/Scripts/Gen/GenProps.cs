using UnityEngine;

/// <summary>Vehicle / container / barrier props shared by obstacles and scenery, plus the obstacle factory.</summary>
public static partial class Gen
{
    public static readonly string[] BlockKeys = { "ob_truck", "ob_container", "ob_apc", "ob_bus", "ob_supply" };
    public static readonly string[] JumpKeys = { "ob_jersey", "ob_sandbags", "ob_roadblock", "ob_log", "ob_barricade", "ob_corpses", "ob_brokenroad" };
    public static readonly string[] OverheadKeys = { "ob_wire", "ob_beam", "ob_pipes", "ob_cables", "ob_gate" };
    public static readonly string[] HazardKeys = { "ob_mine", "ob_barrel", "ob_fire", "ob_electric" };

    public static float Depth(string k)
    {
        switch (k)
        {
            case "ob_truck": return 5.2f;
            case "ob_container": return 4.2f;
            case "ob_apc": return 4.2f;
            case "ob_bus": return 5.4f;
            case "ob_supply": return 1.6f;
            case "ob_corpses": return 1.8f;
            case "ob_brokenroad": return 2.6f;
            case "ob_fire": return 3f;
            case "ob_sandbags": return 0.9f;
            default: return 0.8f;
        }
    }

    // ---------- vehicles & big props (visual only; callers add colliders) ----------
    public static void Truck(Transform p, float x, float z, float yaw)
    {
        var h = Holder(p, "Truck", x, 0, z, yaw);
        B(h, 0, 0.7f, 0, 2.0f, 0.5f, 5.0f, MetalDark);
        B(h, 0, 1.55f, 1.8f, 2.1f, 1.5f, 1.5f, Olive);
        B(h, 0, 1.95f, 2.56f, 1.7f, 0.6f, 0.05f, Glass);
        B(h, 0, 1.75f, -0.7f, 2.2f, 1.9f, 3.2f, Canvas);
        B(h, 0, 2.75f, -0.7f, 2.0f, 0.1f, 3.0f, OliveDark);
        for (int i = 0; i < 3; i++)
        {
            float zz = 1.7f - i * 1.85f;
            Cy(h, -1.05f, 0.45f, zz, 0.45f, 0.35f, Black, new Vector3(0, 0, 90));
            Cy(h, 1.05f, 0.45f, zz, 0.45f, 0.35f, Black, new Vector3(0, 0, 90));
        }
        B(h, -0.7f, 0.95f, 2.57f, 0.3f, 0.2f, 0.05f, WindowGlow);
        B(h, 0.7f, 0.95f, 2.57f, 0.3f, 0.2f, 0.05f, WindowGlow);
        B(h, 0, 1.1f, 2.6f, 1.2f, 0.4f, 0.08f, MetalDark);
    }

    public static void Container(Transform p, float x, float y, float z, float yaw, Material m, float len)
    {
        var h = Holder(p, "Container", x, y, z, yaw);
        B(h, 0, 1.3f, 0, 2.4f, 2.6f, len, m);
        for (int i = -1; i <= 1; i++)
        {
            B(h, -1.21f, 1.3f, i * (len / 3.5f), 0.05f, 2.5f, 0.14f, MetalDark);
            B(h, 1.21f, 1.3f, i * (len / 3.5f), 0.05f, 2.5f, 0.14f, MetalDark);
        }
        B(h, 0, 1.3f, len / 2f + 0.01f, 2.2f, 2.4f, 0.04f, MetalDark);
        B(h, 0, 1.3f, len / 2f + 0.03f, 0.05f, 2.3f, 0.03f, Metal);
    }

    public static void Apc(Transform p, float x, float z, float yaw)
    {
        var h = Holder(p, "APC", x, 0, z, yaw);
        B(h, 0, 1.1f, 0, 2.2f, 1.1f, 4.0f, Olive);
        B(h, 0, 1.85f, -0.3f, 1.9f, 0.55f, 3.0f, OliveDark);
        B(h, 0, 1.15f, 2.05f, 2.0f, 0.9f, 0.3f, Olive, new Vector3(-25, 0, 0));
        Cy(h, 0, 2.3f, 0.2f, 0.6f, 0.4f, OliveDark);
        Cy(h, 0, 2.35f, 1.4f, 0.08f, 2.0f, MetalDark, new Vector3(90, 0, 0));
        for (int i = 0; i < 3; i++)
        {
            float zz = 1.3f - i * 1.3f;
            Cy(h, -1.1f, 0.5f, zz, 0.5f, 0.4f, Black, new Vector3(0, 0, 90));
            Cy(h, 1.1f, 0.5f, zz, 0.5f, 0.4f, Black, new Vector3(0, 0, 90));
        }
        B(h, 0, 1.3f, 2.2f, 1.0f, 0.18f, 0.04f, Glass);
    }

    public static void Bus(Transform p, float x, float z, float yaw)
    {
        var h = Holder(p, "Bus", x, 0, z, yaw);
        B(h, 0, 1.35f, 0, 2.2f, 2.4f, 5.2f, M(new Color(0.18f, 0.2f, 0.15f), 0.2f, 0.2f));
        for (int i = 0; i < 4; i++)
        {
            B(h, -1.11f, 1.9f, -1.8f + i * 1.2f, 0.03f, 0.6f, 0.8f, Glass);
            B(h, 1.11f, 1.9f, -1.8f + i * 1.2f, 0.03f, 0.6f, 0.8f, Glass);
        }
        B(h, 0, 2.58f, 0, 2.0f, 0.06f, 5.0f, Rust);
        B(h, 0, 0.7f, 2.3f, 2.2f, 0.3f, 0.15f, Rust);
        B(h, 0.4f, 1.9f, 2.61f, 1.4f, 0.8f, 0.04f, Glass);
        Cy(h, -1.0f, 0.4f, 1.6f, 0.42f, 0.3f, Black, new Vector3(0, 0, 90));
        Cy(h, 1.0f, 0.4f, 1.6f, 0.42f, 0.3f, Black, new Vector3(0, 0, 90));
        Cy(h, -1.0f, 0.4f, -1.6f, 0.42f, 0.3f, Black, new Vector3(0, 0, 90));
        Cy(h, 1.0f, 0.4f, -1.6f, 0.42f, 0.3f, Black, new Vector3(0, 0, 90));
        Up(PS(h, "Smoke", new Color(0.25f, 0.25f, 0.25f, 0.6f), 5f, 3f, 1f, 1.1f, true, new Vector3(0, 2.7f, 0), 0.5f, true, 30), 20f, 0.5f);
    }

    public static void CrateStack(Transform p, float x, float z, float yaw)
    {
        var h = Holder(p, "Crates", x, 0, z, yaw);
        B(h, -0.57f, 0.6f, 0, 1.1f, 1.2f, 1.5f, Wood);
        B(h, 0.57f, 0.6f, 0, 1.1f, 1.2f, 1.5f, Olive);
        B(h, -0.57f, 1.8f, 0, 1.1f, 1.2f, 1.5f, Olive);
        B(h, 0.57f, 1.8f, 0, 1.1f, 1.2f, 1.5f, Wood);
        B(h, 0, 2.42f, 0, 2.2f, 0.05f, 1.5f, Canvas);
        B(h, -0.57f, 0.6f, -0.76f, 0.7f, 0.5f, 0.02f, YellowPaint);
    }

    public static void CarWreck(Transform p, float x, float z, float yaw)
    {
        var h = Holder(p, "Wreck", x, 0, z, yaw);
        Material rust = M(new Color(0.25f, 0.14f, 0.1f), 0.2f, 0.1f);
        B(h, 0, 0.65f, 0, 1.9f, 0.8f, 3.8f, rust);
        B(h, 0, 1.25f, -0.2f, 1.6f, 0.55f, 1.9f, rust);
        B(h, 0, 1.28f, -0.2f, 1.62f, 0.4f, 1.5f, Glass);
        Cy(h, -0.95f, 0.3f, 1.2f, 0.32f, 0.22f, Black, new Vector3(0, 0, 90));
        Cy(h, 0.95f, 0.3f, -1.2f, 0.32f, 0.22f, Black, new Vector3(0, 0, 90));
    }

    public static void Barrel(Transform p, float x, float y, float z, Material m)
    {
        Cy(p, x, y + 0.55f, z, 0.38f, 1.1f, m);
        Cy(p, x, y + 0.25f, z, 0.4f, 0.06f, Black);
        Cy(p, x, y + 0.85f, z, 0.4f, 0.06f, Black);
    }

    public static void Sandbags(Transform p, float x, float z, float yaw, int cols, int rows)
    {
        var h = Holder(p, "Sandbags", x, 0, z, yaw);
        for (int r = 0; r < rows; r++)
            for (int i = 0; i < cols; i++)
                B(h, -(cols - 1) * 0.25f + i * 0.5f + (r % 2) * 0.22f, 0.14f + r * 0.25f, 0, 0.52f, 0.26f, 0.7f, Sand, new Vector3(0, 0, Random.Range(-4f, 4f)));
    }

    public static void FireBarrel(Transform p, float x, float z)
    {
        Barrel(p, x, 0, z, Rust);
        Up(PS(p, "Fire", new Color(1f, 0.55f, 0.15f, 0.9f), 18f, 0.8f, 1.6f, 0.7f, true, new Vector3(x, 1.2f, z), 0.2f, true, 40));
        Up(PS(p, "Smoke", new Color(0.3f, 0.3f, 0.3f, 0.5f), 5f, 2.8f, 1.3f, 1.0f, true, new Vector3(x, 1.6f, z), 0.2f, true, 30), 20f);
    }

    // ---------- obstacle factory ----------
    static GameObject Ob(GameObject root, ObKind kind, ObDamage dmg, Vector3 center, Vector3 size, bool explodes = false, bool destructible = false, bool persists = false)
    {
        var bc = root.AddComponent<BoxCollider>();
        bc.isTrigger = true; bc.center = center; bc.size = size;
        var o = root.AddComponent<Obstacle>();
        o.kind = kind; o.dmg = dmg; o.explodes = explodes; o.destructible = destructible; o.persists = persists;
        return root;
    }

    static Vector3 OverheadCenter() { return new Vector3(0, 1.85f, 0); }

    public static GameObject MakeObstacle(string key)
    {
        Init();
        var root = NewRoot(key);
        var t = root.transform;
        switch (key)
        {
            // ---- ground blocks (must change lane) ----
            case "ob_truck": Truck(t, 0, 0, 0); return Ob(root, ObKind.Block, ObDamage.Block, new Vector3(0, 1.35f, 0), new Vector3(2.3f, 2.7f, 5.2f));
            case "ob_container":
                {
                    Material[] cols = { Rust, M(new Color(0.15f, 0.25f, 0.4f), 0.2f, 0.2f), OliveDark, M(new Color(0.5f, 0.4f, 0.1f), 0.2f, 0.2f) };
                    Container(t, 0, 0, 0, 0, cols[Random.Range(0, cols.Length)], 4.2f);
                    return Ob(root, ObKind.Block, ObDamage.Block, new Vector3(0, 1.3f, 0), new Vector3(2.4f, 2.6f, 4.2f));
                }
            case "ob_apc": Apc(t, 0, 0, 0); return Ob(root, ObKind.Block, ObDamage.Block, new Vector3(0, 1.3f, 0), new Vector3(2.3f, 2.6f, 4.2f));
            case "ob_bus": Bus(t, 0, 0, 0); return Ob(root, ObKind.Block, ObDamage.Block, new Vector3(0, 1.3f, 0), new Vector3(2.3f, 2.6f, 5.4f));
            case "ob_supply": CrateStack(t, 0, 0, 0); return Ob(root, ObKind.Block, ObDamage.Block, new Vector3(0, 1.2f, 0), new Vector3(2.3f, 2.4f, 1.6f));

            // ---- low obstacles (must jump) ----
            case "ob_jersey":
                B(t, 0, 0.2f, 0, 2.1f, 0.4f, 0.7f, Concrete); B(t, 0, 0.62f, 0, 2.1f, 0.45f, 0.4f, Concrete);
                B(t, -0.5f, 0.62f, -0.21f, 0.5f, 0.3f, 0.02f, RedPaint); B(t, 0.5f, 0.62f, -0.21f, 0.5f, 0.3f, 0.02f, White);
                return Ob(root, ObKind.Jump, ObDamage.Jump, new Vector3(0, 0.45f, 0), new Vector3(2.2f, 0.9f, 0.8f));
            case "ob_sandbags": Sandbags(t, 0, 0, 0, 4, 3); return Ob(root, ObKind.Jump, ObDamage.Jump, new Vector3(0, 0.4f, 0), new Vector3(2.2f, 0.8f, 0.9f));
            case "ob_roadblock":
                B(t, -0.95f, 0.4f, 0, 0.12f, 0.8f, 0.12f, Metal); B(t, 0.95f, 0.4f, 0, 0.12f, 0.8f, 0.12f, Metal);
                B(t, 0, 0.65f, 0, 2.1f, 0.22f, 0.08f, RedPaint); B(t, 0, 0.4f, 0, 2.1f, 0.22f, 0.08f, White);
                B(t, 0, 0.17f, 0, 2.1f, 0.2f, 0.08f, RedPaint);
                return Ob(root, ObKind.Jump, ObDamage.Jump, new Vector3(0, 0.42f, 0), new Vector3(2.2f, 0.85f, 0.4f));
            case "ob_log":
                Cy(t, 0, 0.36f, 0, 0.36f, 2.1f, Wood, new Vector3(0, 0, 90));
                B(t, -0.7f, 0.7f, 0.05f, 0.08f, 0.3f, 0.08f, Wood, new Vector3(0, 0, 25));
                return Ob(root, ObKind.Jump, ObDamage.Jump, new Vector3(0, 0.4f, 0), new Vector3(2.2f, 0.8f, 0.8f));
            case "ob_barricade":
                B(t, -0.55f, 0.4f, 0, 0.1f, 1.0f, 0.1f, Wood, new Vector3(0, 0, 25)); B(t, -0.55f, 0.4f, 0, 0.1f, 1.0f, 0.1f, Wood, new Vector3(0, 0, -25));
                B(t, 0.55f, 0.4f, 0, 0.1f, 1.0f, 0.1f, Wood, new Vector3(0, 0, 25)); B(t, 0.55f, 0.4f, 0, 0.1f, 1.0f, 0.1f, Wood, new Vector3(0, 0, -25));
                B(t, 0, 0.62f, 0, 2.0f, 0.2f, 0.08f, Wood); B(t, 0, 0.3f, 0, 2.0f, 0.15f, 0.08f, Wood);
                return Ob(root, ObKind.Jump, ObDamage.Jump, new Vector3(0, 0.42f, 0), new Vector3(2.1f, 0.85f, 0.4f));
            case "ob_corpses":
                Sp(t, -0.6f, 0.2f, 0, 0.8f, 0.35f, 1.2f, Cloth); Sp(t, 0.3f, 0.25f, 0.3f, 0.9f, 0.4f, 1.3f, ClothDark); Sp(t, 0f, 0.15f, -0.5f, 0.7f, 0.3f, 0.9f, Cloth);
                Sp(t, -0.6f, 0.25f, 0.65f, 0.25f, 0.25f, 0.25f, ZSkin); Sp(t, 0.35f, 0.3f, 0.95f, 0.25f, 0.25f, 0.25f, ZSkin);
                B(t, 0.8f, 0.12f, -0.4f, 0.4f, 0.24f, 0.3f, Concrete, new Vector3(0, 30, 0)); B(t, 0.1f, 0.03f, 0, 1.6f, 0.04f, 1.2f, Blood);
                return Ob(root, ObKind.Jump, ObDamage.Jump, new Vector3(0, 0.3f, 0), new Vector3(2.1f, 0.6f, 1.8f));
            case "ob_brokenroad":
                B(t, 0, 0.015f, 0, 2.2f, 0.03f, 2.6f, Black);
                for (int i = 0; i < 8; i++) B(t, Random.Range(-1.1f, 1.1f), 0.1f, (i % 2 == 0 ? 1f : -1f) * Random.Range(1.0f, 1.3f), Random.Range(0.2f, 0.5f), Random.Range(0.1f, 0.3f), Random.Range(0.2f, 0.4f), ConcreteDark, new Vector3(0, Random.Range(0, 90f), 0));
                for (int i = 0; i < 3; i++) Cy(t, -0.7f + i * 0.7f, 0.25f, 1.2f, 0.02f, 0.5f, Rust, new Vector3(20, 0, Random.Range(-15f, 15f)));
                return Ob(root, ObKind.Jump, ObDamage.Jump, new Vector3(0, 0.25f, 0), new Vector3(2.2f, 0.5f, 2.6f));

            // ---- overhead (must slide) ----
            case "ob_wire":
                B(t, -1.15f, 1.4f, 0, 0.12f, 2.8f, 0.12f, Wood); B(t, 1.15f, 1.4f, 0, 0.12f, 2.8f, 0.12f, Wood);
                for (int i = 0; i < 4; i++) B(t, 0, 1.3f + i * 0.3f, 0, 2.3f, 0.03f, 0.03f, MetalDark);
                for (int i = 0; i < 6; i++) Sp(t, -1f + i * 0.4f, 1.5f + (i % 2) * 0.35f, 0, 0.24f, 0.24f, 0.24f, MetalDark);
                return Ob(root, ObKind.Overhead, ObDamage.Overhead, OverheadCenter(), new Vector3(2.4f, 1.5f, 0.5f));
            case "ob_beam":
                B(t, -1.1f, 1.4f, 0, 0.3f, 2.8f, 0.5f, ConcreteDark); B(t, 1.1f, 1.4f, 0, 0.3f, 2.8f, 0.5f, ConcreteDark);
                B(t, 0, 1.6f, 0, 2.4f, 0.5f, 0.7f, MetalDark); B(t, 0, 1.9f, 0, 2.4f, 0.1f, 0.9f, Metal);
                B(t, 0.3f, 2.4f, 0, 0.04f, 0.9f, 0.04f, Black);
                return Ob(root, ObKind.Overhead, ObDamage.Overhead, OverheadCenter(), new Vector3(2.4f, 1.5f, 0.8f));
            case "ob_pipes":
                B(t, -1.1f, 1.5f, 0, 0.2f, 3f, 0.3f, ConcreteDark); B(t, 1.1f, 1.5f, 0, 0.2f, 3f, 0.3f, ConcreteDark);
                Cy(t, 0, 1.3f, 0, 0.14f, 2.3f, Rust, new Vector3(0, 0, 90)); Cy(t, 0, 1.62f, 0, 0.14f, 2.3f, Metal, new Vector3(0, 0, 90)); Cy(t, 0, 1.95f, 0, 0.14f, 2.3f, Rust, new Vector3(0, 0, 90));
                Cy(t, 0.3f, 2.2f, 0, 0.12f, 0.06f, RedPaint, new Vector3(90, 0, 0));
                return Ob(root, ObKind.Overhead, ObDamage.Overhead, OverheadCenter(), new Vector3(2.4f, 1.5f, 0.5f));
            case "ob_cables":
                B(t, 0, 3.05f, 0, 2.4f, 0.15f, 0.3f, MetalDark);
                for (int i = 0; i < 5; i++)
                {
                    float x = -0.95f + i * 0.475f;
                    Cy(t, x, 2.1f, 0, 0.035f, 1.9f, Black);
                    Sp(t, x, 1.15f, 0, 0.12f, 0.12f, 0.12f, CyanGlow);
                }
                PS(t, "Sparks", new Color(0.5f, 0.9f, 1f, 1f), 12f, 0.3f, 1.2f, 0.1f, true, new Vector3(0, 1.2f, 0), 0.8f, true, 30);
                return Ob(root, ObKind.Overhead, ObDamage.Overhead, OverheadCenter(), new Vector3(2.4f, 1.5f, 0.5f));
            case "ob_gate":
                B(t, -1.1f, 1.1f, 0, 0.25f, 2.2f, 0.3f, Concrete);
                for (int i = 0; i < 4; i++) Cy(t, -0.7f + i * 0.5f, 1.55f, 0, 0.07f, 0.5f, i % 2 == 0 ? RedPaint : White, new Vector3(0, 0, 90));
                B(t, 0, 2.3f, 0, 2.3f, 0.45f, 0.1f, RedPaint);
                Label(t, "STOP", 0, 2.3f, -0.07f, 0, 0.09f, Color.white);
                B(t, -1.1f, 2.3f, 0, 0.1f, 0.15f, 0.1f, RedGlow);
                return Ob(root, ObKind.Overhead, ObDamage.Overhead, OverheadCenter(), new Vector3(2.4f, 1.5f, 0.5f));

            // ---- hazards ----
            case "ob_mine":
                Cy(t, 0, 0.06f, 0, 0.3f, 0.12f, OliveDark); B(t, 0, 0.14f, 0, 0.12f, 0.05f, 0.12f, RedGlow);
                GlowQuad(t, new Vector3(0, 0.3f, -0.1f), 0.8f, new Color(1f, 0.1f, 0.05f, 0.5f));
                return Ob(root, ObKind.Hazard, ObDamage.Mine, new Vector3(0, 0.3f, 0), new Vector3(0.9f, 0.6f, 0.9f), true);
            case "ob_barrel":
                Barrel(t, 0, 0, 0, RedPaint);
                B(t, 0, 0.55f, -0.39f, 0.25f, 0.25f, 0.02f, YellowPaint); Label(t, "!", 0, 0.55f, -0.41f, 0, 0.05f, Color.black);
                return Ob(root, ObKind.Hazard, ObDamage.Barrel, new Vector3(0, 0.55f, 0), new Vector3(0.9f, 1.1f, 0.9f), true, true);
            case "ob_fire":
                B(t, 0, 0.02f, 0, 2.2f, 0.04f, 3f, Black);
                B(t, -0.6f, 0.1f, 0.8f, 0.5f, 0.2f, 0.4f, ConcreteDark, new Vector3(0, 20, 0)); B(t, 0.7f, 0.1f, -0.9f, 0.4f, 0.2f, 0.5f, ConcreteDark, new Vector3(0, -30, 0));
                Up(PS(t, "Flames", new Color(1f, 0.5f, 0.1f, 0.95f), 32f, 0.8f, 2.2f, 1.2f, true, new Vector3(0, 0.2f, 0), 0.9f, true, 60), 15f, 0.9f);
                Up(PS(t, "Smoke", new Color(0.25f, 0.25f, 0.25f, 0.55f), 6f, 2.5f, 1.4f, 1.3f, true, new Vector3(0, 1.2f, 0), 0.7f, true, 30), 20f, 0.7f);
                Lamp(t, new Vector3(0, 1.2f, 0), new Color(1f, 0.5f, 0.15f), 7f, 1.6f);
                return Ob(root, ObKind.Hazard, ObDamage.Fire, new Vector3(0, 0.45f, 0), new Vector3(2.2f, 0.9f, 3f), false, false, true);
            case "ob_electric":
                B(t, -1.1f, 0.8f, 0, 0.12f, 1.6f, 0.12f, MetalDark); B(t, 1.1f, 0.8f, 0, 0.12f, 1.6f, 0.12f, MetalDark);
                for (int i = 0; i < 3; i++) B(t, 0, 0.45f + i * 0.4f, 0, 2.2f, 0.05f, 0.05f, Metal);
                for (int i = 0; i < 5; i++) B(t, -0.9f + i * 0.45f, 0.7f + (i % 2) * 0.4f, 0, 0.5f, 0.04f, 0.04f, CyanGlow, new Vector3(0, 0, (i % 2) * 60 - 30));
                PS(t, "Sparks", new Color(0.5f, 0.9f, 1f, 1f), 25f, 0.3f, 1.5f, 0.12f, true, new Vector3(0, 0.8f, 0), 0.8f, true, 40);
                Lamp(t, new Vector3(0, 0.9f, -0.5f), new Color(0.3f, 0.8f, 1f), 5f, 1.8f);
                return Ob(root, ObKind.Hazard, ObDamage.Electric, new Vector3(0, 0.7f, 0), new Vector3(2.3f, 1.4f, 0.5f), false, false, true);
        }
        return root;
    }
}
