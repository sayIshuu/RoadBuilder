using TMPro;
using UnityEngine;

public class TutorialRerollButton : MonoBehaviour
{
    [SerializeField] private TutorialTileGenerator tileGenerator;

    private int rerollCount;
    [SerializeField] private TMP_Text rerollCountText;

    private void Awake()
    {
        rerollCount = 3;
        rerollCountText.text = rerollCount.ToString();
    }

    public void Reroll()
    {
        if (rerollCount == 0)
        {
            SoundManager.Instance.PlayForbidSound();
        }
        else
        {
            SoundManager.Instance.PlaySelectSound();
            rerollCount--;

            tileGenerator.Reroll();
        }

        rerollCountText.text = rerollCount.ToString();
    }
}
