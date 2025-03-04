using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class pdefd77_BoardCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject[] boardSlot;
    [SerializeField]
    private TextMeshProUGUI gameOverTxt;
    private int score = 0;
    //dfs추적을 위한 list. 각 int값으로 arr의 인덱스값이 들어갑니다.
    private List<(int, int)> path = new List<(int, int)>();
    private bool[,] visited = new bool[7, 7];

    public static int[,] arr = new int[7, 7] { 
                                    { 0, 4, 4, 4, 4, 4, 0 }, 
                                    { 2, 0, 0, 0, 0, 0, 8 }, 
                                    { 2, 0, 0, 0, 0, 0, 8 }, 
                                    { 2, 0, 0, 0, 0, 0, 8 }, 
                                    { 2, 0, 0, 0, 0, 0, 8 }, 
                                    { 2, 0, 0, 0, 0, 0, 8 }, 
                                    { 0, 1, 1, 1, 1, 1, 0 } 
    };

    public void check()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (i != 0 && i != 6 && j != 0 && j != 6) continue;

                int val = dfs(i, j, 0);

                if (val > 0)
                {
                    gameOverTxt.text = "Your length is " + val;
                }
            }
        }
    }

    /*
    private int dfs(int y, int x, int prev, int len)
    {
        if (prev != 1 && y < 6 && (arr[y, x] & 4) > 0 && (arr[y + 1, x] & 1) > 0)
        {
            if (y + 1 == 6)
            {
                return len;
            }
            else
            {
                return dfs(y + 1, x, 4, len + 1);
            }
        }

        if (prev != 2 && x < 6 && (arr[y, x] & 8) > 0 && (arr[y, x - 1] & 2) > 0)
        {
            if (x - 1 == 0)
            {
                return len;
            }
            else
            {
                return dfs(y, x - 1, 8, len + 1);
            }
        }

        if (prev != 4 && y > 0 && (arr[y, x] & 1) > 0 && (arr[y - 1, x] & 4) > 0)
        {
            if (y - 1 == 0)
            {
                return len;
            }
            else
            {
                return dfs(y - 1, x, 1, len + 1);
            }
        }

        if (prev != 8 && x > 0 && (arr[y, x] & 2) > 0 && (arr[y, x + 1] & 8) > 0)
        {
            if (x + 1 == 6)
            {
                return len;
            }
            else
            {
                return dfs(y, x + 1, 2, len + 1);
            }
        }

        return 0;
    }
    */

    private int dfs(int y, int x, int prev)
    {
        // 이미 방문한 경우 탐색 종료
        if (visited[y, x]) return 0;

        // 현재 위치 방문 표시 및 경로 저장
        visited[y, x] = true;
        path.Add((y, x));

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

        if (prev != 2 && x < 6 && (arr[y, x] & 8) > 0 && (arr[y, x - 1] & 2) > 0)
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

        if (prev != 8 && x > 0 && (arr[y, x] & 2) > 0 && (arr[y, x + 1] & 8) > 0)
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

        // 탐색 종료 후 방문한 경로 초기화
        visited[y, x] = false;
        path.RemoveAt(path.Count - 1);

        return 0;
    }


    private void destroyTile(int y, int x)
    {
        if (Random.Range(0, 4) >= 0)
        {
            arr[y, x] = 0;
            Destroy(boardSlot[5 * y + x - 6].transform.GetChild(0).gameObject);
        }
    }
}
