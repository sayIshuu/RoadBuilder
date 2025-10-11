using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class BoardCheck : MonoBehaviour
{
    [SerializeField] private GameOverCanvas gameOverCanvas;
    [SerializeField] private MissionController missionController;
    [SerializeField] private GameObject[] boardSlot;
    private ScoreManager scoreManager;
    private SfxManager sfxManager;
    public static bool gameover = false;
    public int displayedTileCount = 0;
    private int[] uf = new int[49];

    private int[] checkNum = new int[] { 1, 2, 3, 4, 5, 7, 13, 14, 20, 21, 27, 28, 34, 35, 41, 43, 44, 45, 46, 47 };

    public static int[,] adj = new int[7, 7];

    public bool useCompletionAnimation = true;
    public static bool isAnimating = false;

    [Header("Animation Settings")]
    [Tooltip("한 타일 연출 진행 시간 (초기값)")]
    [SerializeField] private float bounceDuration = 0.5f;
    [Tooltip("다음 타일 연출 시간 감소 비율 (1에 가까울수록 느리게 짧아짐)")]
    [SerializeField, Range(0.5f, 1f)] private float speedIncreaseRate = 0.9f;
    [Tooltip("클릭 시 연출이 빨라지는 배율")]
    [SerializeField, Range(1f, 5f)] private float fastForwardMultiplier = 2.0f;
    [Tooltip("다음 타일 연출 시작 되는 비율 (1에 가까울수록 많이 겹침)")]
    [SerializeField, Range(0f, 1f)] private float dominoOverlap = 0.7f;
    [Tooltip("전체 바운스 연출에 적용할 이징 함수")]
    [SerializeField] private Ease bounceEase = Ease.InOutQuad;


    private void Awake()
    {
        adj = new int[7, 7] { { 0, 4, 4, 4, 4, 4, 0 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 2, 0, 0, 0, 0, 0, 8 }, { 0, 1, 1, 1, 1, 1, 0 } };
        gameover = false;
        GameObject boardInventory = GameObject.Find("BoardInventory");
        scoreManager = FindAnyObjectByType<ScoreManager>();
        sfxManager = FindAnyObjectByType<SfxManager>();
        for (int i = 0; i < 25; i++)
        {
            boardSlot[i] = boardInventory.transform.GetChild(i).gameObject;
        }
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
        else
        {
            ProcessTurn();
        }
    }

    private void ProcessTurn()
    {
        //턴 증가
        TurnCounting.Instance.turnCount++;
        //턴에 해당하는 점수 충족 여부 확인 및 게임 종료 결정
        TurnCounting.Instance.CheckTurnAndGoal();

        if (displayedTileCount >= 25) // gameover
        {
            gameover = true;
        }
        if (gameover)
        {
            gameOverCanvas.ActivateGameOverUI();
        }
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
        List<Vector2Int> pathTiles = new List<Vector2Int>();
        for (int i = 1; i <= 5; i++)
        {
            for (int j = 1; j <= 5; j++)
            {
                if (UfFind(uf[7 * i + j]) == num)
                {
                    pathTiles.Add(new Vector2Int(j, i));
                }
            }
        }

        if (useCompletionAnimation && pathTiles.Count > 0)
        {
            StartCoroutine(PlayCompletionAnimation(pathTiles, num));
        }
        else
        {
            int len = pathTiles.Count;
            foreach (var tilePos in pathTiles)
            {
                DestroyTile(tilePos.y, tilePos.x);
            }

            if(len > 11) SoundManager.Instance.PlayLargeScoreSound();
            else SoundManager.Instance.PlayScoreSound();

            displayedTileCount -= len;
            scoreManager.AddScore(len * len * len);
            missionController.CheckAllLengthMission(len);

            ProcessTurn();
        }
    }

    private List<Vector2Int> SortPath(List<Vector2Int> path, int num)
    {
        if (path.Count == 0) return path;

        List<Vector2Int> sortedPath = new List<Vector2Int>();
        Vector2Int startNode = Vector2Int.zero;

        // 어느 벽에서 시작하는지 찾기
        // 우선순위 : 위 - 왼 - 오 - 아래
        for (int i = 1; i <= 5; i++) // Top wall
        {
            if (UfFind(uf[i]) == num) { startNode = GetBoardTile(i, 0); break; }
        }
        if (startNode == Vector2Int.zero)
        {
            for (int i = 1; i <= 5; i++) // Left wall
            {
                if (UfFind(uf[i * 7]) == num) { startNode = GetBoardTile(0, i); break; }
            }
        }
        if (startNode == Vector2Int.zero)
        {
            for (int i = 1; i <= 5; i++) // Right wall
            {
                if (UfFind(uf[i * 7 + 6]) == num) { startNode = GetBoardTile(6, i); break; }
            }
        }
        if (startNode == Vector2Int.zero)
        {
            for (int i = 1; i <= 5; i++) // Bottom wall
            {
                if (UfFind(uf[42 + i]) == num) { startNode = GetBoardTile(i, 6); break; }
            }
        }

        if (!path.Contains(startNode))
        {
            var firstConnected = FindFirstConnected(startNode, num);
            if(firstConnected != Vector2Int.zero) startNode = firstConnected;
        }


        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startNode);
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            sortedPath.Add(current);

            int x = current.x;
            int y = current.y;

            // Check neighbors
            // Up
            if (y > 1 && (adj[y, x] & 1) > 0 && (adj[y - 1, x] & 4) > 0 && path.Contains(new Vector2Int(x, y - 1)) && !visited.Contains(new Vector2Int(x, y - 1))) { queue.Enqueue(new Vector2Int(x, y - 1)); visited.Add(new Vector2Int(x, y - 1)); }
            // Right
            if (x < 5 && (adj[y, x] & 2) > 0 && (adj[y, x + 1] & 8) > 0 && path.Contains(new Vector2Int(x + 1, y)) && !visited.Contains(new Vector2Int(x + 1, y))) { queue.Enqueue(new Vector2Int(x + 1, y)); visited.Add(new Vector2Int(x + 1, y)); }
            // Down
            if (y < 5 && (adj[y, x] & 4) > 0 && (adj[y + 1, x] & 1) > 0 && path.Contains(new Vector2Int(x, y + 1)) && !visited.Contains(new Vector2Int(x, y + 1))) { queue.Enqueue(new Vector2Int(x, y + 1)); visited.Add(new Vector2Int(x, y + 1)); }
            // Left
            if (x > 1 && (adj[y, x] & 8) > 0 && (adj[y, x - 1] & 2) > 0 && path.Contains(new Vector2Int(x - 1, y)) && !visited.Contains(new Vector2Int(x - 1, y))) { queue.Enqueue(new Vector2Int(x - 1, y)); visited.Add(new Vector2Int(x - 1, y)); }
        }

        return sortedPath;
    }

    // 벽에서 시작하는 타일이 없을 때, 연결된 타일 중 하나를 찾아 반환
    private Vector2Int FindFirstConnected(Vector2Int startNode, int num)
    {
        int x = startNode.x;
        int y = startNode.y;

        if (y == 0) // Top
        {
            if ((adj[y + 1, x] & 1) > 0 && UfFind(uf[7 * (y+1) + x]) == num) return new Vector2Int(x, y + 1);
        }
        else if (x == 0) // Left
        {
            if ((adj[y, x+1] & 8) > 0 && UfFind(uf[7 * y + x+1]) == num) return new Vector2Int(x + 1, y);
        }
        else if (x == 6) // Right
        {
            if ((adj[y, x-1] & 2) > 0 && UfFind(uf[7 * y + x-1]) == num) return new Vector2Int(x - 1, y);
        }
        else if (y == 6) // Bottom
        {
            if ((adj[y-1, x] & 4) > 0 && UfFind(uf[7 * (y-1) + x]) == num) return new Vector2Int(x, y - 1);
        }
        return Vector2Int.zero;
    }


    // 벽 좌표를 보드 좌표로 변환
    private Vector2Int GetBoardTile(int x, int y)
    {
        if (y == 0) return new Vector2Int(x, 1);
        if (y == 6) return new Vector2Int(x, 5);
        if (x == 0) return new Vector2Int(1, y);
        if (x == 6) return new Vector2Int(5, y);
        return new Vector2Int(x, y);
    }


    // 연출 코루틴
    IEnumerator PlayCompletionAnimation(List<Vector2Int> pathTiles, int num)
    {
        if (sfxManager != null) sfxManager.ResetSfxIndex();

        isAnimating = true;
        var sortedPath = SortPath(pathTiles, num);
        float currentDuration = bounceDuration;
        float totalDuration = 0;
        float delay = 0;

        bool isFastForward = Input.GetMouseButton(0);

        foreach (var tilePos in sortedPath)
        {
            if (boardSlot[5 * tilePos.y + tilePos.x - 6].transform.childCount > 0)
            {
                Transform tileTransform = boardSlot[5 * tilePos.y + tilePos.x - 6].transform.GetChild(0);
                Sequence sequence = DOTween.Sequence();

                // 사운드 재생을 시퀀스의 가장 처음에 콜백으로 추가합니다.
                if (sfxManager != null)
                {
                    sequence.AppendCallback(() => sfxManager.PlayRisingSfx());
                    sequence.AppendCallback(() => VibrationManager.Instance.Vibrate(VibrationManager.VibrationType.Pop));
                }

                sequence.Append(tileTransform.DOScale(1.2f, currentDuration * 0.25f));
                sequence.Append(tileTransform.DOScale(0.8f, currentDuration * 0.3f));
                sequence.Append(tileTransform.DOScale(1.0f, currentDuration * 0.25f));

                sequence.SetEase(bounceEase);
                sequence.SetDelay(delay);

                if (isFastForward)
                {
                    sequence.timeScale = fastForwardMultiplier;
                }
                sequence.Play();

                totalDuration = delay + currentDuration;
                delay += currentDuration * (1 - dominoOverlap);
                currentDuration *= speedIncreaseRate;
            }
        }

        yield return new WaitForSeconds(totalDuration / (isFastForward ? fastForwardMultiplier : 1f));

        int len = pathTiles.Count;
        foreach (var tilePos in pathTiles)
        {
            DestroyTile(tilePos.y, tilePos.x);
        }

        if (len > 11) SoundManager.Instance.PlayLargeScoreSound();
        else SoundManager.Instance.PlayScoreSound();

        displayedTileCount -= len;
        scoreManager.AddScore(len * len * len);
        missionController.CheckAllLengthMission(len);
        isAnimating = false;

        ProcessTurn();
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
