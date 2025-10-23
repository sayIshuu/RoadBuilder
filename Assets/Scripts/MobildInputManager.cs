using System.Collections;
using UnityEngine;

public class MobildInputManager : MonoBehaviour
{
    [SerializeField] private Canvas toast;     // "한번 더 클릭 시 종료됩니다." 팝업(패널)
    [SerializeField] private float timeout = 2f;   // 두 번째 입력 유효 시간(초)

    private float lastPressTime = -999f;
    private Coroutine hideToastCo;

    private void Awake()
    {
        // 뒤로가기로 앱이 바로 종료되지 않게 방지
        Input.backButtonLeavesApp = false;

        if (toast != null) toast.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            float now = Time.unscaledTime;

            if (now - lastPressTime <= timeout)
            {
                // 두 번째 입력: 종료
#if UNITY_EDITOR
                // 유니티 에디터에서 플레이 모드를 종료할 때 사용
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // 빌드된 애플리케이션(PC, 모바일 등)을 종료할 때 사용
                Application.Quit();
#endif
                return;
            }

            // 첫 번째 입력: 토스트 노출 + 타이머 시작
            lastPressTime = now;
            ShowToast();
        }
    }

    private void ShowToast()
    {
        if (toast == null) return;

        toast.enabled = true;

        if (hideToastCo != null) StopCoroutine(hideToastCo);
        hideToastCo = StartCoroutine(HideToastAfter(timeout));
    }

    private IEnumerator HideToastAfter(float sec)
    {
        yield return new WaitForSecondsRealtime(sec);
        if (toast != null) toast.enabled = false;
    }
}
