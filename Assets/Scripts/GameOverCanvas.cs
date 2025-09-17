using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text prevGoalText;
    [SerializeField] private TMP_Text goalText;
    [SerializeField] private Image scoreGaugeFill;

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
    }

    public void ActivateGameOverUI()
    {
        prevGoalText.text = $"{TurnCounting.Instance.prevGoalScore}";
        goalText.text = $"{TurnCounting.Instance.goalScore}";
        _canvas.enabled = true;

        StartCoroutine(StartScoreEffectCo());
    }

    private IEnumerator StartScoreEffectCo()
    {
        // 목표값 계산
        float min = TurnCounting.Instance.prevGoalScore;
        float max = TurnCounting.Instance.goalScore;
        float targetRatio = Mathf.InverseLerp(min, max, ScoreManager.score);

        // 초기화
        scoreGaugeFill.fillAmount = 0;
        totalScoreText.text = "0";

        // 애니메이션 실행
        float duration = 0.5f;
        float elapsed = 0f;
        int finalScore = ScoreManager.score;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 게이지 증가
            scoreGaugeFill.fillAmount = Mathf.Lerp(0f, targetRatio, t);

            // 점수 카운트업
            int displayScore = Mathf.FloorToInt(Mathf.Lerp(0, finalScore, t));
            totalScoreText.text = displayScore.ToString();

            yield return null;
        }

        // 마지막 값 보정
        scoreGaugeFill.fillAmount = targetRatio;
        totalScoreText.text = finalScore.ToString();

        SoundManager.Instance.PlayGameOverSound();
    }
}
