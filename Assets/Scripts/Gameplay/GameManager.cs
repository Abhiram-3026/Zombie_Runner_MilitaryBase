using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    public GameState State = GameState.Playing;
    public PlayerController player;
    public PlayerHealth health;
    public PlayerShooting shooting;
    public float distance, CurrentSpeed, Difficulty01, runTime;
    public int RunCoins, Kills, bonus;
    public bool newBest;
    bool banked;
    GameConfig cfg;

    public int Score { get { return Mathf.FloorToInt(distance) + bonus; } }
    public float SpawnScale { get { return SaveManager.SpawnScale; } }

    void Awake() { I = this; Time.timeScale = 1f; }

    public void Init(PlayerController p, PlayerHealth h, PlayerShooting s) { player = p; health = h; shooting = s; cfg = GameConfig.I; }

    void Update()
    {
        if (player == null) return;
        if (Input.GetKeyDown(KeyCode.Escape) && State != GameState.GameOver) TogglePause();
        if (State == GameState.GameOver && Input.GetKeyDown(KeyCode.R) && UIManager.I.OverVisible) Restart();
        if (State != GameState.Playing) return;
        float dt = Time.deltaTime;
        runTime += dt;
        distance = player.transform.position.z;
        Difficulty01 = Mathf.Clamp01(distance / cfg.distanceToMaxDifficulty);
        float ramp = Mathf.Lerp(0.4f, 1f, Mathf.Clamp01(runTime / 2f));
        CurrentSpeed = Mathf.Lerp(cfg.baseSpeed, cfg.maxSpeed, Difficulty01) * SaveManager.SpeedScale * ramp * PowerUpManager.I.SpeedMultiplier;
        Collectible.Tick(dt, player.transform.position);
    }

    public void AddCoin()
    {
        int m = PowerUpManager.I.CoinMultiplier;
        RunCoins += cfg.coinValue * m;
        bonus += cfg.coinScore * m;
    }
    public void AddScore(int s) { bonus += s; }
    public void AddKill(ZombieType t) { Kills++; bonus += cfg.killScore * (t == ZombieType.Armored ? 2 : 1); }

    public void TogglePause()
    {
        if (State == GameState.Playing) { State = GameState.Paused; Time.timeScale = 0f; UIManager.I.ShowPause(true); }
        else if (State == GameState.Paused) { State = GameState.Playing; Time.timeScale = 1f; UIManager.I.ShowPause(false); }
    }

    public void TriggerGameOver()
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;
        CurrentSpeed = 0f;
        Bank();
        newBest = Score > SaveManager.BestScore;
        if (newBest) SaveManager.BestScore = Score;
        SaveManager.Save();
        UIManager.I.ShowGameOverDelayed(1.3f);
    }

    /// <summary>Adds this run's coins to the saved total exactly once.</summary>
    public void Bank()
    {
        if (banked) return;
        banked = true;
        SaveManager.TotalCoins += RunCoins;
        SaveManager.Save();
    }

    public void Restart() { Time.timeScale = 1f; Bank(); SceneManager.LoadScene("Gameplay"); }
    public void ToMenu() { Time.timeScale = 1f; Bank(); SceneManager.LoadScene("MainMenu"); }
}
