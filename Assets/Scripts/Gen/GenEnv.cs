using UnityEngine;

/// <summary>Modular 40 m road segments in four military zones: Entrance, Supply Depot, Barracks, Runway/Vehicle Zone.</summary>
public static partial class Gen
{
    public const float SegLen = 40f;
    public static readonly string[] ZoneNames = { "BASE ENTRANCE", "SUPPLY DEPOT", "BARRACKS", "RUNWAY / VEHICLE ZONE" };

    public static GameObject BuildSegment(int zone, int variant)
    {
        Init();
        var root = NewRoot("Segment_" + zone + "_" + variant);
        var t = root.transform;
        float L = SegLen;
        Material ground = zone == 3 ? ConcreteDark : Dirt;

        // road + shoulders + curbs + markings
        B(t, 0, -0.1f, L / 2, 7.8f, 0.2f, L, Asphalt);
        B(t, -9.2f, -0.12f, L / 2, 10.6f, 0.16f, L, ground);
        B(t, 9.2f, -0.12f, L / 2, 10.6f, 0.16f, L, ground);
        B(t, -3.95f, 0.08f, L / 2, 0.3f, 0.3f, L, Concrete);
        B(t, 3.95f, 0.08f, L / 2, 0.3f, 0.3f, L, Concrete);
        for (int zz = 2; zz < (int)L; zz += 6)
        {
            B(t, -1.25f, 0.012f, zz, 0.12f, 0.02f, 2.4f, White);
            B(t, 1.25f, 0.012f, zz, 0.12f, 0.02f, 2.4f, White);
        }
        B(t, -3.6f, 0.012f, L / 2, 0.1f, 0.02f, L, YellowPaint);
        B(t, 3.6f, 0.012f, L / 2, 0.1f, 0.02f, L, YellowPaint);
        // cracks, damage patches, broken road
        for (int i = 0; i < 5; i++)
            B(t, Random.Range(-3.4f, 3.4f), 0.014f, Random.Range(2f, 38f), Random.Range(0.3f, 1.3f), 0.02f, Random.Range(0.3f, 1.6f), Black, new Vector3(0, Random.Range(0, 180f), 0));
        for (int i = 0; i < 2; i++)
            B(t, Random.Range(-3.2f, 3.2f), 0.03f, Random.Range(2f, 38f), Random.Range(0.2f, 0.5f), 0.05f, Random.Range(0.2f, 0.5f), ConcreteDark, new Vector3(0, Random.Range(0, 90f), 0));

        if (zone == 3) RunwayMarkings(t);
        if (zone != 1) { Fence(t, -6.2f, L); Fence(t, 6.2f, L); }

        // scenery slots on both sides
        float[] slots = { 5f, 15f, 25f, 35f };
        for (int side = -1; side <= 1; side += 2)
            for (int s = 0; s < slots.Length; s++)
            {
                float z = slots[s] + Random.Range(-2f, 2f);
                float x = side * Random.Range(7.5f, 11.5f);
                Decor(zone, Random.Range(0, 10), t, x, z, side);
            }

        // floodlights with one real light per segment
        Floodlight(t, -5.4f, 10f + variant * 6f, true, zone);
        Floodlight(t, 5.4f, 30f - variant * 5f, false, zone);

        // zone gate arch + sign
        if (variant == 0) Arch(t, 20f, zone);
        // pickup-style atmosphere: a few fire barrels
        if (variant == 1) FireBarrel(t, (Random.value < 0.5f ? -1 : 1) * 5.2f, Random.Range(6f, 34f));
        return root;
    }

    static void Decor(int zone, int id, Transform t, float x, float z, int side)
    {
        float yaw = side < 0 ? 90f : -90f;
        switch (zone)
        {
            case 0: // Military Base Entrance
                switch (id % 8)
                {
                    case 0: Tower(t, x, z); break;
                    case 1: Booth(t, x * 0.75f, z); break;
                    case 2: Bunker(t, x, z); break;
                    case 3: WallPiece(t, x * 0.8f, z, 9f); break;
                    case 4: Sign(t, side * 4.6f, z, "DANGER"); break;
                    case 5: CarWreck(t, x * 0.85f, z, Random.Range(-30f, 30f)); break;
                    case 6: Sandbags(t, side * 5f, z, 0f, 5, 3); break;
                    default: Tent(t, x, z); break;
                }
                break;
            case 1: // Supply Depot
                switch (id % 8)
                {
                    case 0: StackedContainers(t, x, z, yaw); break;
                    case 1: CrateStack(t, x * 0.85f, z, Random.Range(-20f, 20f)); break;
                    case 2: Truck(t, x * 0.85f, z, Random.Range(-10f, 10f)); break;
                    case 3: Tent(t, x, z); break;
                    case 4: Warehouse(t, x + side * 3f, z); break;
                    case 5: BarrelGroup(t, x * 0.7f, z); break;
                    case 6: StackedContainers(t, x * 0.9f, z, yaw + 5f); break;
                    default: Sign(t, side * 4.6f, z, "SUPPLIES"); break;
                }
                break;
            case 2: // Barracks
                switch (id % 8)
                {
                    case 0: case 1: Barracks(t, x + side * 2f, z); break;
                    case 2: Tent(t, x, z); break;
                    case 3: Bunker(t, x, z); break;
                    case 4: Tower(t, x, z); break;
                    case 5: FlagPole(t, x * 0.8f, z); break;
                    case 6: Truck(t, x * 0.85f, z, Random.Range(-10f, 10f)); break;
                    default: HescoRow(t, side * 5.3f, z); break;
                }
                break;
            default: // Runway / Vehicle zone
                switch (id % 8)
                {
                    case 0: Hangar(t, x + side * 5f, z); break;
                    case 1: ControlTower(t, x + side * 2f, z); break;
                    case 2: Apc(t, x * 0.9f, z, Random.Range(-40f, 40f)); break;
                    case 3: CarWreck(t, x * 0.9f, z, Random.Range(-50f, 50f)); break;
                    case 4: BarrelGroup(t, x * 0.8f, z); break;
                    case 5: StackedContainers(t, x, z, yaw); break;
                    case 6: Truck(t, x * 0.9f, z, Random.Range(-20f, 20f)); break;
                    default: Sign(t, side * 4.6f, z, "RESTRICTED"); break;
                }
                break;
        }
    }

    // ---------- scenery pieces ----------
    static void Fence(Transform t, float x, float L)
    {
        for (float z = 0; z <= L; z += 4f) B(t, x, 0.9f, z, 0.1f, 1.8f, 0.1f, Wood);
        for (int i = 0; i < 3; i++) B(t, x, 0.55f + i * 0.4f, L / 2, 0.03f, 0.03f, L, MetalDark);
        for (float z = 1f; z < L; z += 2.5f) Sp(t, x, 1.9f, z, 0.2f, 0.2f, 0.2f, MetalDark);   // barbed coils
        B(t, x, 1.82f, L / 2, 0.03f, 0.03f, L, MetalDark);
    }

    static void Tower(Transform t, float x, float z)
    {
        var h = Holder(t, "Tower", x, 0, z, 0);
        foreach (float sx in new[] { -1f, 1f }) foreach (float sz in new[] { -1f, 1f }) B(h, sx, 3f, sz, 0.25f, 6f, 0.25f, Wood);
        B(h, 0, 6.6f, 0, 3f, 1.6f, 3f, OliveDark);
        B(h, 0, 7.55f, 0, 3.6f, 0.2f, 3.6f, MetalDark);
        B(h, 1.52f, 6.7f, 0, 0.04f, 0.5f, 1.4f, WindowGlow); B(h, -1.52f, 6.7f, 0, 0.04f, 0.5f, 1.4f, WindowGlow);
        B(h, 0, 6.1f, 0, 3.3f, 0.1f, 3.3f, Wood);
        B(h, 0, 8.0f, 0, 0.3f, 0.5f, 0.5f, MetalDark);                                         // searchlight
        if (!SaveManager.Day) B(h, 0, 11f, 4f, 0.5f, 0.02f, 9f, Flat(new Color(1f, 1f, 0.8f, 0.07f)), new Vector3(-35f, 0, 0)); // light beam
        for (int i = 0; i < 4; i++) B(h, 0, 1f + i * 1.5f, 0, 2.0f, 0.06f, 0.06f, Wood, new Vector3(0, 0, 0));
    }

    static void Booth(Transform t, float x, float z)
    {
        B(t, x, 1.2f, z, 2.2f, 2.4f, 2.2f, Concrete);
        B(t, x, 2.5f, z, 2.6f, 0.2f, 2.6f, MetalDark);
        B(t, x * 0.97f, 1.6f, z, 0.05f, 0.7f, 1.2f, WindowGlow);
        B(t, x, 0.4f, z - 1.3f, 1.8f, 0.8f, 0.4f, Sand);
    }

    static void Bunker(Transform t, float x, float z)
    {
        Sp(t, x, 0.4f, z, 5f, 2.4f, 4f, Sand);
        B(t, x, 0.8f, z - 1.9f, 1.4f, 1.2f, 0.4f, Black);
        Sandbags(t, x, z - 2.4f, 0f, 5, 2);
    }

    static void WallPiece(Transform t, float x, float z, float len)
    {
        B(t, x, 1.4f, z, 0.5f, 2.8f, len, Concrete);
        B(t, x, 2.85f, z, 0.7f, 0.12f, len, ConcreteDark);
        for (int i = 0; i < 4; i++) B(t, x, 1.4f, z - len / 2f + i * (len / 3f), 0.6f, 2.6f, 0.1f, ConcreteDark);
        B(t, x * 0.97f, 1.6f, z + 1f, 0.02f, 0.5f, 1.2f, Blood);
    }

    static void Sign(Transform t, float x, float z, string txt)
    {
        Cy(t, x, 1.2f, z, 0.05f, 2.4f, Metal);
        B(t, x, 2.2f, z, 1.7f, 0.9f, 0.08f, YellowPaint);
        Label(t, txt, x, 2.2f, z - 0.06f, 0, 0.045f, Color.black);
    }

    static void Tent(Transform t, float x, float z)
    {
        var h = Holder(t, "Tent", x, 0, z, Random.Range(-15f, 15f));
        B(h, -0.9f, 1.2f, 0, 0.06f, 2.4f, 4f, Canvas, new Vector3(0, 0, -38f));
        B(h, 0.9f, 1.2f, 0, 0.06f, 2.4f, 4f, Canvas, new Vector3(0, 0, 38f));
        B(h, 0, 0.9f, 2.0f, 2.0f, 1.8f, 0.05f, OliveDark);
    }

    static void StackedContainers(Transform t, float x, float z, float yaw)
    {
        Material[] cols = { Rust, M(new Color(0.15f, 0.25f, 0.4f), 0.2f, 0.2f), OliveDark, M(new Color(0.5f, 0.4f, 0.1f), 0.2f, 0.2f) };
        Container(t, x, 0, z, yaw, cols[Random.Range(0, cols.Length)], 6f);
        if (Random.value < 0.6f) Container(t, x, 2.6f, z, yaw + Random.Range(-6f, 6f), cols[Random.Range(0, cols.Length)], 6f);
    }

    static void Warehouse(Transform t, float x, float z)
    {
        B(t, x, 2.2f, z, 7f, 4.4f, 12f, ConcreteDark);
        B(t, x, 4.5f, z, 7.4f, 0.2f, 12.4f, MetalDark);
        for (int i = 0; i < 3; i++) B(t, x - Mathf.Sign(x) * 3.51f, 2.8f, z - 3.5f + i * 3.5f, 0.04f, 0.7f, 1.2f, WindowGlow);
    }

    static void Barracks(Transform t, float x, float z)
    {
        B(t, x, 1.6f, z, 5f, 3.2f, 13f, Concrete);
        B(t, x, 3.3f, z, 5.4f, 0.25f, 13.4f, OliveDark);
        float face = -Mathf.Sign(x) * 2.52f;
        for (int i = 0; i < 4; i++) B(t, x + face, 1.9f, z - 4.5f + i * 3f, 0.04f, 0.8f, 1.0f, Random.value < 0.5f ? WindowGlow : Glass);
        B(t, x + face, 1.0f, z + 5f, 0.04f, 2.0f, 1.0f, Black);
    }

    static void FlagPole(Transform t, float x, float z)
    {
        Cy(t, x, 4f, z, 0.06f, 8f, Metal);
        B(t, x - Mathf.Sign(x) * 0.7f, 7.4f, z, 1.3f, 0.8f, 0.03f, OliveDark, new Vector3(0, 90, 0));
    }

    static void HescoRow(Transform t, float x, float z)
    {
        for (int i = 0; i < 4; i++) B(t, x, 0.9f, z - 3f + i * 1.9f, 1.2f, 1.8f, 1.8f, Khaki);
    }

    static void BarrelGroup(Transform t, float x, float z)
    {
        Barrel(t, x, 0, z, Rust); Barrel(t, x + 0.9f, 0, z + 0.3f, OliveDark); Barrel(t, x + 0.4f, 0, z - 0.9f, RedPaint);
    }

    static void Hangar(Transform t, float x, float z)
    {
        B(t, x, 3.5f, z, 10f, 7f, 16f, ConcreteDark);
        P(PrimitiveType.Cylinder, t, new Vector3(x, 7f, z), new Vector3(10f, 8f, 10f), MetalDark, new Vector3(90f, 0, 0));
        B(t, x - Mathf.Sign(x) * 5.02f, 3f, z, 0.05f, 5.5f, 9f, Black);
    }

    static void ControlTower(Transform t, float x, float z)
    {
        Cy(t, x, 6f, z, 1.2f, 12f, Concrete);
        B(t, x, 12.6f, z, 4f, 1.6f, 4f, Glass);
        B(t, x, 13.6f, z, 4.4f, 0.25f, 4.4f, MetalDark);
        Cy(t, x, 15f, z, 0.05f, 3f, Metal);
        B(t, x, 15.4f, z, 0.25f, 0.25f, 0.25f, RedGlow);
    }

    static void RunwayMarkings(Transform t)
    {
        for (float z = 3f; z < SegLen; z += 10f)
        {
            B(t, -10.5f, 0.012f, z, 1.0f, 0.02f, 6f, White);
            B(t, 10.5f, 0.012f, z, 1.0f, 0.02f, 6f, White);
        }
        B(t, -7.5f, 0.012f, SegLen / 2, 0.2f, 0.02f, SegLen, YellowPaint);
        B(t, 7.5f, 0.012f, SegLen / 2, 0.2f, 0.02f, SegLen, YellowPaint);
    }

    static void Floodlight(Transform t, float x, float z, bool withLight, int zone)
    {
        Cy(t, x, 4f, z, 0.12f, 8f, Metal);
        B(t, x, 8.1f, z, 1.4f, 0.3f, 0.6f, MetalDark);
        B(t, x, 7.92f, z, 1.2f, 0.05f, 0.45f, WindowGlow);
        if (!SaveManager.Day) GlowQuad(t, new Vector3(x, 7.7f, z - 0.5f), 4f, new Color(1f, 0.85f, 0.55f, 0.55f));
        if (withLight && !SaveManager.Day)
        {
            Color c = zone == 1 ? new Color(1f, 0.7f, 0.4f) : (zone == 2 ? new Color(0.7f, 1f, 0.8f) : new Color(1f, 0.85f, 0.6f));
            Lamp(t, new Vector3(x * 0.6f, 6.5f, z), c, 26f, 1.6f);
        }
    }

    static void Arch(Transform t, float z, int zone)
    {
        B(t, -4.9f, 3f, z, 0.6f, 6f, 0.6f, Concrete);
        B(t, 4.9f, 3f, z, 0.6f, 6f, 0.6f, Concrete);
        B(t, 0, 5.8f, z, 10.4f, 1.2f, 0.7f, ConcreteDark);
        B(t, 0, 5.8f, z - 0.37f, 8.6f, 0.8f, 0.04f, Black);
        Label(t, "WARNING - " + ZoneNames[zone], 0, 5.8f, z - 0.42f, 0, 0.09f, new Color(1f, 0.8f, 0.2f));
        B(t, -3.5f, 5.0f, z - 0.2f, 0.2f, 0.2f, 0.1f, RedGlow);
        B(t, 3.5f, 5.0f, z - 0.2f, 0.2f, 0.2f, 0.1f, RedGlow);
    }
}
