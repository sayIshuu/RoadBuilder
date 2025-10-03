using System;
using UnityEngine;
using System.Globalization;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    public int maxLives = 5;
    private int _currentLives;
    public int CurrentLives
    {
        get => _currentLives;
        set
        {
            if (_currentLives == value) return;
            _currentLives = value;
            OnCurrentLivesChange?.Invoke(value);
        }
    }
    public Action<int> OnCurrentLivesChange;

    private DateTime _lastLifeTime;
    private TimeSpan _recoveryInterval;

    const string KEY_LIFE = "CurrentLives";
    const string KEY_TIME = "LastLifeTime"; // ISO 8601 "O"로 저장

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // load
        if (PlayerPrefs.HasKey(KEY_LIFE))
            CurrentLives = PlayerPrefs.GetInt(KEY_LIFE);
        else
        {
            CurrentLives = maxLives;
            PlayerPrefs.SetInt(KEY_LIFE, _currentLives);
        }

        if (PlayerPrefs.HasKey(KEY_TIME))
            _lastLifeTime = DateTime.Parse(PlayerPrefs.GetString(KEY_TIME), null, DateTimeStyles.RoundtripKind);
        else
        {
            _lastLifeTime = DateTime.Now;
            PlayerPrefs.SetString(KEY_TIME, _lastLifeTime.ToString("O"));
        }

        _recoveryInterval = TimeSpan.FromMinutes(30);

        RecoverLives(); // 최초 1회 계산
    }

    void Update()
    {
        RecoverLives();
    }

    private void RecoverLives()
    {
        // 최대치면 즉시 리필 방지 위해 기준시간을 현재로 유지
        if (_currentLives >= maxLives)
        {
            // 기준 시간 유지(다음에 1 깎을 때부터 30분 측정 시작)
            _lastLifeTime = DateTime.Now;
            // 저장 최소화: 너무 자주 저장 안 하려면 조건부 저장 가능
            PlayerPrefs.SetString(KEY_TIME, _lastLifeTime.ToString("O"));
            return;
        }

        TimeSpan elapsed = DateTime.Now - _lastLifeTime;
        bool changed = false;

        while (elapsed >= _recoveryInterval && _currentLives < maxLives)
        {
            CurrentLives++;
            _lastLifeTime += _recoveryInterval;
            elapsed -= _recoveryInterval;
            changed = true;
        }

        if (changed)
        {
            PlayerPrefs.SetInt(KEY_LIFE, _currentLives);
            PlayerPrefs.SetString(KEY_TIME, _lastLifeTime.ToString("O"));
            PlayerPrefs.Save();
        }
    }

    public void UseLife()
    {
        if (_currentLives > 0)
        {
            bool wasMax = (_currentLives == maxLives);
            CurrentLives--;

            // 최대치에서 처음 깎는 순간부터 타이머 시작
            if (wasMax)
            {
                _lastLifeTime = DateTime.Now;
                PlayerPrefs.SetString(KEY_TIME, _lastLifeTime.ToString("O"));
            }

            PlayerPrefs.SetInt(KEY_LIFE, _currentLives);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("목숨이 부족합니다.");
        }
    }

    public void RecoverAllLife()
    {
        CurrentLives = maxLives;
        _lastLifeTime = DateTime.Now; // 최대치 유지 시 즉시 리필 방지 기준
        PlayerPrefs.SetInt(KEY_LIFE, _currentLives);
        PlayerPrefs.SetString(KEY_TIME, _lastLifeTime.ToString("O"));
        PlayerPrefs.Save();
    }
}
