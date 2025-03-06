using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;

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
    //dfs추적을 위한 list. 각 int값으로 arr의 인덱스값이 들어갑니다.
    private List<(int, int)> path = new List<(int, int)>();
    //어떤 아이템을 그 칸에 들어있는지 나타내는 배열. 0 : 빈칸, 1 : 리롤+1, 2 : 타일제거
    private int[,] item = new int[7, 7];
    private int circuitcount = 0;

    public static int[,] arr = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };

    private void Awake()
    {
        arr = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };
        scoreTxt.text = "Score : " + score;
        GameObject boardInventory = GameObject.Find("BoardInventory");
        for (int i = 0; i < 25; i++)
        {
            boardSlot[i] = boardInventory.transform.GetChild(i).gameObject;
        }
    }

    public void Check()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (i != 0 && i != 6 && j != 0 && j != 6) continue;

                int val = Dfs(i, j, 0);

                if (val < 0)
                {
                    for (int k = 1; k < 6; k++)
                    {
                        for (int l = 1; l < 6; l++)
                        {
                            DestroyTile(k, l);
                            displayedTileCount--;
                        }
                    }
                    score += 1000;
                }

                if (val > 0)
                {
                    GetScore(val);
                }

                if (displayedTileCount >= 25)
                {
                    //gameOverTxt.text = "Your Score is " + score;
                    gameover = true;
                }

                circuitcount = 0;
            }
        }
        //턴 증가
        TurnCounting.Instance.turnCount++;
        //턴에 해당하는 점수 충족 여부 확인 및 게임 종료 결정
        TurnCounting.Instance.CheckTrunAndGoal();

        if (gameover)
        {
            SoundManager.Instance.PlayGameOverSound();
            gameOverTxt.gameObject.SetActive(true);
            gameOverTxt.text = "Your Score is " + score;
        }

        scoreTxt.text = "Score : " + score;
    }


    private int Dfs(int y, int x, int prev)
    {
        // 현재 위치 방문 표시 및 경로 저장
        if (y != 0 && y != 6 && x != 0 && x != 6)
        {
            path.Add((y, x));
        }

        circuitcount++;
        if(circuitcount > 1000)
        {
            path.Clear();
            return -1;
        }

        if (prev != 1 && y < 6 && (arr[y, x] & 4) > 0 && (arr[y + 1, x] & 1) > 0)
        {
            if (y + 1 == 6)
            {
                return path.Count;
            }
            else
            {
                int var = Dfs(y + 1, x, 4);
                if(var > 0)
                {
                    return var;
                }
            }
        }

        if (prev != 2 && x > 0 && (arr[y, x] & 8) > 0 && (arr[y, x - 1] & 2) > 0)
        {
            if (x - 1 == 0)
            {
                return path.Count;
            }
            else
            {
                int var = Dfs(y, x - 1, 8);
                if (var > 0)
                {
                    return var;
                }
            }
        }

        if (prev != 4 && y > 0 && (arr[y, x] & 1) > 0 && (arr[y - 1, x] & 4) > 0)
        {
            if (y - 1 == 0)
            {
                return path.Count;
            }
            else
            {
                int var = Dfs(y - 1, x, 1);
                if (var > 0)
                {
                    return var;
                }
            }
        }

        if (prev != 8 && x < 6 && (arr[y, x] & 2) > 0 && (arr[y, x + 1] & 8) > 0)
        {
            if (x + 1 == 6)
            {
                return path.Count;
            }
            else
            {
                int var = Dfs(y, x + 1, 2);
                if (var > 0)
                {
                    return var;
                }
            }
        }
        if(path.Count > 0)
        {
            path.RemoveAt(path.Count - 1);
        }
        return 0;
    }

    private void GetScore(int len)
    {
        // 점수 계산 : 배율 정해서. 이부분은 쉽게 수정되게. 배율변수 빼기.
        displayedTileCount -= len;
        score += len * len * len;
        // 타일 파괴. path에 들어있는 값들을 이용해서 파괴. 추가로 path에 들어있는 인덱스 값들 이용해서 item획득까지.
        foreach (var (y, x) in path)
        {
            DestroyTile(y, x);
            if (item[y, x] == 1)
            {
                //리롤+1
            }
            else if (item[y, x] == 2)
            {
                //타일제거
            }
            item[y, x] = 0;
        }
    }

    private void DestroyTile(int y, int x)
    {
        arr[y, x] = 0;
        if(boardSlot[5 * y + x - 6].transform.childCount > 0)
        {
            boardSlot[5 * y + x - 6].transform.GetChild(0).GetComponent<TileDestroy>().StartBreak();
        }
    }
}
