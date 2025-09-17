using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnCounting : MonoBehaviour
{
    //�̱�������
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

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ���� �� �������� �ʵ��� ����
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }

        firstLimitTurn = limitTurn;
        firstGoalScore = goalScore;
        UpdateText();
    }

    // ���� �ε�� �� ���� �ʱ�ȭ
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIElements(); // �ؽ�Ʈ�������� ��� �Ҵ�
        ResetVariables(); // ������ �⺻������ �ʱ�ȭ
        UpdateText();
    }

    // ���� �ʱ�ȭ �޼���
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
    }
}
