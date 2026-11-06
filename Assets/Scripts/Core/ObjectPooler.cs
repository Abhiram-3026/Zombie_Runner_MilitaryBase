using System.Collections.Generic;
using UnityEngine;

public interface IShootable
{
    bool CanBeShot { get; }
    void OnShot(float damage, Vector3 point);
}

public enum GameState { Playing, Paused, GameOver }

public class PooledObject : MonoBehaviour { public string key; public int gen; }

/// <summary>Generic keyed object pool. Objects are created inactive by factories registered via Register().</summary>
public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler I;
    readonly Dictionary<string, System.Func<GameObject>> factories = new Dictionary<string, System.Func<GameObject>>();
    readonly Dictionary<string, Stack<GameObject>> pools = new Dictionary<string, Stack<GameObject>>();
    Transform root;

    void Awake() { I = this; root = new GameObject("PoolRoot").transform; root.SetParent(transform, false); }

    public void Register(string key, System.Func<GameObject> factory, int prewarm = 0)
    {
        factories[key] = factory;
        pools[key] = new Stack<GameObject>();
        for (int i = 0; i < prewarm; i++) pools[key].Push(Create(key));
    }

    GameObject Create(string key)
    {
        var go = factories[key]();
        go.SetActive(false);
        go.transform.SetParent(root, false);
        var po = go.AddComponent<PooledObject>();
        po.key = key;
        return go;
    }

    public GameObject Get(string key, Vector3 pos, Quaternion rot)
    {
        var st = pools[key];
        GameObject go = null;
        while (st.Count > 0 && go == null) go = st.Pop();
        if (go == null) go = Create(key);
        go.transform.SetPositionAndRotation(pos, rot);
        go.GetComponent<PooledObject>().gen++;
        go.SetActive(true);
        return go;
    }

    public void Release(GameObject go)
    {
        if (go == null || !go.activeSelf) return;
        var po = go.GetComponent<PooledObject>();
        go.SetActive(false);
        if (po != null && pools.ContainsKey(po.key)) pools[po.key].Push(go); else Destroy(go);
    }
}

public static class Fx
{
    public static void Spawn(string key, Vector3 pos)
    {
        if (ObjectPooler.I != null) ObjectPooler.I.Get(key, pos, Quaternion.identity);
    }
}

/// <summary>Plays child particle systems on enable and returns the object to the pool after 'life' seconds.</summary>
public class AutoRelease : MonoBehaviour
{
    public float life = 1f;
    ParticleSystem[] systems;
    void OnEnable()
    {
        if (systems == null) systems = GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < systems.Length; i++) { systems[i].Clear(true); systems[i].Play(true); }
        CancelInvoke();
        Invoke("Rel", life);
    }
    void Rel() { if (ObjectPooler.I != null) ObjectPooler.I.Release(gameObject); else Destroy(gameObject); }
}
