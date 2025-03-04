using UnityEngine;
using TMPro;

public class pdefd77_BoardCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject[] boardSlot;
    [SerializeField]
    private TextMeshProUGUI gameOverTxt;
    private int score = 0;

    public static int[,] arr = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };

    public void check()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (i != 0 && i != 6 && j != 0 && j != 6) continue;

                int val = dfs(i, j, 0, 0);

                if (val > 0)
                {
                    gameOverTxt.text = "Your length is " + val;
                }
            }
        }
    }

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

    private void destroyTile(int y, int x)
    {
        if (Random.Range(0, 4) >= 0)
        {
            arr[y, x] = 0;
            Destroy(boardSlot[5 * y + x - 6].transform.GetChild(0).gameObject);
        }
    }
}
