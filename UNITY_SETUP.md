# Unity Editor Setup Instructions

이 문서는 광고 기능 구현을 위한 Unity Editor에서의 설정 방법을 설명합니다.

## 전체 작업 요약

코드 수정이 완료되었으며, 다음 Unity Editor 작업이 필요합니다:

1. **GameplayAdTimer GameObject 추가** (기능 1: 5분 타이머 전면광고)
2. **RewardedAdButton GameObject 추가** (기능 2: 리롤 충전 보상형 광고)

---

## 1. GameplayAdTimer GameObject 설정

### 목적
5분 타이머 기반 전면광고 기능을 위한 싱글톤 매니저 오브젝트 생성

### 단계별 설정

#### 1.1. GameObject 생성
1. Unity Editor에서 **MainScene** 열기 (`Assets/Scenes/MainScene.unity`)
2. **Hierarchy** 패널에서 우클릭 → **Create Empty**
3. 새로 생성된 GameObject 이름을 **`GameplayAdTimer`**로 변경

#### 1.2. 컴포넌트 추가
1. `GameplayAdTimer` GameObject 선택
2. **Inspector** 패널에서 **Add Component** 클릭
3. 검색창에 `GameplayAdTimer` 입력
4. **GameplayAdTimer.cs** 스크립트 선택하여 추가

#### 1.3. 확인사항
- GameplayAdTimer GameObject가 MainScene의 루트 레벨에 있는지 확인
- GameplayAdTimer 컴포넌트가 올바르게 추가되었는지 확인
- 이 오브젝트는 `DontDestroyOnLoad`로 씬 전환 시에도 유지됩니다

#### 1.4. 씬 저장
- **Ctrl + S** 또는 **File → Save** 로 씬 저장

---

## 2. RewardedAdButton GameObject 설정

### 목적
리롤 횟수가 0일 때 표시되는 보상형 광고 버튼 UI 생성

### ✨ 최적화 포인트: CanvasGroup 사용
이 구현은 `GameObject.SetActive()` 대신 **CanvasGroup**을 사용하여 성능을 최적화합니다:
- **SetActive 방식**: GameObject 비활성화 시 OnDisable/OnEnable 호출, 레이아웃 재계산
- **CanvasGroup 방식**: GameObject는 활성 유지, 시각적 표시와 상호작용만 제어
  - `alpha`: 투명도로 시각적 숨김/표시
  - `interactable`: 클릭 가능 여부
  - `blocksRaycasts`: UI 레이캐스트 차단 여부

### 단계별 설정

#### 2.1. 기존 RerollButton GameObject 찾기

1. **Hierarchy** 패널에서 Canvas 및 하위 오브젝트 탐색
2. **RerollButton** (또는 리롤 기능을 담당하는 GameObject) 찾기
   - 힌트: `RerollButton.cs` 컴포넌트가 붙어있는 오브젝트
   - 일반적으로 Canvas > UI 요소들 중에 위치

#### 2.2. 보상형 광고 버튼 생성 (독립 오브젝트)

**중요**: RewardedAdButton은 RerollButton의 **자식이 아닌 별도의 독립적인 오브젝트**로 생성합니다.

1. **Canvas를 우클릭** (또는 RerollButton이 있는 UI 컨테이너) → **UI** → **Button - TextMeshPro**
   - RerollButton과 **같은 레벨**에 버튼이 생성됩니다 (형제 관계)
2. 새 버튼 이름을 **`RewardedAdButton`**으로 변경

#### 2.3. 버튼 위치 및 크기 조정

**Rect Transform 설정 예시**:
```
Position: RerollButton 위 또는 옆에 배치 (사용자가 쉽게 찾을 수 있는 위치)
Width: 150~200
Height: 40~60
Anchors: 적절히 설정 (예: 중앙 정렬)
```

**권장 위치**:
- RerollButton 바로 위 (50~80px 상단)
- RerollButton과 같은 수평선상에서 옆
- 게임 UI 레이아웃에 맞게 조정

#### 2.4. 버튼 텍스트 수정

1. `RewardedAdButton`의 자식 오브젝트인 **Text (TMP)** 선택
2. **Inspector**에서 TextMeshProUGUI 컴포넌트 수정:
   - **Text**: `"광고 보고 충전"` 또는 `"📺 리롤 +3"` 또는 원하는 텍스트
   - **Font Size**: 14~18 (버튼 크기에 맞게 조정)
   - **Alignment**: Center 정렬
   - **Color**: 흰색 또는 잘 보이는 색상

#### 2.5. 버튼 스타일링 (선택사항)

1. `RewardedAdButton` GameObject 선택
2. **Image** 컴포넌트 설정:
   - **Color**: 노란색 또는 금색 (예: `#FFD700`) - 광고/보상 느낌
   - **Source Image**: 기본 또는 커스텀 버튼 스프라이트

**추천 색상**:
- 밝은 노란색: `R:255, G:215, B:0` (Gold)
- 밝은 초록색: `R:0, G:200, B:0` (보상 느낌)

#### 2.6. ✨ CanvasGroup 컴포넌트 추가 (최적화 핵심)

**이 단계가 가장 중요합니다!**

1. **RewardedAdButton GameObject** 선택
2. **Inspector** 패널에서 **Add Component** 클릭
3. 검색창에 `Canvas Group` 입력
4. **Canvas Group** 컴포넌트 선택하여 추가
5. CanvasGroup 컴포넌트의 초기 상태 설정:
   - **Alpha**: `0` (완전 투명 - 숨김 상태)
   - **Interactable**: 체크 해제 (클릭 불가)
   - **Block Raycasts**: 체크 해제 (레이캐스트 차단 안함)
   - **Ignore Parent Groups**: 체크 (선택사항, 부모 CanvasGroup 영향 받지 않음)

**확인**: CanvasGroup 컴포넌트가 추가되고 Alpha=0, Interactable=false, Block Raycasts=false로 설정되었는지 확인

#### 2.7. RerollButton 스크립트 연결 (CanvasGroup)

**중요**: GameObject가 아닌 **CanvasGroup 컴포넌트**를 연결합니다!

1. **RerollButton GameObject** 선택 (리롤 버튼 오브젝트)
2. **Inspector**에서 **RerollButton (Script)** 컴포넌트 찾기
3. **Rewarded Ad Button Canvas Group** 필드 찾기
4. **Hierarchy에서 `RewardedAdButton` GameObject**를 이 필드에 **드래그 앤 드롭**
   - Unity가 자동으로 RewardedAdButton의 CanvasGroup 컴포넌트를 연결합니다
   - 또는 RewardedAdButton의 CanvasGroup 컴포넌트를 직접 드래그해도 됩니다

**확인**:
- 필드에 `RewardedAdButton (CanvasGroup)` 으로 표시되는지 확인
- GameObject 아이콘이 아닌 CanvasGroup 컴포넌트 아이콘으로 표시되어야 함

#### 2.8. 버튼 onClick 이벤트 설정

1. **RewardedAdButton GameObject** 선택
2. **Inspector**에서 **Button** 컴포넌트 찾기
3. **On Click ()** 이벤트 섹션에서 **+** 버튼 클릭
4. **None (Object)** 필드에 **RerollButton GameObject**를 드래그
5. 함수 선택 드롭다운:
   - **RerollButton** → **WatchAdForReroll ()**

**확인**:
- On Click() 이벤트가 `RerollButton.WatchAdForReroll` 으로 설정되었는지 확인
- Runtime Only로 설정되어 있는지 확인

#### 2.9. 최종 확인

**CanvasGroup 설정 재확인** (매우 중요!):
1. **RewardedAdButton GameObject** 선택
2. **Inspector**에서 CanvasGroup 컴포넌트 확인:
   - Alpha = **0** (투명)
   - Interactable = **체크 해제** (false)
   - Block Raycasts = **체크 해제** (false)
3. **GameObject는 활성화 상태 유지** (체크박스가 켜져있어야 함)
   - ❌ GameObject를 비활성화하면 안 됩니다!
   - ✅ CanvasGroup의 Alpha=0으로 숨김 처리

**RerollButton 연결 확인**:
- RerollButton 컴포넌트의 **Rewarded Ad Button Canvas Group** 필드에 CanvasGroup이 올바르게 연결되었는지 확인

#### 2.10. 씬 저장
- **Ctrl + S** 또는 **File → Save** 로 씬 저장

---

## 3. 설정 확인 체크리스트

### GameplayAdTimer
- [ ] MainScene에 `GameplayAdTimer` GameObject 존재
- [ ] `GameplayAdTimer.cs` 컴포넌트 추가됨
- [ ] 씬 저장 완료

### RewardedAdButton (CanvasGroup 최적화)
- [ ] Canvas의 독립적인 오브젝트로 `RewardedAdButton` GameObject 존재 (RerollButton의 형제)
- [ ] 버튼 텍스트가 "광고 보고 충전" 등으로 설정됨
- [ ] 버튼 위치와 크기가 적절히 조정됨
- [ ] ✨ **CanvasGroup 컴포넌트 추가됨** (핵심!)
- [ ] CanvasGroup 초기 상태: Alpha=0, Interactable=false, Block Raycasts=false
- [ ] GameObject는 **활성화 상태 유지** (비활성화 안 됨)
- [ ] RerollButton 스크립트의 `Rewarded Ad Button Canvas Group` 필드에 CanvasGroup 연결됨
- [ ] RewardedAdButton의 onClick 이벤트가 `RerollButton.WatchAdForReroll()`로 설정됨
- [ ] 씬 저장 완료

---

## 4. 테스트 준비

### AdMob 테스트 모드 설정

1. **Project** 패널에서 `Assets/Scripts/AdMob/AdMobConfig` ScriptableObject 찾기
2. **Inspector**에서 **Use Test Ids** 체크박스 **체크** (테스트 광고 사용)
3. 실제 배포 전에는 반드시 **체크 해제** 필요

### 테스트 시나리오 1: 5분 타이머 전면광고

1. 게임 실행
2. Unity Console에서 다음 명령어 실행 (5분 경과 시뮬레이션):
   ```csharp
   PlayerPrefs.SetString("LastAdShowTime", System.DateTime.Now.AddMinutes(-6).ToString());
   ```
3. 게임오버 후 **다시시작** 버튼 클릭
4. **예상 결과**: 전면광고 표시

### 테스트 시나리오 2: 보상형 광고 리롤 충전

1. 게임 실행
2. 리롤 버튼을 3회 클릭하여 리롤 횟수 소진
3. **예상 결과**: `RewardedAdButton` 자동으로 표시됨
4. `RewardedAdButton` 클릭
5. **예상 결과**: 보상형 광고 표시
6. 광고 시청 완료
7. **예상 결과**:
   - 리롤 횟수 3 증가 (0 → 3)
   - 버튼 자동으로 숨김
   - Select 사운드 재생

### 테스트 시나리오 3: 광고 시청 제한 (세션당 3회)

**3-1. 광고 3회 시청 후 버튼 클릭 차단**:
1. 위 시나리오 2를 3회 반복하여 광고 시청 횟수 소진
2. 리롤을 다시 3회 사용하여 리롤 횟수 0으로 만들기
3. **예상 결과**: RewardedAdButton이 **나타나지 않음** ✨ (광고 기회 소진)
4. 리롤 버튼 클릭 시 **예상 결과**: Forbid 사운드만 재생

**3-2. 광고 기회가 남아있을 때 버튼 클릭**:
1. 광고 2회만 시청한 상태에서 리롤 횟수를 0으로 만들기
2. **예상 결과**: RewardedAdButton이 표시됨 (1회 기회 남음)
3. RewardedAdButton 클릭
4. **예상 결과**: 보상형 광고 표시 (3번째 광고)
5. 광고 시청 완료 후 리롤 다시 3회 사용
6. **예상 결과**: RewardedAdButton이 **나타나지 않음** (3회 모두 소진)

---

## 5. Hierarchy 구조 예시

```
Canvas
├── (기존 UI 요소들...)
├── RerollButton                        ← RerollButton.cs 컴포넌트
│   └── Text (TMP)                      (리롤 횟수 표시)
├── RewardedAdButton                    ← 독립적인 오브젝트 (RerollButton의 형제)
│   ├── [Button 컴포넌트]               onClick → RerollButton.WatchAdForReroll()
│   ├── [CanvasGroup 컴포넌트] ✨       Alpha=0, Interactable=false, BlockRaycasts=false
│   └── Text (TMP)                      ("광고 보고 충전")
└── (기타 UI 요소들...)

(루트 레벨)
├── GameplayAdTimer                     ← GameplayAdTimer.cs 컴포넌트
└── (기타 매니저들...)
```

**핵심 포인트**:
- RewardedAdButton은 RerollButton의 **자식이 아닌 형제** (같은 레벨)
- GameObject는 **항상 활성화** 상태 유지
- **CanvasGroup 컴포넌트**로 표시/숨김 제어 (최적화)

---

## 6. 문제 해결

### 광고가 표시되지 않을 때

1. **AdMobConfig 확인**:
   - `Use Test Ids`가 체크되어 있는지 확인
   - 광고 단위 ID가 올바른지 확인

2. **AdsManager 초기화 확인**:
   - Console에서 AdMob 초기화 로그 확인
   - AdsManager GameObject가 씬에 존재하는지 확인

3. **인터넷 연결 확인**:
   - 실제 디바이스가 인터넷에 연결되어 있는지 확인
   - Unity Editor에서는 광고가 표시되지 않음 (빌드 필요)

### RewardedAdButton이 나타나지 않을 때

1. **CanvasGroup 참조 연결 확인** ✨:
   - RerollButton 컴포넌트의 `Rewarded Ad Button Canvas Group` 필드가 비어있지 않은지 확인
   - 필드에 `RewardedAdButton (CanvasGroup)` 으로 표시되는지 확인

2. **CanvasGroup 컴포넌트 확인**:
   - RewardedAdButton GameObject에 CanvasGroup 컴포넌트가 추가되었는지 확인
   - CanvasGroup이 없으면 버튼이 표시/숨김 제어가 안 됨

3. **리롤 횟수 확인**:
   - 리롤 횟수가 정확히 0인지 확인
   - Console에서 리롤 횟수 로그 확인
   - 리롤 횟수가 0일 때 CanvasGroup의 Alpha가 1로 변경되는지 확인

4. **GameObject 활성화 상태 확인**:
   - RewardedAdButton GameObject가 **활성화** 상태인지 확인
   - GameObject를 비활성화하면 CanvasGroup이 작동하지 않음
   - 초기 상태: GameObject 활성화 + CanvasGroup Alpha=0

5. **CanvasGroup 초기 값 확인**:
   - Alpha = 0 (투명)
   - Interactable = false
   - Block Raycasts = false
   - 위 값들이 올바르게 설정되었는지 확인

### onClick 이벤트가 작동하지 않을 때

1. **이벤트 설정 확인**:
   - RewardedAdButton의 Button 컴포넌트 확인
   - On Click() 이벤트에 `RerollButton.WatchAdForReroll`이 올바르게 설정되었는지 확인

2. **참조 오브젝트 확인**:
   - On Click() 이벤트의 오브젝트 필드에 RerollButton GameObject가 연결되었는지 확인

---

## 7. 추가 개선 사항 (선택)

### UI 애니메이션 추가
- RewardedAdButton에 펄스 애니메이션 추가 (주목도 향상)
- DOTween을 사용하여 버튼 크기 변화 애니메이션

### 시각적 피드백
- 광고 시청 후 리롤 충전 시 파티클 효과
- 리롤 횟수 증가 시 숫자 애니메이션

### 타이머 UI 표시
- 다음 광고까지 남은 시간 표시 (예: "다음 광고까지 2:30")
- `GameplayAdTimer.Instance.GetRemainingSeconds()` 활용

---

## 완료!

모든 설정이 완료되었습니다. 게임을 빌드하여 실제 디바이스에서 테스트하세요.

**주의**: Unity Editor에서는 광고가 표시되지 않습니다. 반드시 Android/iOS 디바이스에 빌드하여 테스트해야 합니다.
