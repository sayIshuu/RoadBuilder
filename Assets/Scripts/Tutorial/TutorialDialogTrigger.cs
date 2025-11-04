using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDialogTrigger : TutorialBase
{
    [SerializeField] private string dialog;
    [SerializeField] private string subDialog;

    [SerializeField] private GameObject targetObj;
    private Transform _targetObjParent;

    [SerializeField] private bool isBlinking;
    private TutorialBlinkingEffect _blinkingEffect;

    private Image tutorialBackgroundImg;
    private TMP_Text tutorialText;
    private TMP_Text tutorialSubText;

    private float _clickDelay = 1f; // 클릭 딜레이
    private float _enterTime;

    private void Awake()
    {
        tutorialBackgroundImg = FindAnyObjectByType<TutorialBackgroundImg>().GetComponent<Image>();
        tutorialText = FindAnyObjectByType<TutorialText>().GetComponent<TMP_Text>();
        tutorialSubText =  FindAnyObjectByType<TutorialSubText>().GetComponent<TMP_Text>();
    }

    public override void Enter(TutorialController controller)
    {
        if (targetObj != null)
        {
            _targetObjParent = targetObj.transform.parent;
            targetObj.transform.SetParent(tutorialBackgroundImg.gameObject.transform);
        }

        if (isBlinking && targetObj != null)
        {
            _blinkingEffect = gameObject.AddComponent<TutorialBlinkingEffect>();
            _blinkingEffect.StartBlinking(targetObj);
        }

        tutorialText.text = dialog;
        tutorialSubText.text = subDialog;

        tutorialBackgroundImg.gameObject.SetActive(true);
        _enterTime = Time.time;
    }

    public override void Execute(TutorialController controller)
    {
        if (Time.time - _enterTime < _clickDelay) return;
            
        if (Input.GetMouseButtonDown(0))
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        if (targetObj != null)
        {
            targetObj.transform.SetParent(_targetObjParent);
        }

        if (isBlinking && targetObj != null)
        {
            _blinkingEffect.StopBlinking();
        }

        tutorialBackgroundImg.gameObject.SetActive(false);
    }
}
