using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDialogTrigger : TutorialBase
{
    [SerializeField] private string dialog;
    [SerializeField] private GameObject targetObj;
    private Transform _targetObjParent;

    private Image tutorialBackgroundImg;
    private TMP_Text dialogueText;

    private void Awake()
    {
        tutorialBackgroundImg = FindAnyObjectByType<TutorialBackgroundImg>().GetComponent<Image>();
        dialogueText = FindAnyObjectByType<TutorialText>().GetComponent<TMP_Text>();
    }

    public override void Enter(TutorialController controller)
    {
        if (targetObj != null)
        {
            _targetObjParent = targetObj.transform.parent;
            targetObj.transform.SetParent(tutorialBackgroundImg.gameObject.transform);
        }

        dialogueText.text = dialog;
        tutorialBackgroundImg.gameObject.SetActive(true);
    }

    public override void Execute(TutorialController controller)
    {
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

        tutorialBackgroundImg.gameObject.SetActive(false);
    }
}
