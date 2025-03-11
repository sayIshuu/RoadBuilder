using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System;

public class BoardCheck : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField]
    private MissionController missionController;
    [SerializeField]
    private GameObject[] boardSlot;
    [SerializeField]
    private TextMeshProUGUI gameOverTxt;
    //public static int score = 0;
    private ScoreManager scoreManager;
    public static bool gameover = false;
    public int displayedTileCount = 0;
    private int[] uf = new int[49];

    private int[] checkNum = new int[] { 1, 2, 3, 4, 5, 7, 13, 14, 20, 21, 27, 28, 34, 35, 41, 43, 44, 45, 46, 47 };

    public static int[,] adj = new int[7, 7];

    private void Awake()
    {
        adj = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };
        gameover = false;
        GameObject boardInventory = GameObject.Find("BoardInventory");
        scoreManager = FindAnyObjectByType<ScoreManager>();
        for (int i = 0; i < 25; i++)
        {
            boardSlot[i] = boardInventory.transform.GetChild(i).gameObject;
        }
        gameOverPanel.SetActive(false);
    }

    public void Check()
    {
        for (int i = 0; i < 49; i++) uf[i] = i;
        int cycleCheck = 0;

        // 연결하기
        for (int i = 1; i <= 5; i++)
        {
            for (int j = 1; j <= 5; j++)
            {
                if ((adj[i, j] & 1) > 0 && (adj[i - 1, j] & 4) > 0) // 도로와 위쪽이 이어져 있는지
                {
                    UfMerge(7 * i + j, 7 * i + j - 7);
                }
                if ((adj[i, j] & 2) > 0 && (adj[i, j + 1] & 8) > 0) // 도로와 오른쪽이 이어져 있는지
                {
                    UfMerge(7 * i + j, 7 * i + j + 1);
                }
                if ((adj[i, j] & 4) > 0 && (adj[i + 1, j] & 1) > 0) // 도로와 아래쪽이 이어져 있는지
                {
                    UfMerge(7 * i + j, 7 * i + j + 7);
                }
                if ((adj[i, j] & 8) > 0 && (adj[i, j - 1] & 2) > 0) // 도로와 왼쪽이 이어져 있는지
                {
                    UfMerge(7 * i + j, 7 * i + j - 1);
                }
            }
        }

        // 연결 확인
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                if (i > 0 && i < 6 && j > 0 && j < 6) continue;

                if (uf[7 * i + j] != 7 * i + j)
                {
                    cycleCheck = uf[7 * i + j];
                }
            }
        }

        if (cycleCheck > 0)
        {
            GetScore(cycleCheck);
        }

        //턴 증가
        TurnCounting.Instance.turnCount++;
        //턴에 해당하는 점수 충족 여부 확인 및 게임 종료 결정
        TurnCounting.Instance.CheckTrunAndGoal();

        if (displayedTileCount >= 25) // gameover
        {
            gameover = true;
        }
        if(gameover)
        {
            SoundManager.Instance.PlayGameOverSound();
            gameOverTxt.gameObject.SetActive(true);
            gameOverTxt.text = "Your Score is " + ScoreManager.score;
            gameOverPanel.SetActive(true);
        }

        //scoreTxt.text = score.ToString();
    }

    private void UfMerge(int a, int b)
    {
        a = UfFind(a);
        b = UfFind(b);

        if (Array.Exists(checkNum, x => x == a))
        {
            uf[b] = a;
        }
        else
        {
            uf[a] = b;
        }
    }

    private int UfFind(int x)
    {
        if (uf[x] == x)
        {
            return x;
        }
        else
        {
            uf[x] = UfFind(uf[x]);
            return uf[x];
            //return UfFind(uf[x]);
        }
    }

    private void GetScore(int num)
    {
        int len = 0;

        for(int i = 1; i <= 5; i++)
        {
            for(int j = 1; j <= 5; j++)
            {
                if (UfFind(uf[7 * i + j]) == num)
                {
                    DestroyTile(i, j);
                    len++;
                }
            }
        }

        if(len > 11) SoundManager.Instance.PlayLargeScoreSound();
        else SoundManager.Instance.PlayScoreSound();

        // 점수 계산 : 배율 정해서. 이부분은 쉽게 수정되게. 배율변수 빼기.
        displayedTileCount -= len;
        scoreManager.AddScore(len * len * len);
        missionController.CheckAllLengthMission(len);
    }

    private void DestroyTile(int y, int x)
    {
        adj[y, x] = 0;
        if(boardSlot[5 * y + x - 6].transform.childCount > 0)
        {
            boardSlot[5 * y + x - 6].transform.GetChild(0).GetComponent<TileDestroy>().StartBreak();
        }
    }
}
