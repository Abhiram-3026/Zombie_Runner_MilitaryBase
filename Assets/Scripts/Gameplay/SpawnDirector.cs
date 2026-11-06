using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns obstacles, zombies, coins and pickups ahead of the player.
/// Fairness rule: every pattern keeps at least one "safe lane" free of obstacles/zombies, or is a full-width wall that can be
/// jumped or slid under, so no layout is ever impossible. Consecutive safe lanes differ by at most one lane.
/// </summary>
public class SpawnDirector : MonoBehaviour
{
    PlayerController player;
    GameConfig cfg;
    ObjectPooler pool;
    float nextZ, prevEnd;
    int prevSafe = 1, patterns;
    struct Live { public PooledObject po; public int gen; }
    readonly List<Live> live = new List<Live>();
    float sweepT;

    public void Init(PlayerController p) { player = p; cfg = GameConfig.I; pool = ObjectPooler.I; nextZ = cfg.firstSpawnZ; prevEnd = cfg.firstSpawnZ - 25f; }
    static float LaneX(int l) { return (l - 1) * GameConfig.I.laneWidth; }

    void Update()
    {
        var gm = GameManager.I;
        if (gm == null || gm.State != GameState.Playing || player == null) return;
        float limit = player.transform.position.z + cfg.spawnAhead;
        int guard = 0;
        while (nextZ < limit && guard++ < 3) SpawnPattern(gm);
        sweepT -= Time.deltaTime;
        if (sweepT <= 0f) { sweepT = 0.4f; Sweep(); }
    }

    void Sweep()
    {
        float lim = player.transform.position.z - 25f;
        for (int i = live.Count - 1; i >= 0; i--)
        {
            var e = live[i];
            if (e.po == null || e.po.gen != e.gen || !e.po.gameObject.activeSelf) { live.RemoveAt(i); continue; }
            if (e.po.transform.position.z < lim) { pool.Release(e.po.gameObject); live.RemoveAt(i); }
        }
    }

    GameObject Spawn(string key, Vector3 pos, float yaw = 0f)
    {
        var go = pool.Get(key, pos, Quaternion.Euler(0, yaw, 0));
        var po = go.GetComponent<PooledObject>();
        live.Add(new Live { po = po, gen = po.gen });
        return go;
    }

    void SpawnPattern(GameManager gm)
    {
        float d = gm.Difficulty01;
        int safe = Mathf.Clamp(prevSafe + Random.Range(-1, 2), 0, 2);
        float z = nextZ;
        GapRewards(gm, prevSafe, safe, prevEnd + 5f, z - 5f);
        float len;
        float r = Random.value;
        bool easy = patterns < 2;
        if (!easy && r < 0.13f) len = FullWall(z, true);
        else if (!easy && r < 0.24f) len = FullWall(z, false);
        else if (!easy && r < 0.36f + 0.1f * d) len = Horde(z, safe, d);
        else len = LanePattern(z, safe, d, easy);
        patterns++;
        prevSafe = safe;
        prevEnd = z + len;
        float sec = Mathf.Lerp(cfg.maxGapSeconds, cfg.minGapSeconds, d) / gm.SpawnScale;
        nextZ = z + len + Mathf.Max(16f, gm.CurrentSpeed * sec);
    }

    float FullWall(float z, bool jump)
    {
        string[] arr = jump ? Gen.JumpKeys : Gen.OverheadKeys;
        string k = arr[Random.Range(0, arr.Length)];
        for (int l = 0; l < 3; l++)
        {
            Spawn(k, new Vector3(LaneX(l), 0, z));
            if (jump && Random.value < 0.6f) CoinArc(LaneX(l), z);
            else if (!jump && Random.value < 0.5f) { Spawn("p_coin", new Vector3(LaneX(l), 0.6f, z - 3f)); Spawn("p_coin", new Vector3(LaneX(l), 0.6f, z - 1.5f)); }
        }
        return Gen.Depth(k);
    }

    float Horde(float z, int safe, float d)
    {
        int n = 2 + Mathf.RoundToInt(d * 3f) + Random.Range(0, 2);
        for (int l = 0; l < 3; l++)
        {
            if (l == safe) continue;
            for (int k = 0; k < n; k++) SpawnZombie(l, z + k * 2.8f + Random.Range(0f, 1f), d);
        }
        return n * 2.8f;
    }

    float LanePattern(float z, int safe, float d, bool easy)
    {
        float len = 2f;
        for (int l = 0; l < 3; l++)
        {
            if (l == safe) continue;
            if (easy && Random.value < 0.5f) continue;
            len = Mathf.Max(len, LaneItem(l, z, d));
        }
        return len;
    }

    float LaneItem(int l, float z, float d)
    {
        float r = Random.value;
        float c1 = 0.2f - 0.12f * d, c2 = c1 + 0.22f + 0.15f * d, c3 = c2 + 0.14f, c4 = c3 + 0.17f, c5 = c4 + 0.14f;
        float x = LaneX(l);
        if (r < c1) return 0f;
        if (r < c2) { SpawnZombie(l, z + Random.Range(0f, 4f), d); return 3f; }
        string k;
        if (r < c3) k = Gen.BlockKeys[Random.Range(0, Gen.BlockKeys.Length)];
        else if (r < c4) k = Gen.JumpKeys[Random.Range(0, Gen.JumpKeys.Length)];
        else if (r < c5) k = Gen.OverheadKeys[Random.Range(0, Gen.OverheadKeys.Length)];
        else k = Gen.HazardKeys[Random.Range(0, Gen.HazardKeys.Length)];
        Spawn(k, new Vector3(x, 0, z));
        // skill coins: arc above jump obstacles, or a risky pair just before other obstacles
        if (r >= c3 && r < c4 && Random.value < 0.5f) CoinArc(x, z);
        else if (Random.value < 0.3f) { Spawn("p_coin", new Vector3(x, 1f, z - 4f)); Spawn("p_coin", new Vector3(x, 1f, z - 2.5f)); }
        return Gen.Depth(k);
    }

    void CoinArc(float x, float z)
    {
        Spawn("p_coin", new Vector3(x, 1.7f, z - 1.4f));
        Spawn("p_coin", new Vector3(x, 2.1f, z));
        Spawn("p_coin", new Vector3(x, 1.7f, z + 1.4f));
    }

    void SpawnZombie(int lane, float z, float d)
    {
        float fast = 0.15f + 0.9f * d, arm = 0.08f + 0.7f * d, r = Random.value * (1f + fast + arm);
        string k = r < 1f ? "z_normal" : (r < 1f + fast ? "z_fast" : "z_armored");
        Spawn(k, new Vector3(LaneX(lane), 0, z), 180f);
    }

    // coin curves / lines between patterns, with optional pickup replacing the middle coin
    void GapRewards(GameManager gm, int from, int to, float z0, float z1)
    {
        if (z1 - z0 < 8f) return;
        int n = Mathf.Clamp((int)((z1 - z0) / 2.2f), 3, 12);
        bool arc = Random.value < 0.25f;
        string pick = RollPickup(gm);
        for (int i = 0; i < n; i++)
        {
            float t = n == 1 ? 0f : i / (float)(n - 1);
            float x = Mathf.Lerp(LaneX(from), LaneX(to), Mathf.SmoothStep(0f, 1f, t));
            float z = Mathf.Lerp(z0, z1, t);
            float y = arc ? 1f + Mathf.Sin(Mathf.PI * t) * 1.3f : 1f;
            if (pick != null && i == n / 2) Spawn(pick, new Vector3(x, 1.1f, z));
            else Spawn("p_coin", new Vector3(x, y, z));
        }
    }

    string RollPickup(GameManager gm)
    {
        float hpP = 1f - gm.health.hp / gm.health.max;
        float r = Random.value, t = 0f;
        t += 0.05f + hpP * 0.25f; if (r < t) return "p_health";
        t += 0.10f + (gm.shooting.Ammo < 10 ? 0.25f : 0f); if (r < t) return "p_ammo";
        t += 0.035f; if (r < t) return "p_magnet";
        t += 0.035f; if (r < t) return "p_shield";
        t += 0.03f; if (r < t) return "p_speed";
        t += 0.035f; if (r < t) return "p_double";
        return null;
    }
}
