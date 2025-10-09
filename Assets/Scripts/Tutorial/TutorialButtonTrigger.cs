using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButtonTrigger : TutorialBase
{
    [SerializeField] private Button targetBtn;
    private TutorialController _controller;

    [SerializeField] private string dialog;
    [SerializeField] private string subDialog;

    [SerializeField] private GameObject targetObj;
    private Transform _targetObjParent;

    [SerializeField] private bool isBlinking;
    private TutorialBlinkingEffect _blinkingEffect;

    private Image tutorialBackgroundImg;
    private TMP_Text dialogueText;
    private TMP_Text tutorialSubText;

    private void Awake()
    {
        tutorialBackgroundImg = FindAnyObjectByType<TutorialBackgroundImg>().GetComponent<Image>();
        dialogueText = FindAnyObjectByType<TutorialText>().GetComponent<TMP_Text>();
        tutorialSubText =  FindAnyObjectByType<TutorialSubText>().GetComponent<TMP_Text>();
    }

    private void Start()
    {
        targetBtn.interactable = false;
    }

    public override void Enter(TutorialController controller)
    {
        _controller = controller;
        targetBtn.interactable = true;
        targetBtn.onClick.AddListener(OnClickTargetBtn);

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

        dialogueText.text = dialog;
        tutorialSubText.text = subDialog;

        tutorialBackgroundImg.gameObject.SetActive(true);
    }

    public override void Execute(TutorialController controller)
    {
    }

    public override void Exit()
    {
        if (targetObj != null)
        {
            Destroy(targetObj.GetComponent<TutorialBlinkingEffect>());
            targetObj.transform.SetParent(_targetObjParent);
        }

        if (isBlinking && targetObj != null)
        {
            _blinkingEffect.StopBlinking();
        }

        tutorialBackgroundImg.gameObject.SetActive(false);

        targetBtn.onClick.RemoveListener(OnClickTargetBtn);
    }

    private void OnClickTargetBtn()
    {
        _controller.SetNextTutorial();
    }
}
