using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Ready,
        Running,
        Paused,
        GameOver
    }

    public GameState State = GameState.Ready;

    public int Score { get; private set; }
    public int Lives { get; private set; } = 3;
    public int HighScore { get; private set; }
    public int Level { get; private set; } = 1;
    public int TotalKills { get; private set; }

    public const int MaxGunLevel = 4;
    public int GunLevel { get { return Mathf.Clamp(Level, 1, MaxGunLevel); } }

    [Header("Effects")]
    public Sprite[] explosionFrames;

    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public bool CanActivate()
    {
        return State == GameState.Running;
    }

    public void StartGame()
    {
        Score = 0;
        Lives = 3;
        Level = 1;
        TotalKills = 0;
        State = GameState.Running;
        if (HUD.Instance != null) HUD.Instance.OnGameStarted();
    }

    public void NotifyChickenKill()
    {
        if (State != GameState.Running) return;
        TotalKills++;
    }

    public void OnBossKilled(Vector3 position)
    {
        if (State != GameState.Running) return;
        Level++;
        if (HUD.Instance != null)
        {
            HUD.Instance.ShowBossBar(false);
            HUD.Instance.ShowBanner("LEVEL " + Level);
            HUD.Instance.SetGunLevel(GunLevel);
        }
    }

    public void PauseGame()
    {
        if (State != GameState.Running) return;
        State = GameState.Paused;
        Time.timeScale = 0f;
        if (HUD.Instance != null) HUD.Instance.ShowPause(true);
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        State = GameState.Running;
        Time.timeScale = 1f;
        if (HUD.Instance != null) HUD.Instance.ShowPause(false);
    }

    public void AddScore(int points)
    {
        if (State != GameState.Running) return;

        Score += points;
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }

        if (HUD.Instance != null) HUD.Instance.SetScore(Score, HighScore);
    }

    public void LoseLife()
    {
        if (State != GameState.Running) return;

        Lives--;
        if (HUD.Instance != null) HUD.Instance.SetLives(Lives);

        if (Lives <= 0)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        State = GameState.GameOver;
        Time.timeScale = 0f;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOver();
        if (HUD.Instance != null) HUD.Instance.ShowGameOver(Score, HighScore);
    }

    public void SpawnExplosion(Vector3 position)
    {
        if (explosionFrames == null || explosionFrames.Length == 0) return;

        GameObject boom = new GameObject("Explosion");
        boom.transform.position = position;
        SpriteRenderer sr = boom.AddComponent<SpriteRenderer>();
        sr.sprite = explosionFrames[0];
        sr.sortingOrder = 20;

        float targetHeight = 1.6f;
        float factor = targetHeight / sr.bounds.size.y;
        boom.transform.localScale = Vector3.one * factor;

        ExplosionEffect effect = boom.AddComponent<ExplosionEffect>();
        effect.frames = explosionFrames;
        effect.frameTime = 0.05f;
    }

    public void RestartGame()
    {
        ResetToReady();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetToReady()
    {
        Time.timeScale = 1f;
        State = GameState.Ready;
        Score = 0;
        Lives = 3;
        Level = 1;
        TotalKills = 0;
    }

    public void GoToMainMenu()
    {
        ResetToReady();
        SceneManager.LoadScene("SplashScene");
    }
}