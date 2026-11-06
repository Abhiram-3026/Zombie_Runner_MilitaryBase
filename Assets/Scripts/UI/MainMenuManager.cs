using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    GameObject mainPanel, settingsPanel, controlsPanel;
    Text bestT, qualityT, diffT, dayT;

    public void Build()
    {
        UIK.EnsureEventSystem();
        var cv = UIK.MakeCanvas("Menu", 10);
        var ctr = new Vector2(0.5f, 0.5f); var tc = new Vector2(0.5f, 1f);
        var white = new Color(0.95f, 0.95f, 0.9f);

        mainPanel = UIK.Full(cv.transform, "Main").gameObject;
        UIK.Txt(mainPanel.transform, "3D ZOMBIE RUNNER", 130, TextAnchor.MiddleCenter, new Color(0.9f, 0.9f, 0.8f), tc, tc, new Vector2(0, -150), new Vector2(1600, 160));
        UIK.Txt(mainPanel.transform, "MILITARY BASE EDITION", 46, TextAnchor.MiddleCenter, new Color(0.9f, 0.2f, 0.15f), tc, tc, new Vector2(0, -250), new Vector2(1000, 60));
        UIK.Btn(mainPanel.transform, "START GAME", ctr, new Vector2(0, 40), new Vector2(480, 84), () => SceneManager.LoadScene("Gameplay"));
        UIK.Btn(mainPanel.transform, "SETTINGS", ctr, new Vector2(0, -60), new Vector2(480, 84), () => Show(settingsPanel));
        UIK.Btn(mainPanel.transform, "CONTROLS", ctr, new Vector2(0, -160), new Vector2(480, 84), () => Show(controlsPanel));
        UIK.Btn(mainPanel.transform, "EXIT", ctr, new Vector2(0, -260), new Vector2(480, 84), Quit);
        bestT = UIK.Txt(mainPanel.transform, "", 34, TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.3f), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 40), new Vector2(1200, 50));

        // settings
        settingsPanel = UIK.Img(cv.transform, "Settings", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Color(0, 0, 0, 0.85f)).gameObject;
        var sr = settingsPanel.GetComponent<RectTransform>(); sr.anchorMin = Vector2.zero; sr.anchorMax = Vector2.one; sr.offsetMin = Vector2.zero; sr.offsetMax = Vector2.zero;
        UIK.Txt(settingsPanel.transform, "SETTINGS", 80, TextAnchor.MiddleCenter, white, ctr, ctr, new Vector2(0, 360), new Vector2(900, 100));
        Row(settingsPanel.transform, "MUSIC VOLUME", 220, SaveManager.MusicVolume, v => { SaveManager.MusicVolume = v; AudioManager.I.ApplyVolumes(); });
        Row(settingsPanel.transform, "SOUND EFFECTS", 120, SaveManager.SfxVolume, v => { SaveManager.SfxVolume = v; });
        Row(settingsPanel.transform, "MOUSE SENSITIVITY", 20, SaveManager.MouseSens, v => { SaveManager.MouseSens = v; });
        var qb = UIK.Btn(settingsPanel.transform, "", ctr, new Vector2(0, -90), new Vector2(760, 74), CycleQuality, 30);
        qualityT = qb.GetComponentInChildren<Text>();
        var db = UIK.Btn(settingsPanel.transform, "", ctr, new Vector2(0, -185), new Vector2(760, 74), CycleDifficulty, 30);
        diffT = db.GetComponentInChildren<Text>();
        var tb = UIK.Btn(settingsPanel.transform, "", ctr, new Vector2(0, -280), new Vector2(760, 74), CycleDay, 30);
        dayT = tb.GetComponentInChildren<Text>();
        UIK.Btn(settingsPanel.transform, "BACK", ctr, new Vector2(0, -390), new Vector2(360, 76), () => { SaveManager.Save(); Show(mainPanel); });
        RefreshLabels();
        settingsPanel.SetActive(false);

        // controls
        controlsPanel = UIK.Img(cv.transform, "Controls", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, new Color(0, 0, 0, 0.85f)).gameObject;
        var cr = controlsPanel.GetComponent<RectTransform>(); cr.anchorMin = Vector2.zero; cr.anchorMax = Vector2.one; cr.offsetMin = Vector2.zero; cr.offsetMax = Vector2.zero;
        UIK.Txt(controlsPanel.transform, "CONTROLS", 80, TextAnchor.MiddleCenter, white, ctr, ctr, new Vector2(0, 360), new Vector2(900, 100));
        string[] lines = { "A / LEFT ARROW  -  Move left", "D / RIGHT ARROW  -  Move right", "W / UP ARROW  -  Jump", "S / DOWN ARROW  -  Slide (in air: fast drop)",
                           "SPACE / LEFT MOUSE  -  Shoot (hold to auto-fire)", "1 / 2 / Q  -  Rifle / Handgun / Switch weapon", "ESC  -  Pause / Resume", "R  -  Restart after game over" };
        for (int i = 0; i < lines.Length; i++)
            UIK.Txt(controlsPanel.transform, lines[i], 38, TextAnchor.MiddleCenter, white, ctr, ctr, new Vector2(0, 240 - i * 62), new Vector2(1400, 50));
        UIK.Btn(controlsPanel.transform, "BACK", ctr, new Vector2(0, -280), new Vector2(360, 76), () => Show(mainPanel));
        controlsPanel.SetActive(false);
        Show(mainPanel);
    }

    void Row(Transform p, string label, float y, float val, UnityEngine.Events.UnityAction<float> cb)
    {
        var ctr = new Vector2(0.5f, 0.5f);
        UIK.Txt(p, label, 34, TextAnchor.MiddleRight, new Color(0.95f, 0.95f, 0.9f), ctr, ctr, new Vector2(-170, y), new Vector2(520, 50));
        UIK.MakeSlider(p, ctr, new Vector2(250, y), new Vector2(420, 40), val, cb);
    }

    void Show(GameObject g)
    {
        mainPanel.SetActive(g == mainPanel); settingsPanel.SetActive(g == settingsPanel); controlsPanel.SetActive(g == controlsPanel);
        bestT.text = "BEST SCORE: " + SaveManager.BestScore + "     TOTAL COINS: " + SaveManager.TotalCoins;
    }

    void RefreshLabels()
    {
        qualityT.text = "GRAPHICS QUALITY: " + QualitySettings.names[Mathf.Clamp(SaveManager.Quality, 0, QualitySettings.names.Length - 1)].ToUpper() + "  (click to change)";
        dayT.text = "TIME OF DAY: " + (SaveManager.Day ? "DAY" : "NIGHT") + "  (click to change)";
        diffT.text = "DIFFICULTY: " + SaveManager.DifficultyName + "  (click to change)";
    }

    void CycleQuality()
    {
        int n = QualitySettings.names.Length;
        SaveManager.Quality = (Mathf.Clamp(SaveManager.Quality, 0, n - 1) + 1) % n;
        SaveManager.ApplyQuality(); RefreshLabels();
    }
    void CycleDay() { SaveManager.Day = !SaveManager.Day; SaveManager.Save(); SceneManager.LoadScene("MainMenu"); }
    void CycleDifficulty() { SaveManager.Difficulty = (SaveManager.Difficulty + 1) % 3; RefreshLabels(); }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
