using UnityEngine;

public enum Sfx { Shoot, Empty, Coin, Health, Ammo, PowerUp, Damage, Zombie, ZombieDie, GameOver, Click, Step, Explosion, Shield }

/// <summary>Fully procedural audio: every sound effect and the military-survival music loop are synthesised at startup.</summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager I;
    const int SR = 22050;
    const float PI2 = Mathf.PI * 2f;
    AudioClip[] clips;
    AudioSource musicSrc;
    AudioSource[] srcs;
    int next;
    static readonly System.Random rng = new System.Random(7);
    static float N() { return (float)(rng.NextDouble() * 2.0 - 1.0); }
    static readonly float[] HealthNotes = { 523.25f, 659.25f, 783.99f, 1046.5f };
    static readonly float[] OverNotes = { 392f, 349.2f, 311.1f, 261.6f };
    static readonly float[] Bass = { 55f, 55f, 55f, 65.41f, 55f, 55f, 49f, 49f, 55f, 55f, 55f, 73.42f, 65.41f, 65.41f, 49f, 51.91f };

    public static AudioManager Ensure()
    {
        if (I == null) new GameObject("AudioManager").AddComponent<AudioManager>();
        return I;
    }

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        srcs = new AudioSource[10];
        for (int i = 0; i < srcs.Length; i++) { srcs[i] = gameObject.AddComponent<AudioSource>(); srcs[i].playOnAwake = false; }
        musicSrc = gameObject.AddComponent<AudioSource>();
        musicSrc.playOnAwake = false; musicSrc.loop = true;
        Build();
        ApplyVolumes();
    }

    static AudioClip Make(string n, float dur, System.Func<float, float> f)
    {
        int len = (int)(dur * SR);
        var d = new float[len];
        for (int i = 0; i < len; i++) d[i] = Mathf.Clamp(f(i / (float)SR), -1f, 1f);
        var c = AudioClip.Create(n, len, 1, SR, false);
        c.SetData(d, 0);
        return c;
    }

    void Build()
    {
        clips = new AudioClip[14];
        clips[(int)Sfx.Shoot] = Make("shoot", 0.25f, t => N() * Mathf.Exp(-t * 30f) * 0.8f + Mathf.Sin(PI2 * 110f * t) * Mathf.Exp(-t * 22f) * 0.6f);
        clips[(int)Sfx.Empty] = Make("empty", 0.08f, t => (Mathf.Sin(PI2 * 1400f * t) > 0 ? 1f : -1f) * Mathf.Exp(-t * 70f) * 0.3f);
        clips[(int)Sfx.Coin] = Make("coin", 0.3f, t => (Mathf.Sin(PI2 * 1175f * t) * Mathf.Exp(-t * 12f) + (t > 0.08f ? Mathf.Sin(PI2 * 1568f * (t - 0.08f)) * Mathf.Exp(-(t - 0.08f) * 10f) * 0.8f : 0f)) * 0.4f);
        clips[(int)Sfx.Health] = Make("health", 0.6f, t => { int k = Mathf.Min(3, (int)(t / 0.13f)); float lt = t - k * 0.13f; return Mathf.Sin(PI2 * HealthNotes[k] * lt) * Mathf.Exp(-lt * 6f) * 0.4f; });
        clips[(int)Sfx.Ammo] = Make("ammo", 0.25f, t => { float a = t < 0.08f ? N() * Mathf.Exp(-t * 60f) : (t > 0.1f ? N() * Mathf.Exp(-(t - 0.1f) * 60f) : 0f); return a * 0.4f + Mathf.Sin(PI2 * 2200f * t) * Mathf.Exp(-t * 40f) * 0.15f; });
        clips[(int)Sfx.PowerUp] = Make("powerup", 0.7f, t => (Mathf.Sin(PI2 * (300f * t + 450f * t * t)) * 0.45f + Mathf.Sin(PI2 * (600f * t + 900f * t * t)) * 0.2f) * (1f - t / 0.7f));
        clips[(int)Sfx.Damage] = Make("damage", 0.35f, t => Mathf.Sin(PI2 * 65f * t) * Mathf.Exp(-t * 10f) * 0.7f + N() * Mathf.Exp(-t * 25f) * 0.5f);
        clips[(int)Sfx.Zombie] = Make("zombie", 0.9f, t => { float f = 75f + 25f * Mathf.Sin(PI2 * 6f * t); float saw = 2f * Mathf.Repeat(f * t, 1f) - 1f; float env = Mathf.Sin(Mathf.PI * t / 0.9f); return saw * env * 0.3f + N() * 0.12f * env; });
        clips[(int)Sfx.ZombieDie] = Make("zdie", 0.7f, t => { float ph = 130f * t - 110f * t * t; float saw = 2f * Mathf.Repeat(ph, 1f) - 1f; return saw * Mathf.Exp(-t * 3f) * 0.35f + N() * Mathf.Exp(-t * 8f) * 0.15f; });
        clips[(int)Sfx.GameOver] = Make("gameover", 1.8f, t => { int k = Mathf.Min(3, (int)(t / 0.45f)); float lt = t - k * 0.45f; float f = OverNotes[k]; return (Mathf.Sin(PI2 * f * lt) + 0.5f * Mathf.Sin(PI2 * 2f * f * lt)) * Mathf.Exp(-lt * 2.5f) * 0.35f; });
        clips[(int)Sfx.Click] = Make("click", 0.07f, t => Mathf.Sin(PI2 * 880f * t) * Mathf.Exp(-t * 55f) * 0.35f);
        clips[(int)Sfx.Step] = Make("step", 0.12f, t => N() * Mathf.Exp(-t * 40f) * 0.3f + Mathf.Sin(PI2 * 70f * t) * Mathf.Exp(-t * 30f) * 0.3f);
        clips[(int)Sfx.Explosion] = Make("explosion", 1.2f, t => N() * Mathf.Exp(-t * 3.5f) * 0.7f + Mathf.Sin(PI2 * 45f * t) * Mathf.Exp(-t * 4f) * 0.7f);
        clips[(int)Sfx.Shield] = Make("shield", 0.4f, t => Mathf.Sin(PI2 * (500f + 600f * t) * t) * 0.3f * (1f - t / 0.4f));
        musicSrc.clip = Make("music", 9.6f, Music);
    }

    // 100 BPM, A-minor drone/bass + kick + hats + pad. Loop length 9.6 s (pad frequencies chosen to loop cleanly).
    static float Music(float t)
    {
        const float beat = 0.6f;
        int b = ((int)(t / beat)) % 16;
        float bt = t % beat;
        float bf = Bass[b];
        float bass = (Mathf.Sin(PI2 * bf * bt) + 0.5f * Mathf.Sin(PI2 * 2f * bf * bt)) * Mathf.Exp(-bt * 3.5f) * 0.32f;
        float kick = Mathf.Sin(PI2 * (60f * bt - 45f * bt * bt)) * Mathf.Exp(-bt * 12f) * 0.45f;
        float ht = t % (beat * 0.5f);
        float hat = N() * Mathf.Exp(-ht * 70f) * 0.07f;
        float lfo = 0.6f + 0.4f * Mathf.Sin(PI2 * t / 4.8f);
        float pad = (Mathf.Sin(PI2 * 110f * t) + 0.8f * Mathf.Sin(PI2 * 131.25f * t) + 0.6f * Mathf.Sin(PI2 * 165f * t)) * 0.05f * lfo;
        return bass + kick + hat + pad;
    }

    public void Play(Sfx s, float vol = 1f, float pitch = 1f)
    {
        if (clips == null) return;
        var a = srcs[next]; next = (next + 1) % srcs.Length;
        a.pitch = pitch;
        a.PlayOneShot(clips[(int)s], vol * SaveManager.SfxVolume);
    }
    public void PlayMusic() { if (!musicSrc.isPlaying) musicSrc.Play(); }
    public void ApplyVolumes() { if (musicSrc != null) musicSrc.volume = SaveManager.MusicVolume * 0.45f; }
}
