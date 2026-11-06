using System.Collections.Generic;
using UnityEngine;

/// <summary>Endless pooled road: spawns 40 m segments ahead, recycles behind; blends fog/ambient between the four zones.</summary>
public class EnvironmentGenerator : MonoBehaviour
{
    PlayerController player;
    ObjectPooler pool;
    int nextIndex = -1;
    readonly List<GameObject> live = new List<GameObject>();
    Camera cam;
    Color fog;
    public void Init(PlayerController p, Camera c) { player = p; pool = ObjectPooler.I; cam = c; fog = Atmosphere.ZoneFog(0); }

    void Update()
    {
        if (player == null) return;
        float pz = player.transform.position.z;
        int guard = 0;
        while (nextIndex * Gen.SegLen < pz + 180f && guard++ < 8)
        {
            int zone = Mathf.Max(0, nextIndex / 10) % 4;
            var go = pool.Get("seg" + zone + "_" + Random.Range(0, 3), new Vector3(0, 0, nextIndex * Gen.SegLen), Quaternion.identity);
            live.Add(go);
            nextIndex++;
        }
        while (live.Count > 0 && live[0].transform.position.z + Gen.SegLen < pz - 40f)
        {
            pool.Release(live[0]);
            live.RemoveAt(0);
        }
        int idx = Mathf.Max(0, Mathf.FloorToInt(pz / Gen.SegLen));
        fog = Color.Lerp(fog, Atmosphere.ZoneFog((idx / 10) % 4), 1f - Mathf.Exp(-0.8f * Time.deltaTime));
        RenderSettings.fogColor = fog;
        if (cam != null) cam.backgroundColor = fog;
    }
}
