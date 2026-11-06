using UnityEngine;
using UnityEngine.UI;

/// <summary>Gameplay HUD, pause menu and game-over screen (built entirely in code).</summary>
public class UIManager : MonoBehaviour
{
    public static UIManager I;
    Text scoreT, distT, coinsT, ammoT, speedT, hpT, notifyT, hintT, overTitle, overNew;
    Text[] overStats = new Text[6];
    RectTransform hpFill;
    Image hpFillImg, damageImg;
    GameObject pausePanel, overPanel;
    class Row { public GameObject go; public Text timer; public RectTransform bar; }
    readonly Row[] rows = new Row[4];
    float notifyTime, flashA, hintTime = 9f;
    Color notifyCol;
    int lastWeapon = -1, lastScore = -1, lastDist = -1, lastCoins = -1, lastAmmo = -1, lastSpeed = -1, lastHp = -1;
    public bool OverVisible { get { return overPanel != null && overPanel.activeSelf; } }

    static readonly Color[] PuCol = { new Color(0.85f, 0.3f, 0.95f), new Color(0.25f, 0.6f, 1f), new Color(0.25f, 0.95f, 0.35f), new Color(1f, 0.82f, 0.15f) };
    static readonly string[] PuName = { "COIN MAGNET", "SHIELD", "SPEED BOOST", "DOUBLE COINS" };
    static readonly string[] PuSym = { "M", "S", ">>", "2X" };

    void Awake() { I = this; }

    public void Build()
    {
        UIK.EnsureEventSystem();
        var cv = UIK.MakeCanvas("HUD", 10);
        var tl = new Vector2(0, 1); var tr = new Vector2(1, 1); var tc = new Vector2(0.5f, 1); var bl = new Vector2(0, 0); var ctr = new Vector2(0.5f, 0.5f);
        var white = new Color(0.95f, 0.95f, 0.9f);

        damageImg = UIK.Img(cv.transform, "Damage", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Color(0.8f, 0f, 0f, 0f));
        var dr = damageImg.rectTransform; dr.anchorMin = Vector2.zero; dr.anchorMax = Vector2.one; dr.offsetMin = Vector2.zero; dr.offsetMax = Vector2.zero;
        damageImg.raycastTarget = false;

        // health bar
        UIK.Img(cv.transform, "HpBack", tl, tl, new Vector2(30, -30), new Vector2(520, 38), new Color(0, 0, 0, 0.65f));
        var fillBg = UIK.Rect(cv.transform, "HpArea", tl, tl, new Vector2(34, -34), new Vector2(512, 30));
        var fill = UIK.NewRT(fillBg, "HpFill");
        fill.anchorMin = Vector2.zero; fill.anchorMax = Vector2.one; fill.offsetMin = Vector2.zero; fill.offsetMax = Vector2.zero;
        hpFillImg = fill.gameObject.AddComponent<Image>(); hpFill = fill;
        hpT = UIK.Txt(cv.transform, "HEALTH 100%", 26, TextAnchor.MiddleCenter, white, tl, tl, new Vector2(30, -30), new Vector2(520, 38));
        ammoT = UIK.Txt(cv.transform, "AMMO 30", 36, TextAnchor.MiddleLeft, new Color(1f, 0.85f, 0.3f), tl, tl, new Vector2(34, -76), new Vector2(400, 44));
        speedT = UIK.Txt(cv.transform, "SPEED 0 km/h", 30, TextAnchor.MiddleLeft, white, bl, bl, new Vector2(34, 30), new Vector2(400, 40));

        scoreT = UIK.Txt(cv.transform, "SCORE 0", 54, TextAnchor.MiddleCenter, white, tc, tc, new Vector2(0, -24), new Vector2(700, 64));
        distT = UIK.Txt(cv.transform, "DISTANCE 0 m", 28, TextAnchor.MiddleCenter, new Color(0.8f, 0.85f, 0.7f), tc, tc, new Vector2(0, -88), new Vector2(500, 36));
        coinsT = UIK.Txt(cv.transform, "COINS 0", 40, TextAnchor.MiddleRight, new Color(1f, 0.85f, 0.2f), tr, tr, new Vector2(-250, -30), new Vector2(400, 50));
        UIK.Btn(cv.transform, "II PAUSE", tr, new Vector2(-120, -52), new Vector2(200, 60), () => GameManager.I.TogglePause(), 26);

        for (int i = 0; i < 4; i++)
        {
            var r = new Row();
            var rt = UIK.Rect(cv.transform, "PU" + i, tl, tl, new Vector2(30, -140 - i * 72), new Vector2(360, 64));
            var bg = rt.gameObject.AddComponent<Image>(); bg.color = new Color(0, 0, 0, 0.6f);
            var icon = UIK.Img(rt, "Icon", tl, tl, new Vector2(6, -6), new Vector2(52, 52), PuCol[i]);
            UIK.Txt(icon.transform, PuSym[i], 26, TextAnchor.MiddleCenter, Color.black, ctr, ctr, Vector2.zero, new Vector2(52, 52));
            UIK.Txt(rt, PuName[i], 22, TextAnchor.MiddleLeft, white, tl, tl, new Vector2(70, -6), new Vector2(200, 26));
            r.timer = UIK.Txt(rt, "0.0s", 24, TextAnchor.MiddleRight, white, tl, tl, new Vector2(240, -6), new Vector2(110, 26));
            UIK.Img(rt, "BarBack", tl, tl, new Vector2(70, -42), new Vector2(280, 10), new Color(0.15f, 0.15f, 0.15f, 1f));
            var holder = UIK.Rect(rt, "BarHolder", tl, tl, new Vector2(70, -42), new Vector2(280, 10));
            var bar = UIK.NewRT(holder, "Bar");
            bar.anchorMin = Vector2.zero; bar.anchorMax = Vector2.one; bar.offsetMin = Vector2.zero; bar.offsetMax = Vector2.zero;
            bar.gameObject.AddComponent<Image>().color = PuCol[i];
            r.bar = bar; r.go = rt.gameObject; r.go.SetActive(false);
            rows[i] = r;
        }
        notifyT = UIK.Txt(cv.transform, "", 46, TextAnchor.MiddleCenter, white, tc, tc, new Vector2(0, -170), new Vector2(900, 60));
        hintT = UIK.Txt(cv.transform, "A/D or Arrows: Lane   W/Up: Jump   S/Down: Slide   SPACE/Mouse: Shoot   1/2/Q: Weapon   ESC: Pause", 26, TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, 0.85f), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 24), new Vector2(1400, 40));

        // pause panel
        pausePanel = UIK.Img(cv.transform, "Pause", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Color(0, 0, 0, 0.7f)).gameObject;
        Stretch(pausePanel);
        UIK.Txt(pausePanel.transform, "PAUSED", 100, TextAnchor.MiddleCenter, white, ctr, ctr, new Vector2(0, 230), new Vector2(900, 120));
        UIK.Btn(pausePanel.transform, "RESUME", ctr, new Vector2(0, 80), new Vector2(460, 80), () => GameManager.I.TogglePause());
        UIK.Btn(pausePanel.transform, "RESTART", ctr, new Vector2(0, -30), new Vector2(460, 80), () => GameManager.I.Restart());
        UIK.Btn(pausePanel.transform, "MAIN MENU", ctr, new Vector2(0, -140), new Vector2(460, 80), () => GameManager.I.ToMenu());
        pausePanel.SetActive(false);

        // game over panel
        overPanel = UIK.Img(cv.transform, "GameOver", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Color(0.1f, 0f, 0f, 0.78f)).gameObject;
        Stretch(overPanel);
        overTitle = UIK.Txt(overPanel.transform, "GAME OVER", 120, TextAnchor.MiddleCenter, new Color(0.9f, 0.15f, 0.1f), ctr, ctr, new Vector2(0, 360), new Vector2(1200, 140));
        overNew = UIK.Txt(overPanel.transform, "NEW BEST SCORE!", 44, TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.2f), ctr, ctr, new Vector2(0, 275), new Vector2(900, 56));
        for (int i = 0; i < 6; i++)
            overStats[i] = UIK.Txt(overPanel.transform, "", 40, TextAnchor.MiddleCenter, white, ctr, ctr, new Vector2(0, 200 - i * 56), new Vector2(900, 50));
        UIK.Btn(overPanel.transform, "RETRY (R)", ctr, new Vector2(-250, -210), new Vector2(440, 84), () => GameManager.I.Restart());
        UIK.Btn(overPanel.transform, "MAIN MENU", ctr, new Vector2(250, -210), new Vector2(440, 84), () => GameManager.I.ToMenu());
        overPanel.SetActive(false);
    }

    static void Stretch(GameObject g)
    {
        var r = g.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
    }

    public void Notify(string s, Color c) { if (notifyT == null) return; notifyT.text = s; notifyCol = c; notifyTime = 2f; }
    public void Flash() { flashA = 0.55f; }
    public void ShowPause(bool on) { pausePanel.SetActive(on); }
    public void ShowGameOverDelayed(float s) { Invoke("ShowGameOver", s); }

    void ShowGameOver()
    {
        var gm = GameManager.I;
        overPanel.SetActive(true);
        overNew.gameObject.SetActive(gm.newBest);
        overStats[0].text = "FINAL SCORE: " + gm.Score;
        overStats[1].text = "BEST SCORE: " + SaveManager.BestScore;
        overStats[2].text = "COINS COLLECTED: " + gm.RunCoins;
        overStats[3].text = "TOTAL COINS: " + SaveManager.TotalCoins;
        overStats[4].text = "DISTANCE SURVIVED: " + Mathf.FloorToInt(gm.distance) + " m";
        overStats[5].text = "ZOMBIES ELIMINATED: " + gm.Kills;
    }

    void Update()
    {
        var gm = GameManager.I;
        if (gm == null || gm.health == null || scoreT == null) return;
        if (gm.Score != lastScore) { lastScore = gm.Score; scoreT.text = "SCORE " + lastScore; }
        int d = Mathf.FloorToInt(gm.distance);
        if (d != lastDist) { lastDist = d; distT.text = "DISTANCE " + d + " m"; }
        if (gm.RunCoins != lastCoins) { lastCoins = gm.RunCoins; coinsT.text = "COINS " + lastCoins; }
        if (gm.shooting.Ammo != lastAmmo || gm.shooting.Weapon != lastWeapon) { lastAmmo = gm.shooting.Ammo; lastWeapon = gm.shooting.Weapon; ammoT.text = "AMMO " + lastAmmo + "   [" + gm.shooting.WeaponName + "]"; ammoT.color = lastAmmo == 0 ? new Color(1f, 0.3f, 0.2f) : new Color(1f, 0.85f, 0.3f); }
        int sp = Mathf.RoundToInt(gm.CurrentSpeed * 3.6f);
        if (sp != lastSpeed) { lastSpeed = sp; speedT.text = "SPEED " + sp + " km/h"; }
        float pct = gm.health.hp / gm.health.max;
        int hpi = Mathf.CeilToInt(pct * 100f);
        if (hpi != lastHp)
        {
            lastHp = hpi; hpT.text = "HEALTH " + hpi + "%";
            hpFill.anchorMax = new Vector2(Mathf.Clamp01(pct), 1f);
            hpFillImg.color = Color.Lerp(new Color(0.9f, 0.1f, 0.1f), new Color(0.3f, 0.85f, 0.25f), pct);
        }
        float udt = Time.unscaledDeltaTime;
        var pm = PowerUpManager.I;
        for (int i = 0; i < 4; i++)
        {
            var t = (PowerUpType)i;
            bool on = pm.Active(t);
            if (rows[i].go.activeSelf != on) rows[i].go.SetActive(on);
            if (on)
            {
                rows[i].timer.text = pm.Remaining(t).ToString("0.0") + "s";
                rows[i].bar.anchorMax = new Vector2(Mathf.Clamp01(pm.Remaining(t) / Mathf.Max(0.01f, pm.Duration(t))), 1f);
            }
        }
        if (notifyTime > 0f)
        {
            notifyTime -= udt;
            var c = notifyCol; c.a = Mathf.Clamp01(notifyTime / 0.5f); notifyT.color = c;
        }
        if (hintTime > 0f) { hintTime -= udt; if (hintTime <= 0f) hintT.gameObject.SetActive(false); }
        flashA = Mathf.MoveTowards(flashA, 0f, udt * 1.6f);
        float low = (pct < 0.3f && !gm.health.IsDead) ? 0.1f + 0.08f * Mathf.Sin(Time.unscaledTime * 6f) : 0f;
        damageImg.color = new Color(0.8f, 0f, 0f, Mathf.Max(flashA, low));
    }
}
