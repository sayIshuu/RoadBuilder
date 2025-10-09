using UnityEngine;
using DG.Tweening;

public class TutorialBlinkingEffect : MonoBehaviour
{
    [SerializeField] float fadeTime = 0.6f;
    [SerializeField] float minAlpha = 0.3f;
    [SerializeField] float maxAlpha = 1f;

    private GameObject _tagetObj;
    private CanvasGroup _canvasGroup;
    private Tween _tween;

    public void StartBlinking(GameObject targetObj)
    {
        _tagetObj = targetObj;
        _canvasGroup = targetObj.AddComponent<CanvasGroup>();

        _tween = _canvasGroup.DOFade(minAlpha, fadeTime)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopBlinking()
    {
        _canvasGroup.alpha = maxAlpha;

        _tween?.Kill();
        _tween = null;

        Destroy(_tagetObj.GetComponent<CanvasGroup>());
    }
}
