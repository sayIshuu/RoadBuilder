using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnCounting : MonoBehaviour
{
    public static TurnCounting Instance;
    public RerollButton rerollObject;
    [SerializeField]
    LevelUpEffect levelUpEffect;
    public int turnCount;
    public int limitTurn;
    private int firstLimitTurn;
    public int goalScore;
    public int prevGoalScore = 0;
    private int firstGoalScore;
    private int increaseMultiplier = 2;

    private int level = 1;

    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI limitTurnText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI goalScoreText;

    [SerializeField] private TextMeshProUGUI tenTurnText;
    //[SerializeField] private TextMeshProUGUI scoreText;

    private Color32 redColor = new Color32(255, 59, 59, 255);
    private Color32 orangeColor = new Color32(255, 255, 0, 255);
    private Color32 greenColor = new Color32(141, 255, 159, 255);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        firstLimitTurn = limitTurn;
        firstGoalScore = goalScore;
        UpdateText();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIElements();
        ResetVariables();
        UpdateText();
    }

    private void ResetVariables()
    {
        turnCount = 0;
        level = 1;
        limitTurn = firstLimitTurn;
        goalScore = firstGoalScore;
        prevGoalScore = 0;
        increaseMultiplier = 2;
    }

    private void AssignUIElements()
    {
        limitTurnText = GameObject.Find("TurnCountText")?.GetComponent<TextMeshProUGUI>();
        goalScoreText = GameObject.Find("GoalScoreCountText")?.GetComponent<TextMeshProUGUI>();
    }

    public void UpdateUI()
    {
        UpdateText();
        UpdateGoalWarningColor();
    }

    public void CheckTurnAndGoal()
    {
        UpdateText();

        if(turnCount%100 == 0 && turnCount != 0)
        {
            rerollObject.PlusRerollCount();
        }

        UpdateGoalWarningColor();

        if (turnCount >= limitTurn)
        {
            turnText.color = Color.white;
            goalText.color = Color.white;
            limitTurnText.color = Color.white;
            goalScoreText.color = Color.white;
            if (ScoreManager.score < goalScore)
            {
                //game over
                BoardCheck.gameover = true;
            }
            else
            {
                if(increaseMultiplier == 5)
                {
                    firstGoalScore *= 2;
                }
                else if (increaseMultiplier == 10)
                {
                    firstGoalScore *= 2;
                    increaseMultiplier = 6;
                }
                //����
                limitTurn += firstLimitTurn;

                if(goalScore >= 30000)
                {
                    prevGoalScore = goalScore;
                    goalScore += 5000;
                }
                else if(goalScore >= 25000)
                {
                    prevGoalScore = goalScore;
                    goalScore = 30000;
                }
                else
                {
                    prevGoalScore = goalScore;
                    goalScore += firstGoalScore * increaseMultiplier;
                    increaseMultiplier += 1;
                }

                levelUpEffect.CrackerShoot(level);
                level++;
                SoundManager.Instance.PlayLevelUpSound();
            }
        }
    }

    //�ؽ�Ʈ ����
    private void UpdateText()
    {
        limitTurnText.text = turnCount.ToString();
        goalScoreText.text = goalScore.ToString();
        float next = Mathf.Ceil(turnCount / 10f) * 10;
        tenTurnText.text = (next == 0 ? 10 : next).ToString();
    }

    private void UpdateGoalWarningColor()
    {
        Color target;
        // 목표 미달성 시 경고 색상
        if (ScoreManager.score < goalScore)
        {
            //const int warnWindow = 10;
            //int startWarnTurn = Mathf.Max(0, limitTurn - warnWindow);
            //int endWarnTurn   = Mathf.Max(0, limitTurn - 1);

            //if (turnCount >= startWarnTurn && turnCount <= endWarnTurn)
            //{
            //    float t = Mathf.InverseLerp(startWarnTurn, endWarnTurn, turnCount);

            //    if (t < 0.5f)
            //    {
            //        float subT = t / 0.5f;
            //        target = Color.Lerp(Color.white, orangeColor, subT);
            //    }
            //    else
            //    {
            //        float subT = (t - 0.5f) / 0.5f;
            //        target = Color.Lerp(orangeColor, redColor, subT);
            //    }
            //}

            target = redColor;
        }
        else
        {
            target = greenColor;
        }

        limitTurnText.color = target;
        //turnText.color = target;
        //goalText.color = target;
        goalScoreText.color = target;
    }
}
