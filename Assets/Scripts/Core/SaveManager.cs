using UnityEngine;

/// <summary>PlayerPrefs wrapper: best score, total coins, audio, graphics, difficulty.</summary>
public static class SaveManager
{
    public static int BestScore { get { return PlayerPrefs.GetInt("zr_best", 0); } set { PlayerPrefs.SetInt("zr_best", value); } }
    public static int TotalCoins { get { return PlayerPrefs.GetInt("zr_coins", 0); } set { PlayerPrefs.SetInt("zr_coins", value); } }
    public static float MusicVolume { get { return PlayerPrefs.GetFloat("zr_music", 0.6f); } set { PlayerPrefs.SetFloat("zr_music", value); } }
    public static float SfxVolume { get { return PlayerPrefs.GetFloat("zr_sfx", 0.8f); } set { PlayerPrefs.SetFloat("zr_sfx", value); } }
    public static float MouseSens { get { return PlayerPrefs.GetFloat("zr_mouse", 0.5f); } set { PlayerPrefs.SetFloat("zr_mouse", value); } }
    public static int Difficulty { get { return PlayerPrefs.GetInt("zr_diff", 1); } set { PlayerPrefs.SetInt("zr_diff", value); } }
    public static int Quality { get { return PlayerPrefs.GetInt("zr_quality", QualitySettings.GetQualityLevel()); } set { PlayerPrefs.SetInt("zr_quality", value); } }

    public static bool Day { get { return PlayerPrefs.GetInt("zr_day", 1) == 1; } set { PlayerPrefs.SetInt("zr_day", value ? 1 : 0); } }
    public static float DamageScale { get { return Difficulty == 0 ? 0.75f : (Difficulty == 2 ? 1.3f : 1f); } }
    public static float SpeedScale { get { return Difficulty == 0 ? 0.9f : (Difficulty == 2 ? 1.1f : 1f); } }
    public static float SpawnScale { get { return Difficulty == 0 ? 0.8f : (Difficulty == 2 ? 1.25f : 1f); } }
    public static string DifficultyName { get { return Difficulty == 0 ? "EASY" : (Difficulty == 2 ? "HARD" : "NORMAL"); } }

    public static void ApplyQuality()
    {
        if (PlayerPrefs.HasKey("zr_quality"))
            QualitySettings.SetQualityLevel(Mathf.Clamp(Quality, 0, QualitySettings.names.Length - 1), true);
    }
    public static void Save() { PlayerPrefs.Save(); }
}
