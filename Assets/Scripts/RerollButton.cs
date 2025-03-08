using UnityEngine;
using UnityEngine.UI;

public class RerollButton : MonoBehaviour
{
    [SerializeField]
    private TileGenerator tileGenerator;
    //[SerializeField] private Toggle rerollToggle;
    //[SerializeField] private Image rerollButtonBackground;
    private int rerollCount;

    private void Awake()
    {
        rerollCount = 3;
    }

    public void Reroll()
    {
        if (rerollCount == 0)
        {
            SoundManager.Instance.PlayForbidSound();
        }
        else
        {
            SoundManager.Instance.PlayDisplaySound();
            rerollCount--;

            tileGenerator.Reroll();

            /*
            if (rerollCount == 0)
            {
                rerollButtonBackground.color = Color.red;
            }
            */
        }
    }
}
