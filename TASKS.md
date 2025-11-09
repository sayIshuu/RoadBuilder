# 광고 기능 추가 작업 목록

이 문서는 RoadBuilder 프로젝트에 두 가지 새로운 광고 기능을 추가하기 위한 상세한 작업 목록입니다.

**⚠️ 중요**: 이 파일은 작업 진행 중 계속 업데이트됩니다. 각 작업을 완료할 때마다 체크박스를 체크하고 진행 상황을 기록하세요.

---

## 📊 전체 진행 상황

### 기능 1: 5분 타이머 기반 전면광고
- [ ] Task 1.1: GameplayAdTimer 스크립트 생성
- [ ] Task 1.2: ResetButton 스크립트 수정
- [ ] Task 1.3: 씬에 GameplayAdTimer 오브젝트 추가

### 기능 2: 리롤 충전 보상형 광고
- [ ] Task 2.1: RerollButton 스크립트 수정
- [ ] Task 2.2: Unity UI에 보상형 광고 버튼 추가
- [ ] Task 2.3: (선택) 광고 시청 제한 기능 추가

### 테스트 및 배포
- [ ] 기능 1 테스트 완료
- [ ] 기능 2 테스트 완료
- [ ] 통합 테스트 완료
- [ ] 배포 전 체크리스트 완료

---

## 📋 목차

1. [기능 1: 5분 타이머 기반 전면광고](#기능-1-5분-타이머-기반-전면광고)
2. [기능 2: 리롤 충전 보상형 광고](#기능-2-리롤-충전-보상형-광고)
3. [테스트 체크리스트](#테스트-체크리스트)

---

## 기능 1: 5분 타이머 기반 전면광고

### 요구사항
- 게임오버 후 다시시작 버튼을 누를 때 전면광고가 등장
- 매번이 아니라 게임을 켜고 5분의 타이머가 채워질 때마다 다시 시작 버튼을 누르면 등장
- 메인 플레이씬에서만 타이머가 시작 (튜토리얼씬에서는 광고 나오면 안됨)
- 타이머가 다 채워지면 다음 다시시작 버튼 클릭 시 전면광고 등장

---

### Task 1.1: GameplayAdTimer 스크립트 생성

**파일 경로**: `Assets/Scripts/AdMob/GameplayAdTimer.cs`

**구현 내용**:
```csharp
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameplayAdTimer : MonoBehaviour
{
    public static GameplayAdTimer Instance { get; private set; }

    private const float AD_INTERVAL_SECONDS = 300f; // 5분 = 300초
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
```

**체크포인트**:
- [ ] `Assets/Scripts/AdMob` 폴더에 `GameplayAdTimer.cs` 파일 생성
- [ ] 싱글톤 패턴 구현 (DontDestroyOnLoad 사용)
- [ ] PlayerPrefs로 마지막 광고 표시 시간 저장/로드
- [ ] 5분(300초) 타이머 로직 구현
- [ ] `ShouldShowInterstitial()` 메서드로 광고 표시 여부 판단
- [ ] `ResetTimer()` 메서드로 타이머 초기화

---

### Task 1.2: ResetButton 스크립트 수정

**파일 경로**: `Assets/Scripts/ResetButton.cs`

**수정 내용**:

기존 `Reset()` 메서드를 다음과 같이 수정:

```csharp
public void Reset()
{
    // 메인씬에서만 광고 타이머 체크
    if (SceneManager.GetActiveScene().name == "MainScene")
    {
        // 5분 타이머가 완료되었는지 체크
        if (GameplayAdTimer.Instance != null && GameplayAdTimer.Instance.ShouldShowInterstitial())
        {
            // 전면광고 표시
            SoundManager.Instance.PlaySelectSound();

            AdsManager.I.TryShowInterstitial(onClosed: () =>
            {
                // 광고가 닫힌 후 타이머 리셋
                GameplayAdTimer.Instance.ResetTimer();

                // 씬 재시작
                SceneManager.LoadScene("MainScene");
            });

            return; // 광고 표시 후 함수 종료
        }
    }

    // 타이머가 완료되지 않았거나 튜토리얼씬인 경우 바로 재시작
    SoundManager.Instance.PlaySelectSound();
    SceneManager.LoadScene("MainScene");
}
```

**필요한 using 문 추가**:
```csharp
using UnityEngine.SceneManagement;
```

**체크포인트**:
- [ ] `Reset()` 메서드에 씬 이름 체크 로직 추가
- [ ] GameplayAdTimer를 통한 타이머 확인 로직 추가
- [ ] `AdsManager.I.TryShowInterstitial()` 호출 추가
- [ ] 광고 종료 후 타이머 리셋 및 씬 재시작 처리
- [ ] 튜토리얼씬에서는 광고가 나오지 않도록 분기 처리

---

### Task 1.3: 씬에 GameplayAdTimer 오브젝트 추가

**Unity Editor 작업**:

1. **빈 GameObject 생성**:
   - Hierarchy에서 우클릭 → Create Empty
   - 이름: `GameplayAdTimer`

2. **GameplayAdTimer 컴포넌트 추가**:
   - Inspector에서 Add Component
   - `GameplayAdTimer` 스크립트 추가

3. **씬 저장**:
   - MainScene에 추가 (또는 시작 씬에 추가하여 DontDestroyOnLoad로 유지)

**체크포인트**:
- [ ] GameplayAdTimer GameObject 생성
- [ ] GameplayAdTimer.cs 컴포넌트 추가
- [ ] 씬 저장

---

## 기능 2: 리롤 충전 보상형 광고

### 요구사항
- 리롤횟수가 없을 때 리롤 버튼 위에 보상형 광고 보기 버튼 등장
- 버튼을 클릭하고 광고를 시청하면 리롤횟수를 3회 충전

---

### Task 2.1: RerollButton 스크립트 수정

**파일 경로**: `Assets/Scripts/RerollButton.cs`

**수정 내용**:

#### 1. 필드 추가

```csharp
[SerializeField] private GameObject rewardedAdButton; // 보상형 광고 버튼
```

#### 2. `UpdateRerollCountUI()` 메서드 추가

기존 `rerollCountText.text` 업데이트 로직을 메서드로 분리:

```csharp
private void UpdateRerollCountUI()
{
    rerollCountText.text = rerollCount.ToString();

    // 리롤 횟수가 0이면 광고 버튼 표시, 아니면 숨김
    if (rewardedAdButton != null)
    {
        rewardedAdButton.SetActive(rerollCount == 0);
    }
}
```

#### 3. 기존 메서드 수정

`Awake()`, `Reroll()`, `PlusRerollCount()` 메서드에서 `rerollCountText.text = ...` 부분을 `UpdateRerollCountUI()`로 대체:

```csharp
void Awake()
{
    rerollCount = 3;
    UpdateRerollCountUI(); // 변경
}

public void Reroll()
{
    if (rerollCount == 0)
    {
        SoundManager.Instance.PlayForbidSound();
        return;
    }

    if (BoardCheck.gameover) return;
    if (BoardCheck.isBoardChecking) return;

    SoundManager.Instance.PlaySelectSound();
    rerollCount--;
    tileGenerator.Reroll();

    UpdateRerollCountUI(); // 변경
}

public void PlusRerollCount()
{
    rerollCount++;
    UpdateRerollCountUI(); // 변경
}
```

#### 4. `WatchAdForReroll()` 메서드 추가

보상형 광고를 시청하여 리롤 충전:

```csharp
public void WatchAdForReroll()
{
    if (AdsManager.I == null)
    {
        Debug.LogWarning("[RerollButton] AdsManager not available");
        return;
    }

    AdsManager.I.TryShowRewarded(
        onReward: () =>
        {
            // 광고 시청 완료 시 리롤 3회 충전
            rerollCount += 3;
            UpdateRerollCountUI();

            SoundManager.Instance.PlaySelectSound();
            Debug.Log("[RerollButton] Rewarded 3 rerolls from ad");
        },
        onClosed: () =>
        {
            // 광고가 닫혔을 때 (선택적 처리)
            Debug.Log("[RerollButton] Rewarded ad closed");
        }
    );
}
```

**전체 수정된 RerollButton.cs**:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RerollButton : MonoBehaviour
{
    [SerializeField] private TileGenerator tileGenerator;

    private int rerollCount;
    [SerializeField] private TextMeshProUGUI rerollCountText;
    [SerializeField] private GameObject rewardedAdButton; // 추가

    void Awake()
    {
        rerollCount = 3;
        UpdateRerollCountUI(); // 변경
    }

    public void Reroll()
    {
        if (rerollCount == 0)
        {
            SoundManager.Instance.PlayForbidSound();
            return;
        }

        if (BoardCheck.gameover) return;
        if (BoardCheck.isBoardChecking) return;

        SoundManager.Instance.PlaySelectSound();
        rerollCount--;
        tileGenerator.Reroll();

        UpdateRerollCountUI(); // 변경
    }

    public void PlusRerollCount()
    {
        rerollCount++;
        UpdateRerollCountUI(); // 변경
    }

    // 추가: UI 업데이트 메서드
    private void UpdateRerollCountUI()
    {
        rerollCountText.text = rerollCount.ToString();

        // 리롤 횟수가 0이면 광고 버튼 표시, 아니면 숨김
        if (rewardedAdButton != null)
        {
            rewardedAdButton.SetActive(rerollCount == 0);
        }
    }

    // 추가: 보상형 광고 시청 메서드
    public void WatchAdForReroll()
    {
        if (AdsManager.I == null)
        {
            Debug.LogWarning("[RerollButton] AdsManager not available");
            return;
        }

        AdsManager.I.TryShowRewarded(
            onReward: () =>
            {
                // 광고 시청 완료 시 리롤 3회 충전
                rerollCount += 3;
                UpdateRerollCountUI();

                SoundManager.Instance.PlaySelectSound();
                Debug.Log("[RerollButton] Rewarded 3 rerolls from ad");
            },
            onClosed: () =>
            {
                // 광고가 닫혔을 때 (선택적 처리)
                Debug.Log("[RerollButton] Rewarded ad closed");
            }
        );
    }
}
```

**체크포인트**:
- [ ] `rewardedAdButton` GameObject 필드 추가
- [ ] `UpdateRerollCountUI()` 메서드 구현
- [ ] 기존 메서드에서 UI 업데이트 로직을 `UpdateRerollCountUI()` 호출로 변경
- [ ] `WatchAdForReroll()` 메서드 구현
- [ ] 보상형 광고 시청 시 리롤 3회 충전 로직 구현
- [ ] 리롤 횟수 0일 때 광고 버튼 표시, 아니면 숨김 처리

---

### Task 2.2: Unity UI에 보상형 광고 버튼 추가

**Unity Editor 작업**:

#### 1. 보상형 광고 버튼 생성

**MainScene의 Reroll UI 영역에 버튼 추가**:

1. **Hierarchy 탐색**:
   - Reroll 버튼이 있는 Canvas 찾기
   - Reroll 버튼 근처에 새 Button 추가

2. **Button 생성**:
   - 우클릭 → UI → Button - TextMeshPro
   - 이름: `RewardedAdButton`

3. **Button 위치 조정**:
   - Reroll 버튼 위 또는 옆에 배치
   - Rect Transform 조정 (예: Reroll 버튼 위 50px)

4. **Button 텍스트 수정**:
   - 자식 Text (TMP) 오브젝트 선택
   - Text: "광고 보고 충전" 또는 "📺 리롤 충전"
   - Font Size: 적절히 조정

5. **Button 이미지 설정**:
   - Button 컴포넌트의 Image 색상 변경 (예: 노란색/금색)
   - 또는 별도 광고 아이콘 이미지 사용

#### 2. RerollButton 스크립트 연결

1. **Reroll 버튼 오브젝트 선택**:
   - RerollButton 컴포넌트가 붙어있는 GameObject 선택

2. **Inspector에서 참조 연결**:
   - `Rewarded Ad Button` 필드에 방금 만든 `RewardedAdButton` 드래그 앤 드롭

3. **RewardedAdButton의 onClick 이벤트 설정**:
   - RewardedAdButton GameObject 선택
   - Button 컴포넌트의 On Click() 이벤트에 추가
   - RerollButton GameObject를 드래그
   - Function: `RerollButton.WatchAdForReroll()`

#### 3. 초기 상태 설정

- **RewardedAdButton GameObject 비활성화**:
  - Inspector에서 체크박스 해제 (게임 시작 시 리롤 횟수가 3이므로 숨김)

**체크포인트**:
- [ ] MainScene Canvas에 보상형 광고 버튼 추가
- [ ] 버튼 위치와 디자인 조정
- [ ] RerollButton 스크립트의 `rewardedAdButton` 필드에 연결
- [ ] 버튼의 onClick 이벤트에 `WatchAdForReroll()` 메서드 연결
- [ ] 버튼 초기 상태 비활성화

---

### Task 2.3: (선택) 광고 시청 제한 기능 추가

보상형 광고를 무제한으로 시청할 수 있게 하거나, 제한을 둘 수 있습니다.

**제한 옵션**:
- 게임 세션당 최대 N회
- 하루 최대 N회
- 쿨타임 적용 (예: 30초마다 1회)

**구현 예시 (게임 세션당 최대 3회 제한)**:

```csharp
private int maxAdWatchCount = 3; // 최대 광고 시청 횟수
private int currentAdWatchCount = 0; // 현재 광고 시청 횟수

public void WatchAdForReroll()
{
    // 광고 시청 횟수 제한 체크
    if (currentAdWatchCount >= maxAdWatchCount)
    {
        Debug.Log("[RerollButton] Maximum ad watch count reached");
        SoundManager.Instance.PlayForbidSound();
        // UI에 메시지 표시 (선택사항)
        return;
    }

    if (AdsManager.I == null)
    {
        Debug.LogWarning("[RerollButton] AdsManager not available");
        return;
    }

    AdsManager.I.TryShowRewarded(
        onReward: () =>
        {
            rerollCount += 3;
            currentAdWatchCount++; // 시청 횟수 증가
            UpdateRerollCountUI();

            SoundManager.Instance.PlaySelectSound();
            Debug.Log($"[RerollButton] Rewarded 3 rerolls from ad ({currentAdWatchCount}/{maxAdWatchCount})");
        },
        onClosed: () =>
        {
            Debug.Log("[RerollButton] Rewarded ad closed");
        }
    );
}
```

**체크포인트**:
- [ ] 광고 시청 제한 정책 결정
- [ ] 필요 시 제한 로직 구현
- [ ] 제한 도달 시 사용자 피드백 추가

---

## 테스트 체크리스트

### 기능 1: 전면광고 테스트

#### 테스트 환경 설정
- [ ] AdMobConfig에서 `useTestIds = true` 설정 (테스트 광고 사용)
- [ ] 실제 디바이스 또는 에뮬레이터에서 테스트

#### 테스트 시나리오

**시나리오 1: 메인씬에서 5분 타이머 작동**
- [ ] 게임 시작 (처음 실행)
- [ ] 메인씬 진입
- [ ] PlayerPrefs 확인: `LastAdShowTime` 키가 저장되었는지 확인
- [ ] 게임오버 후 다시시작 버튼 클릭
- [ ] **예상 결과**: 광고가 나오지 않고 바로 재시작 (타이머가 5분 미만)

**시나리오 2: 5분 경과 후 광고 표시**
- [ ] PlayerPrefs의 `LastAdShowTime`을 5분 이전 시간으로 수동 변경
  ```csharp
  // Unity Console에서 실행:
  PlayerPrefs.SetString("LastAdShowTime", DateTime.Now.AddMinutes(-6).ToString());
  ```
- [ ] 게임오버 후 다시시작 버튼 클릭
- [ ] **예상 결과**: 전면광고가 표시됨
- [ ] 광고 닫기
- [ ] **예상 결과**: 씬이 재시작되고 타이머가 리셋됨

**시나리오 3: 튜토리얼씬에서 광고 미표시**
- [ ] PlayerPrefs의 `TutorialCompleted` 삭제하여 튜토리얼 재시작
- [ ] 게임 시작
- [ ] 튜토리얼씬 진입
- [ ] (튜토리얼에 다시시작 버튼이 있다면) 다시시작 버튼 클릭
- [ ] **예상 결과**: 광고가 나오지 않고 바로 재시작

**시나리오 4: 타이머 리셋 확인**
- [ ] 5분 경과 후 광고 시청
- [ ] 게임 플레이 (5분 미만)
- [ ] 게임오버 후 다시시작 버튼 클릭
- [ ] **예상 결과**: 광고가 나오지 않음 (타이머가 리셋되었으므로)

#### 디버깅 팁
- [ ] Debug.Log 출력 확인: `[GameplayAdTimer]` 태그로 필터링
- [ ] 남은 시간 확인: `GameplayAdTimer.Instance.GetRemainingSeconds()` 호출

---

### 기능 2: 보상형 광고 테스트

#### 테스트 환경 설정
- [ ] AdMobConfig에서 `useTestIds = true` 설정
- [ ] 실제 디바이스 또는 에뮬레이터에서 테스트

#### 테스트 시나리오

**시나리오 1: 초기 상태**
- [ ] 게임 시작
- [ ] 리롤 횟수 확인: 3회
- [ ] **예상 결과**: 보상형 광고 버튼이 숨겨져 있음

**시나리오 2: 리롤 3회 사용 후 광고 버튼 표시**
- [ ] 리롤 버튼 3회 클릭하여 리롤 횟수 소진
- [ ] 리롤 횟수 확인: 0회
- [ ] **예상 결과**: 보상형 광고 버튼이 나타남

**시나리오 3: 리롤 횟수 0일 때 리롤 버튼 클릭**
- [ ] 리롤 횟수 0인 상태에서 리롤 버튼 클릭
- [ ] **예상 결과**: Forbid 사운드 재생, 아무 동작 없음

**시나리오 4: 보상형 광고 시청**
- [ ] 보상형 광고 버튼 클릭
- [ ] **예상 결과**: 보상형 광고 표시
- [ ] 광고 시청 완료
- [ ] **예상 결과**:
  - 리롤 횟수 3회 충전 (0 → 3)
  - 보상형 광고 버튼 숨김
  - Select 사운드 재생

**시나리오 5: 광고 시청 중 취소**
- [ ] 리롤 횟수 다시 0으로 만들기
- [ ] 보상형 광고 버튼 클릭
- [ ] 광고를 완료하지 않고 X 버튼으로 닫기
- [ ] **예상 결과**: 리롤 횟수 증가하지 않음

**시나리오 6: 광고 충전 후 다시 소진**
- [ ] 보상형 광고로 리롤 3회 충전
- [ ] 리롤 3회 사용
- [ ] **예상 결과**: 다시 보상형 광고 버튼 나타남

**시나리오 7: (제한 기능 구현 시) 최대 시청 횟수 도달**
- [ ] 보상형 광고를 N회 시청 (maxAdWatchCount)
- [ ] N+1회 시도
- [ ] **예상 결과**: Forbid 사운드 재생, 광고 표시되지 않음

#### 디버깅 팁
- [ ] Debug.Log 출력 확인: `[RerollButton]` 태그로 필터링
- [ ] Inspector에서 `rerollCount` 값 실시간 확인
- [ ] `rewardedAdButton.activeSelf` 상태 확인

---

### 통합 테스트

#### 시나리오: 두 광고 기능 동시 작동
- [ ] 게임 시작
- [ ] 리롤 3회 사용하여 보상형 광고 버튼 표시
- [ ] 보상형 광고 시청하여 리롤 충전
- [ ] 5분 경과 (또는 PlayerPrefs 조작)
- [ ] 게임오버 후 다시시작 버튼 클릭
- [ ] **예상 결과**: 전면광고 표시
- [ ] 씬 재시작 후 리롤 횟수 확인
- [ ] **예상 결과**: 리롤 횟수 3 (초기 상태로 리셋됨)

#### 성능 테스트
- [ ] 메모리 누수 확인 (여러 번 광고 시청)
- [ ] 광고 로딩 실패 시 동작 확인
- [ ] 인터넷 연결 없을 때 동작 확인

---

## 배포 전 체크리스트

### 코드 검증
- [ ] 모든 Debug.Log 메시지 확인 (필요 시 제거 또는 주석 처리)
- [ ] 예외 처리 확인 (null 체크, AdsManager 없을 때 등)
- [ ] 코드 리뷰 완료

### AdMob 설정
- [ ] **중요**: `AdMobConfig`에서 `useTestIds = false` 설정
- [ ] 실제 광고 단위 ID 확인
- [ ] Google AdMob 콘솔에서 광고 단위 활성화 확인

### Unity 설정
- [ ] GameplayAdTimer GameObject가 씬에 추가되었는지 확인
- [ ] RerollButton의 `rewardedAdButton` 참조가 연결되었는지 확인
- [ ] 모든 버튼의 onClick 이벤트가 올바르게 설정되었는지 확인

### 빌드 테스트
- [ ] Android 빌드 테스트
- [ ] iOS 빌드 테스트 (해당 시)
- [ ] 실제 디바이스에서 광고 표시 확인

### 문서화
- [ ] 코드 주석 추가
- [ ] 변경 사항 커밋 메시지 작성
- [ ] (필요 시) README.md 업데이트

---

## 참고 사항

### 기존 코드 구조
- **AdsManager.cs**: 광고 관리 싱글톤 (`Assets/Scripts/AdMob/AdsManager.cs`)
- **AdMobService.cs**: AdMob SDK 래퍼 (`Assets/Scripts/AdMob/AdMobService.cs`)
- **ResetButton.cs**: 다시시작 버튼 (`Assets/Scripts/ResetButton.cs`)
- **RerollButton.cs**: 리롤 버튼 (`Assets/Scripts/RerollButton.cs`)

### 주요 메서드
- `AdsManager.I.TryShowInterstitial(Action onClosed)`: 전면광고 표시
- `AdsManager.I.TryShowRewarded(Action onReward, Action onClosed)`: 보상형 광고 표시
- `SceneManager.GetActiveScene().name`: 현재 씬 이름 가져오기

### 싱글톤 패턴
이 프로젝트는 여러 싱글톤을 사용합니다:
- `AdsManager.I`
- `SoundManager.Instance`
- `GameplayAdTimer.Instance` (신규 추가)

모두 `DontDestroyOnLoad`를 사용하여 씬 전환 시에도 유지됩니다.

---

## 추가 개선 아이디어 (선택사항)

### UI 개선
- [ ] 광고 로딩 인디케이터 추가
- [ ] 타이머 남은 시간 표시 UI (예: "다음 광고까지 2분 30초")
- [ ] 리롤 충전 애니메이션 추가

### 분석 및 추적
- [ ] Firebase Analytics 이벤트 추가:
  - `interstitial_ad_shown`
  - `rewarded_ad_watched`
  - `reroll_refilled`

### 사용자 경험
- [ ] 첫 광고 시청 시 튜토리얼 추가
- [ ] 보상형 광고 버튼에 애니메이션 효과 추가 (펄스, 빛남 등)

---

## 문제 해결

### 광고가 표시되지 않을 때
1. **AdMobConfig 확인**:
   - `useTestIds`가 올바르게 설정되었는지 확인
   - 광고 단위 ID가 올바른지 확인

2. **AdsManager 초기화 확인**:
   - AdsManager GameObject가 씬에 있는지 확인
   - Console에서 초기화 로그 확인

3. **광고 프리로드 확인**:
   - `AdsManager.I.TryShowInterstitial()` 호출 전에 광고가 프리로드되었는지 확인
   - AdMobService에서 자동 프리로드됨

4. **인터넷 연결 확인**:
   - 디바이스가 인터넷에 연결되어 있는지 확인

### PlayerPrefs 초기화
테스트를 위해 PlayerPrefs를 초기화하려면:
```csharp
// Unity Console에서 실행:
PlayerPrefs.DeleteKey("LastAdShowTime");
PlayerPrefs.DeleteKey("TutorialCompleted");
PlayerPrefs.Save();
```

---

**작성일**: 2025-11-05
**버전**: 1.0
**작성자**: Claude Code
