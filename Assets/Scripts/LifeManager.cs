using System;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    //싱글톤
    public static LifeManager Instance { get; private set; }

    public int maxLives = 3; // 최대 목숨 수
    private int _currentLives; // 현재 목숨 수
    public int CurrentLives
    {
        get { return _currentLives; }
        set
        {
            _currentLives = value;
            OnCurrentLivesChange?.Invoke(value);
        }
    }
    public Action<int> OnCurrentLivesChange;

    private DateTime _lastLifeTime; // 마지막 목숨 회복 시간
    private TimeSpan _recoveryInterval; // 30분 간격으로 회복

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // 게임 시작 시 목숨 수 초기화
        // PlayerPrefs에서 현재 목숨을 불러오고, 만약 처음 실행이라면 기본값 설정
        if (!PlayerPrefs.HasKey("CurrentLives"))
        {
            CurrentLives = maxLives; // 처음 실행 시 maxLives 값을 사용
            PlayerPrefs.SetInt("CurrentLives", _currentLives); // 기본 값 저장
        }
        else
        {
            CurrentLives = PlayerPrefs.GetInt("CurrentLives"); // 기존 값 불러오기
        }

        // 마지막 목숨 회복 시간 확인
        if (!PlayerPrefs.HasKey("LastLifeTime"))
        {
            _lastLifeTime = DateTime.Now; // 처음 실행 시 현재 시간으로 설정
            PlayerPrefs.SetString("LastLifeTime", _lastLifeTime.ToString()); // 기본 값 저장
        }
        else
        {
            _lastLifeTime = DateTime.Parse(PlayerPrefs.GetString("LastLifeTime")); // 기존 값 불러오기
        }
        _recoveryInterval = TimeSpan.FromMinutes(30);

        // 목숨 회복 여부 확인
        RecoverLives();
    }

    void Update()
    {
        // 주기적으로 목숨 회복 여부 확인
        RecoverLives();
    }

    private void RecoverLives()
    {
        // 마지막 목숨 회복 시간에서 현재 시간 차이 계산
        TimeSpan timeSinceLastLife = DateTime.Now - _lastLifeTime;

        if (timeSinceLastLife >= _recoveryInterval)
        {
            // 30분이 경과하면 목숨 1회복
            if (_currentLives < maxLives)
            {
                CurrentLives++;
                _lastLifeTime = DateTime.Now; // 마지막 회복 시간을 현재 시간으로 업데이트
                PlayerPrefs.SetInt("CurrentLives", _currentLives);
                PlayerPrefs.SetString("LastLifeTime", _lastLifeTime.ToString());
                PlayerPrefs.Save();
            }
        }
    }

    public void UseLife()
    {
        // 게임 진행 시 목숨 차감
        if (_currentLives > 0)
        {
            CurrentLives--;
            PlayerPrefs.SetInt("CurrentLives", _currentLives);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("목숨이 부족합니다.");
        }
    }
}
