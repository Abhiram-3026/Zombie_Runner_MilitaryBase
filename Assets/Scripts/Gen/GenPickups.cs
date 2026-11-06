using UnityEngine;

public static partial class Gen
{
    /// <summary>Builds a collectible. The root sits at the pickup's world position; 'Spin' child is rotated/bobbed by Collectible.Tick.</summary>
    public static GameObject MakePickup(string key)
    {
        Init();
        var root = NewRoot(key);
        var spin = Pv(root.transform, "Spin", 0, 0, 0);
        var col = root.AddComponent<SphereCollider>();
        col.isTrigger = true; col.radius = key == "p_coin" ? 0.75f : 1.0f;
        var c = root.AddComponent<Collectible>();
        c.spin = spin;
        switch (key)
        {
            case "p_coin":
                {
                    c.type = CollectibleType.Coin;
                    var m = Pv(spin, "Mesh", 0, 0, 0); m.localEulerAngles = new Vector3(90, 0, 0);
                    Cy(m, 0, 0, 0, 0.34f, 0.07f, Gold); Cy(m, 0, 0, 0, 0.37f, 0.045f, Brass); Cy(m, 0, 0, 0, 0.22f, 0.09f, Gold);
                    GlowQuad(root.transform, Vector3.zero, 1.5f, new Color(1f, 0.8f, 0.2f, 0.5f));
                    break;
                }
            case "p_health":
                c.type = CollectibleType.Health;
                B(spin, 0, 0, 0, 0.75f, 0.55f, 0.32f, RedPaint);
                B(spin, 0, 0, -0.17f, 0.42f, 0.12f, 0.02f, White); B(spin, 0, 0, -0.17f, 0.12f, 0.42f, 0.02f, White);
                B(spin, 0, 0, 0.17f, 0.42f, 0.12f, 0.02f, White); B(spin, 0, 0, 0.17f, 0.12f, 0.42f, 0.02f, White);
                B(spin, 0, 0.34f, 0, 0.3f, 0.08f, 0.1f, White);
                GlowQuad(root.transform, Vector3.zero, 2.2f, new Color(1f, 0.15f, 0.15f, 0.5f));
                break;
            case "p_ammo":
                c.type = CollectibleType.Ammo;
                B(spin, 0, -0.35f, 0, 0.8f, 0.12f, 0.3f, Olive);
                B(spin, 0, 0, 0, 0.6f, 0.62f, 0.2f, MetalDark);
                for (int i = 0; i < 3; i++)
                {
                    Cy(spin, -0.22f + i * 0.22f, -0.05f, -0.12f, 0.07f, 0.5f, Brass);
                    Sp(spin, -0.22f + i * 0.22f, 0.22f, -0.12f, 0.14f, 0.16f, 0.14f, Gold);
                }
                GlowQuad(root.transform, Vector3.zero, 2.2f, new Color(1f, 0.8f, 0.2f, 0.5f));
                break;
            case "p_magnet":
                c.type = CollectibleType.Magnet;
                for (int i = 0; i <= 6; i++)
                {
                    float a = Mathf.PI * i / 6f;
                    B(spin, Mathf.Cos(a) * 0.35f, 0.1f + Mathf.Sin(a) * 0.35f, 0, 0.17f, 0.17f, 0.17f, RedPaint, new Vector3(0, 0, a * Mathf.Rad2Deg + 90f));
                }
                B(spin, -0.35f, -0.12f, 0, 0.17f, 0.4f, 0.17f, RedPaint); B(spin, 0.35f, -0.12f, 0, 0.17f, 0.4f, 0.17f, RedPaint);
                B(spin, -0.35f, -0.42f, 0, 0.18f, 0.16f, 0.18f, White); B(spin, 0.35f, -0.42f, 0, 0.18f, 0.16f, 0.18f, White);
                GlowQuad(root.transform, Vector3.zero, 2.4f, new Color(0.9f, 0.3f, 1f, 0.55f));
                break;
            case "p_shield":
                c.type = CollectibleType.Shield;
                P(PrimitiveType.Sphere, spin, Vector3.zero, new Vector3(0.95f, 1.0f, 0.95f), Flat(new Color(0.2f, 0.6f, 1f, 0.4f)), null);
                B(spin, 0, 0, 0, 0.42f, 0.56f, 0.08f, CyanGlow);
                Cy(spin, 0, 0, 0, 0.42f, 0.04f, CyanGlow, new Vector3(90, 0, 0));
                GlowQuad(root.transform, Vector3.zero, 2.4f, new Color(0.2f, 0.6f, 1f, 0.55f));
                break;
            case "p_speed":
                c.type = CollectibleType.Speed;
                Cy(spin, 0, 0, 0, 0.45f, 0.08f, MetalDark, new Vector3(90, 0, 0));
                for (int i = 0; i < 2; i++)
                {
                    float x = -0.12f + i * 0.26f;
                    B(spin, x, 0.12f, -0.06f, 0.36f, 0.1f, 0.06f, GreenGlow, new Vector3(0, 0, -45));
                    B(spin, x, -0.12f, -0.06f, 0.36f, 0.1f, 0.06f, GreenGlow, new Vector3(0, 0, 45));
                    B(spin, x, 0.12f, 0.06f, 0.36f, 0.1f, 0.06f, GreenGlow, new Vector3(0, 0, -45));
                    B(spin, x, -0.12f, 0.06f, 0.36f, 0.1f, 0.06f, GreenGlow, new Vector3(0, 0, 45));
                }
                GlowQuad(root.transform, Vector3.zero, 2.4f, new Color(0.2f, 1f, 0.3f, 0.55f));
                break;
            case "p_double":
                {
                    c.type = CollectibleType.DoubleCoins;
                    var m = Pv(spin, "Mesh", 0, 0, 0); m.localEulerAngles = new Vector3(90, 0, 0);
                    Cy(m, 0, 0, 0, 0.45f, 0.09f, Gold); Cy(m, 0, 0, 0, 0.49f, 0.06f, Brass);
                    Label(spin, "2X", 0, 0, -0.07f, 0, 0.06f, new Color(0.25f, 0.12f, 0f));
                    Label(spin, "2X", 0, 0, 0.07f, 180f, 0.06f, new Color(0.25f, 0.12f, 0f));
                    GlowQuad(root.transform, Vector3.zero, 2.6f, new Color(1f, 0.85f, 0.2f, 0.6f));
                    break;
                }
        }
        return root;
    }
}
