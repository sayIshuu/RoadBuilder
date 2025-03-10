using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnCounting : MonoBehaviour
{
    //싱글톤패턴
    public static TurnCounting Instance;
    public RerollButton rerollObject;
    [SerializeField]
    LevelUpEffect levelUpEffect;
    public int turnCount;
    public int limitTurn;
    private int firstLimitTurn;
    public int goalScore;
    private int firstGoalScore;
    private int increaseMultiplier = 2;

    private int level = 1;

    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI limitTurnText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI goalScoreText;

    private void Awake()
    {
        // 싱글톤 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경 시 삭제되지 않도록 설정
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }

        firstLimitTurn = limitTurn;
        firstGoalScore = goalScore;
        UpdateText();
    }

    // 씬이 로드될 때 변수 초기화
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIElements(); // 텍스트누락방지 요소 할당
        ResetVariables(); // 변수를 기본값으로 초기화
        UpdateText();
    }

    // 변수 초기화 메서드
    private void ResetVariables()
    {
        turnCount = 0;
        level = 1;
        limitTurn = firstLimitTurn;
        goalScore = firstGoalScore;
        increaseMultiplier = 2;
    }

    private void AssignUIElements()
    {
        limitTurnText = GameObject.Find("TurnCountText")?.GetComponent<TextMeshProUGUI>();
        goalScoreText = GameObject.Find("GoalScoreCountText")?.GetComponent<TextMeshProUGUI>();
    }

    public void CheckTrunAndGoal()
    {
        UpdateText();

        if(turnCount%100 == 0 && turnCount != 0)
        {
            rerollObject.PlusRerollCount();
        }

        if(turnCount == limitTurn-1)
        {
            if(ScoreManager.score < goalScore)
            {
                //FF3B3B
                limitTurnText.color = new Color32(255, 59, 59, 255);
                turnText.color = new Color32(255, 59, 59, 255);
                goalText.color = new Color32(255, 59, 59, 255);
                goalScoreText.color = new Color32(255, 59, 59, 255);
            }
            else
            {
                //8DFF9F
                limitTurnText.color = new Color32(141, 255, 159, 255);
                turnText.color = new Color32(141, 255, 159, 255);
                goalText.color = new Color32(141, 255, 159, 255);
                goalScoreText.color = new Color32(141, 255, 159, 255);
            }
        }

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
                //갱신
                limitTurn += firstLimitTurn;
                
                if(goalScore >= 30000)
                {
                    goalScore += 5000;
                }
                else if(goalScore >= 25000)
                {
                    goalScore = 30000;
                }
                else
                {
                    goalScore += firstGoalScore * increaseMultiplier;
                    increaseMultiplier += 1;
                }

                levelUpEffect.CrackerShoot(level);
                level++;
                SoundManager.Instance.PlayLevelUpSound();
            }
        }
    }

    //텍스트 갱신
    private void UpdateText()
    {
        limitTurnText.text = turnCount.ToString();
        goalScoreText.text = goalScore.ToString();
    }
}
