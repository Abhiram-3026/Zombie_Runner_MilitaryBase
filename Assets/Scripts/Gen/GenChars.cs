using UnityEngine;

public static partial class Gen
{
    /// <summary>Builds the soldier (uniform, vest, backpack, helmet, rifle). Returned root is INACTIVE; caller activates it.</summary>
    public static GameObject MakeSoldier(Transform parent, out SoldierAnimator anim, out Transform muzzle)
    {
        Init();
        var root = NewRoot("Soldier");
        root.transform.SetParent(parent, false);
        anim = root.AddComponent<SoldierAnimator>();
        var mt = Pv(root.transform, "Model", 0, 0, 0);
        anim.model = mt;

        B(mt, 0, 1.22f, 0, 0.5f, 0.62f, 0.26f, Olive);                       // torso
        B(mt, 0, 1.25f, 0.07f, 0.54f, 0.46f, 0.16f, Vest);                    // tactical vest
        B(mt, -0.15f, 1.08f, 0.17f, 0.11f, 0.13f, 0.06f, OliveDark);          // pouches
        B(mt, 0.0f, 1.08f, 0.17f, 0.11f, 0.13f, 0.06f, OliveDark);
        B(mt, 0.15f, 1.08f, 0.17f, 0.11f, 0.13f, 0.06f, OliveDark);
        B(mt, 0, 1.30f, -0.26f, 0.42f, 0.52f, 0.22f, Khaki);                  // backpack
        Cy(mt, 0, 1.60f, -0.26f, 0.09f, 0.44f, OliveDark, new Vector3(0, 0, 90)); // bedroll
        B(mt, 0, 0.92f, 0, 0.52f, 0.08f, 0.28f, Black);                       // belt
        B(mt, 0, 1.58f, 0, 0.12f, 0.1f, 0.12f, Skin);                         // neck
        var head = Pv(mt, "Head", 0, 1.62f, 0);
        Sp(head, 0, 0.14f, 0, 0.24f, 0.26f, 0.25f, Skin);
        Sp(head, 0, 0.21f, -0.01f, 0.34f, 0.22f, 0.36f, OliveDark);            // helmet
        B(head, 0, 0.15f, 0.15f, 0.30f, 0.04f, 0.10f, OliveDark);              // helmet brim
        B(head, 0, 0.17f, 0.17f, 0.06f, 0.06f, 0.04f, MetalDark);              // NVG mount

        var legL = Pv(mt, "LegL", -0.13f, 0.92f, 0);
        var legR = Pv(mt, "LegR", 0.13f, 0.92f, 0);
        foreach (var leg in new[] { legL, legR })
        {
            B(leg, 0, -0.40f, 0, 0.18f, 0.8f, 0.2f, OliveDark);
            B(leg, 0, -0.42f, 0.11f, 0.14f, 0.13f, 0.04f, Black);              // knee pad
            B(leg, 0, -0.84f, 0.05f, 0.20f, 0.14f, 0.32f, Black);              // boot
        }
        var armL = Pv(mt, "ArmL", -0.34f, 1.46f, 0);
        var armR = Pv(mt, "ArmR", 0.34f, 1.46f, 0);
        foreach (var arm in new[] { armL, armR })
        {
            B(arm, 0, -0.25f, 0, 0.13f, 0.5f, 0.15f, Olive);
            B(arm, 0, -0.53f, 0, 0.12f, 0.1f, 0.13f, Black);                   // glove
        }
        var rifle = Pv(mt, "Rifle", 0.16f, 1.34f, 0.3f);
        B(rifle, 0, 0, 0.1f, 0.07f, 0.1f, 0.8f, Black);
        Cy(rifle, 0, 0.02f, 0.62f, 0.025f, 0.5f, MetalDark, new Vector3(90, 0, 0));
        B(rifle, 0, -0.15f, 0.08f, 0.06f, 0.2f, 0.1f, MetalDark);              // magazine
        B(rifle, 0, -0.02f, -0.4f, 0.06f, 0.12f, 0.3f, Black);                 // stock
        B(rifle, 0, 0.09f, 0.15f, 0.05f, 0.05f, 0.2f, MetalDark);              // scope
        muzzle = Pv(rifle, "Muzzle", 0, 0.02f, 0.9f);

        var pistol = Pv(mt, "Pistol", 0.34f, 1.36f, 0.42f);
        B(pistol, 0, 0, 0.06f, 0.06f, 0.07f, 0.3f, MetalDark);       // slide
        B(pistol, 0, -0.04f, 0.04f, 0.05f, 0.06f, 0.22f, Black);     // frame
        B(pistol, 0, -0.12f, -0.04f, 0.05f, 0.17f, 0.07f, Black);    // grip
        B(pistol, 0, -0.03f, 0.1f, 0.012f, 0.05f, 0.05f, MetalDark); // trigger guard
        var pMuzzle = Pv(pistol, "PistolMuzzle", 0, 0.01f, 0.24f);
        pistol.gameObject.SetActive(false);
        anim.pistol = pistol; anim.pistolMuzzle = pMuzzle; anim.rifleMuzzle = muzzle;
        anim.head = head; anim.legL = legL; anim.legR = legR; anim.armL = armL; anim.armR = armR; anim.rifle = rifle;
        return root;
    }

    /// <summary>Builds a zombie of the given type with ZombieAI, kinematic rigidbody and trigger collider. Returned root is INACTIVE.</summary>
    public static GameObject MakeZombie(ZombieType type)
    {
        Init();
        var root = NewRoot("Zombie_" + type);
        var model = Pv(root.transform, "Model", 0, 0, 0);
        bool fast = type == ZombieType.Fast, armored = type == ZombieType.Armored;
        Material skin = fast ? ZSkinFast : ZSkin;
        Material shirt = type == ZombieType.Normal ? Cloth : (fast ? ClothDark : OliveDark);
        var body = Pv(model, "Body", 0, 0.92f, 0);
        B(body, 0, 0.32f, 0, 0.5f, 0.62f, 0.26f, shirt);
        B(body, 0.1f, 0.4f, 0.14f, 0.2f, 0.28f, 0.03f, Blood);               // blood stain
        B(body, -0.12f, 0.15f, 0.14f, 0.18f, 0.1f, 0.03f, ClothDark);        // torn patch
        var head = Pv(body, "Head", 0, 0.72f, 0);
        Sp(head, 0, 0.12f, 0, 0.26f, 0.28f, 0.26f, skin);
        B(head, -0.06f, 0.15f, 0.12f, 0.05f, 0.05f, 0.03f, fast ? RedGlow : OrangeGlow);  // glowing eyes
        B(head, 0.06f, 0.15f, 0.12f, 0.05f, 0.05f, 0.03f, fast ? RedGlow : OrangeGlow);
        B(head, 0, 0.03f, 0.12f, 0.12f, 0.05f, 0.03f, Blood);               // mouth
        var armL = Pv(body, "ArmL", -0.33f, 0.58f, 0);
        var armR = Pv(body, "ArmR", 0.33f, 0.58f, 0);
        foreach (var a in new[] { armL, armR })
        {
            B(a, 0, -0.28f, 0, 0.12f, 0.58f, 0.12f, skin);
            B(a, 0, -0.1f, 0, 0.14f, 0.22f, 0.14f, shirt);
        }
        var legL = Pv(model, "LegL", -0.13f, 0.92f, 0);
        var legR = Pv(model, "LegR", 0.13f, 0.92f, 0);
        foreach (var l in new[] { legL, legR })
        {
            B(l, 0, -0.45f, 0, 0.17f, 0.9f, 0.18f, ClothDark);
            B(l, 0, -0.88f, 0.04f, 0.18f, 0.1f, 0.28f, Black);
        }
        if (armored)
        {
            B(body, 0, 0.38f, 0.15f, 0.6f, 0.52f, 0.1f, MetalDark);          // plate carrier
            Sp(head, 0, 0.17f, 0, 0.36f, 0.26f, 0.38f, Metal);               // helmet
            Sp(armL, 0, 0.02f, 0, 0.22f, 0.18f, 0.22f, Metal);               // pauldrons
            Sp(armR, 0, 0.02f, 0, 0.22f, 0.18f, 0.22f, Metal);
            model.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        else if (fast) model.localScale = new Vector3(0.92f, 1.0f, 0.92f);

        var rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true; rb.useGravity = false;
        var cap = root.AddComponent<CapsuleCollider>();
        cap.isTrigger = true; cap.direction = 1; cap.radius = armored ? 0.5f : 0.4f; cap.height = armored ? 2.2f : 1.8f;
        cap.center = new Vector3(0, cap.height / 2f, 0);
        var ai = root.AddComponent<ZombieAI>();
        ai.type = type; ai.model = model; ai.body = body; ai.head = head;
        ai.armL = armL; ai.armR = armR; ai.legL = legL; ai.legR = legR;
        return root;
    }

    /// <summary>Translucent blue energy bubble for the shield power-up.</summary>
    public static GameObject MakeShieldBubble(Transform parent)
    {
        var g = Pv(parent, "ShieldBubble", 0, 0.95f, 0).gameObject;
        P(PrimitiveType.Sphere, g.transform, Vector3.zero, new Vector3(2.5f, 2.7f, 2.5f), Flat(new Color(0.2f, 0.6f, 1f, 0.22f)), null);
        P(PrimitiveType.Sphere, g.transform, Vector3.zero, new Vector3(2.65f, 2.85f, 2.65f), Flat(new Color(0.5f, 0.9f, 1f, 0.1f)), null);
        return g;
    }
}
