using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RerollButton : MonoBehaviour
{
    [SerializeField] private TileGenerator tileGenerator;
    //[SerializeField] private Toggle rerollToggle;
    //[SerializeField] private Image rerollButtonBackground;
    private int rerollCount;
    [SerializeField] private TextMeshProUGUI rerollCountText;

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

            /*
            if (rerollCount == 0)
            {
                rerollButtonBackground.color = Color.red;
            }
            */
        }
        rerollCountText.text = rerollCount.ToString();
    }

    public void PlusRerollCount()
    {
        rerollCount++;
        rerollCountText.text = rerollCount.ToString();
    }
}
