using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameplayAdTimer : MonoBehaviour
{
    public static GameplayAdTimer Instance { get; private set; }

    [SerializeField] private float AD_INTERVAL_SECONDS = 300f; // 5분 = 300초
    private const string LAST_AD_TIME_KEY = "LastAdShowTime";

    private DateTime lastAdShowTime;
    private bool isTimerReady = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLastAdTime();
        CheckIfTimerReady();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 메인씬에 진입하면 타이머 체크
        if (scene.name == "MainScene")
        {
            CheckIfTimerReady();
        }
    }

    private void LoadLastAdTime()
    {
        string savedTime = PlayerPrefs.GetString(LAST_AD_TIME_KEY, "");

        if (string.IsNullOrEmpty(savedTime))
        {
            // 첫 실행 시 현재 시간 저장
            lastAdShowTime = DateTime.Now;
            SaveLastAdTime();
        }
        else
        {
            lastAdShowTime = DateTime.Parse(savedTime);
        }
    }

    private void SaveLastAdTime()
    {
        PlayerPrefs.SetString(LAST_AD_TIME_KEY, lastAdShowTime.ToString());
        PlayerPrefs.Save();
    }

    private void CheckIfTimerReady()
    {
        TimeSpan elapsed = DateTime.Now - lastAdShowTime;
        isTimerReady = elapsed.TotalSeconds >= AD_INTERVAL_SECONDS;

        Debug.Log($"[GameplayAdTimer] Elapsed: {elapsed.TotalSeconds}s / {AD_INTERVAL_SECONDS}s, Ready: {isTimerReady}");
    }

    public bool ShouldShowInterstitial()
    {
        CheckIfTimerReady();
        return isTimerReady;
    }

    public void ResetTimer()
    {
        lastAdShowTime = DateTime.Now;
        isTimerReady = false;
        SaveLastAdTime();

        Debug.Log("[GameplayAdTimer] Timer reset. Next ad in 5 minutes.");
    }

    // 남은 시간 확인 (디버깅/UI용)
    public float GetRemainingSeconds()
    {
        TimeSpan elapsed = DateTime.Now - lastAdShowTime;
        float remaining = AD_INTERVAL_SECONDS - (float)elapsed.TotalSeconds;
        return Mathf.Max(0, remaining);
    }
}
