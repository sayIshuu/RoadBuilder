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
        // 목숨이 가득 차 있으면 굳이 계산할 필요가 없으므로 함수를 종료합니다.
        if (_currentLives >= maxLives)
        {
            return;
        }

        TimeSpan timeSinceLastLife = DateTime.Now - _lastLifeTime;

        // if를 while로 변경: 회복 가능한 목숨을 모두 회복시킬 때까지 반복
        while (timeSinceLastLife >= _recoveryInterval)
        {
            if (_currentLives < maxLives)
            {
                CurrentLives++;

                // 현재 시간으로 초기화하는 대신, 마지막 회복 시간에 30분을 더해줍니다.
                // 이렇게 해야 누적된 시간을 정확하게 계산할 수 있습니다.
                _lastLifeTime += _recoveryInterval;
                timeSinceLastLife -= _recoveryInterval; // 남은 시간 갱신
            }
            else
            {
                // 목숨이 가득 차면 루프를 빠져나옵니다.
                break;
            }
        }

        // 최종적으로 계산된 값들을 저장합니다.
        PlayerPrefs.SetInt("CurrentLives", _currentLives);
        PlayerPrefs.SetString("LastLifeTime", _lastLifeTime.ToString());
        PlayerPrefs.Save();
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

    public void RecoverAllLife()
    {
        CurrentLives = maxLives;
        return;
    }
}
