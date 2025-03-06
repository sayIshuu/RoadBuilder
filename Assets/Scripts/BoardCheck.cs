using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class BoardCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject[] boardSlot;
    [SerializeField]
    private TextMeshProUGUI scoreTxt;
    [SerializeField]
    private TextMeshProUGUI gameOverTxt;
    public static int score = 0;
    public static bool gameover = false;
    public int displayedTileCount = 0;
    private int[] uf = new int[49];

    public static int[,] adj = new int[7, 7];

    private void Awake()
    {
        adj = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };
        gameover = false;
        score = 0;
        scoreTxt.text = "Score : " + score;
        GameObject boardInventory = GameObject.Find("BoardInventory");
        for (int i = 0; i < 25; i++)
        {
            boardSlot[i] = boardInventory.transform.GetChild(i).gameObject;
        }
    }

    public void CheckEx()
    {
        uf = new int[49] { 0, 0, 0, 0, 0, 0, 0, 0, 8, 9, 10, 11, 12, 0, 0, 15, 16, 17, 18, 19, 0, 0, 22, 23, 24, 25, 26, 0, 0, 29, 30, 31, 32, 33, 0, 0, 36, 37, 38, 39, 40, 0, 0, 0, 0, 0, 0, 0, 0 };
        bool isCycle = false;

        // 연결 여부 확인
        for (int i = 1; i <= 5; i++)
        {
            for (int j = 1; j <= 5; j++)
            {
                if ((adj[i, j] & 1) > 0 && (adj[i - 1, j] & 4) > 0) // 도로와 위쪽이 이어져 있음
                {
                    if (UfFind(7 * i + j) == 0 && UfFind(7 * i + j - 7) == 0) isCycle = true;
                    else UfMerge(7 * i + j, 7 * i + j - 7);
                }
                if (j == 5 && (adj[i, j] & 2) > 0 && (adj[i, j + 1] & 8) > 0) // 도로와 오른쪽이 이어져 있음 (맨 오른쪽 타일에서만 확인)
                {
                    if (UfFind(7 * i + j) == 0 && UfFind(7 * i + j + 1) == 0) isCycle = true;
                    else UfMerge(7 * i + j, 7 * i + j + 1);
                }
                if (i == 5 && (adj[i, j] & 4) > 0 && (adj[i + 1, j] & 1) > 0) // 도로와 아래쪽이 이어져 있음 (맨 아래쪽 타일에서만 확인)
                {
                    if (UfFind(7 * i + j) == 0 && UfFind(7 * i + j + 7) == 0) isCycle = true;
                    else UfMerge(7 * i + j, 7 * i + j + 7);
                }
                if ((adj[i, j] & 8) > 0 && (adj[i, j - 1] & 2) > 0) // 도로와 왼쪽이 이어져 있음
                {
                    if (UfFind(7 * i + j) == 0 && UfFind(7 * i + j - 1) == 0) isCycle = true;
                    else UfMerge(7 * i + j, 7 * i + j - 1);
                }
            }
        }

        if (isCycle)
        {
            GetScoreEx();
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
            gameOverTxt.text = "Your Score is " + score;
        }

        scoreTxt.text = "Score : " + score;
    }

    private void UfMerge(int a, int b)
    {
        a = UfFind(a);
        b = UfFind(b);

        if (a > b)
        {
            uf[a] = b;
        }
        else if (a < b)
        {
            uf[b] = a;
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

    private void GetScoreEx()
    {
        int len = 0;

        for(int i = 1; i <= 5; i++)
        {
            for(int j = 1; j <= 5; j++)
            {
                if (uf[7 * i + j] == 0)
                {
                    DestroyTile(i, j);
                    len++;
                }
            }
        }

        // 점수 계산 : 배율 정해서. 이부분은 쉽게 수정되게. 배율변수 빼기.
        displayedTileCount -= len;
        score += len * len * len;
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
