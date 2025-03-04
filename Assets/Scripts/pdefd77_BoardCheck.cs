using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class pdefd77_BoardCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject[] boardSlot;
    [SerializeField]
    private TextMeshProUGUI scoreTxt;
    [SerializeField]
    private TextMeshProUGUI gameOverTxt;
    private int score = 0;
    public int displayedTileCount = 0;
    //dfs������ ���� list. �� int������ arr�� �ε������� ���ϴ�.
    private List<(int, int)> path = new List<(int, int)>();
    private bool[,] visited = new bool[7, 7];
    //���� ����. ���̿� ���� �󸶳� ���� ������ ��
    [SerializeField]
    private int scoreMultiplier = 1;

    public static int[,] arr = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };

    public void Awake()
    {
        arr = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };
        scoreTxt.text = "Score : " + score;
    }

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
                    if (displayedTileCount >= 25)
                    {
                        gameOverTxt.text = "Your Score is " + score;
                    }
                    if(gameoverScore >= 25)
                    {
                        gameOverTxt.text = "Your Score is " + score;
                    }
                    
                    gameOverTxt.gameObject.SetActive(true);
                    gameOverTxt.text = "Your length is " + val;
                }
            }
        }

        scoreTxt.text = "Score : " + score;
    }

    private int dfs(int y, int x, int prev)
    {
        // �̹� �湮�� ��� Ž�� ����
        if (visited[y, x]) return 0;

        // ���� ��ġ �湮 ǥ�� �� ��� ����
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

        // Ž�� ���� �� �湮�� ��� �ʱ�ȭ
        visited[y, x] = false;
        path.RemoveAt(path.Count - 1);

        return 0;
    }

    private void getScore(int len)
    {
        // ���� ��� : ���� ���ؼ�. �̺κ��� ���� �����ǰ�. �������� ����.
        displayedTileCount -= len;
        score += len * scoreMultiplier;
        // Ÿ�� �ı�. path�� ����ִ� ������ �̿��ؼ� �ı�.
        // { ���� �ڵ��߰� }
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
