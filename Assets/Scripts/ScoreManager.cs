using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    public int BestScore = 0;
    [SerializeField] private TextMeshProUGUI scoreTxt;

    private void Awake()
    {
        score = 0;
        scoreTxt.text = score.ToString();
    }

    public void AddScore(int value)
    {
        score += value;
        scoreTxt.text = score.ToString();
    }

    public void checkBestScore(int value)
    {
        if (BestScore < value)
        {
            BestScore = value;
        }
    }
}
