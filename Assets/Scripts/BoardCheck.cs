using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class BoardCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject[] boardSlot;
    [SerializeField]
    private TextMeshProUGUI scoreTxt;
    [SerializeField]
    private TextMeshProUGUI gameOverTxt;
    private int score = 0;
    public int displayedTileCount = 0;
    //dfs추적을 위한 list. 각 int값으로 arr의 인덱스값이 들어갑니다.
    private List<(int, int)> path = new List<(int, int)>();
    //어떤 아이템을 그 칸에 들어있는지 나타내는 배열. 0 : 빈칸, 1 : 리롤+1, 2 : 타일제거
    private int[,] item = new int[7, 7];
    //배율 변수. 길이에 따라 얼마나 점수 증폭될 지
    [SerializeField]
    private int scoreMultiplier = 1;

    public static int[,] arr = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };

    public void Awake()
    {
        arr = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };
        scoreTxt.text = "Score : " + score;
        GameObject boardInventory = GameObject.Find("BoardInventory");
        for (int i = 0; i < 25; i++)
        {
            boardSlot[i] = boardInventory.transform.GetChild(i).gameObject;
        }
    }

    public void check()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (i != 0 && i != 6 && j != 0 && j != 6) continue;

                path.Clear();
                int val = dfs(i, j, 0);

                if (val > 0)
                {
                    if (displayedTileCount >= 25)
                    {
                        gameOverTxt.text = "Your Score is " + score;
                    }

                    //gameOverTxt.gameObject.SetActive(true);
                    //gameOverTxt.text = "Your length is " + val;

                    getScore(val);
                }
            }
        }

        scoreTxt.text = "Score : " + score;
    }


    private int dfs(int y, int x, int prev)
    {
        // 현재 위치 방문 표시 및 경로 저장
        if (y != 0 && y != 6 && x != 0 && x != 6)
        {
            path.Add((y, x));
        }

        if (prev != 1 && y < 6 && (arr[y, x] & 4) > 0 && (arr[y + 1, x] & 1) > 0)
        {
            if (y + 1 == 6)
            {
                return path.Count;

            }
            else
            {
                return dfs(y + 1, x, 4);
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
                return dfs(y, x - 1, 8);
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
                return dfs(y - 1, x, 1);
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
                return dfs(y, x + 1, 2);
            }
        }
        return 0;
    }

    private void getScore(int len)
    {
        // 점수 계산 : 배율 정해서. 이부분은 쉽게 수정되게. 배율변수 빼기.
        displayedTileCount -= len;
        score += len * scoreMultiplier;
        // 타일 파괴. path에 들어있는 값들을 이용해서 파괴. 추가로 path에 들어있는 인덱스 값들 이용해서 item획득까지.
        foreach (var (y, x) in path)
        {
            destroyTile(y, x);
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

    private void destroyTile(int y, int x)
    {
        arr[y, x] = 0;
        Destroy(boardSlot[5 * y + x - 6].transform.GetChild(0).gameObject);
    }
}
