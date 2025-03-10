using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    [SerializeField] private TextMeshProUGUI scoreTxt;

    private int bestScore = 0;
    //[SerializeField] private TextMeshProUGUI bestScoreTxt;

    [SerializeField] private TextMeshProUGUI bestScoreTxt2;

    private void Awake()
    {
        score = 0;
        scoreTxt.text = score.ToString();
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        //bestScoreTxt.text = bestScore.ToString();
        bestScoreTxt2.text = bestScore.ToString();
    }

    public void AddScore(int value)
    {
        score += value;
        scoreTxt.text = score.ToString();
        saveBestScore();
    }

    public void saveBestScore()
    {
        if(score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
        //bestScoreTxt.text = bestScore.ToString();
        bestScoreTxt2.text = bestScore.ToString();
    }
}
