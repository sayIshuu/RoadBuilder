using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.U2D;

public enum Colors { WHITE, RED, MAGENTA, YELLOW, TRANSPARENT };

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private TouchPadHandler touchPadHandler;
    [SerializeField] private Transform[] _offerSlot;
    [SerializeField] private BoardSlot[] _boardSlot;

    [SerializeField] private GameObject Tile;
    [SerializeField] private GameObject FakeTile;
    [SerializeField] private SpriteAtlas tileSpriteAtlas;

    private int tileCount = 0;
    private List<int> tileTypes = new List<int>();

    private void Start()
    {
        Generate();
    }

    //이벤트로 뺄만한 함수
    private void Update()
    {
        if (tileCount == 0)
        {
            Generate();
        }
    }

    public void Reroll()
    {
        Generate();
    }

    private void DeleteTile(Transform slot)
    {
        // slot의 모든 자식 오브젝트를 순회하며 파괴합니다.
        // 이렇게 하면 타일이 여러 개 쌓여도 모두 확실하게 제거됩니다.
        foreach (Transform childTile in slot)
        {
            MinusTileCount();
            Destroy(childTile.gameObject);
        }
    }

    private void DeleteAllSlotTiles()
    {
        // Offer Slot의 모든 블록 제거
        for (int i = 0; i < 3; i++)
        {
            DeleteTile(_offerSlot[i]);
        }
    }

    public void DeleteAllBoardTiles()
    {
        // Board Slot의 모든 블록 제거
        for (int i = 0; i < 25; i++)
        {
            DeleteTile(_boardSlot[i].transform);
        }
    }

    public void MinusTileCount()
    {
        tileCount --;
    }

    private void Generate()
    {
        DeleteAllSlotTiles();

        // 모두 같은 타일이 나오지 않을 때까지 계속 반복하는 루프
        while (true)
        {
            // 루프가 돌 때마다 상태를 초기화
            tileCount = 3;
            tileTypes.Clear();

            // 타일 3개를 생성
            for (int i = 0; i < 3; i++)
            {
                GenerateRandomTile(_offerSlot[i]);
            }

            // 생성 성공
            if (!IsAllSame())
            {
                break;
            }

            // 만약 모두 같다면, 다시 생성하기 전에 방금 만든 타일들을 삭제
            DeleteAllSlotTiles();
        }
    }

    private int GetRandNum()
    {
        float randomValue = Random.Range(0f, 100f); // 0~100 사이의 랜덤 숫자

        if (randomValue < 95f)        // 95% 확률 → 1~6
        {
            return Random.Range(1, 7);
        }
        else if (randomValue < 99.5f) // 4.5% 확률 → 7~10
        {
            return Random.Range(7, 11);
        }
        else                          // 0.5% 확률 → 11
        {
            return 11;
        }
    }

    private void GenerateRandomTile(Transform slot)
    {
        // 랜덤 타일 생성
        int randNum = GetRandNum();
        GenerateTile(slot, randNum);
    }

    GameObject GenerateTile(Transform slot, int tileIndex)
    {
        if (tileIndex == -1)
        {
            // Fake 타일 (튜토리얼에 사용되는 빈 타일)
            GameObject fakeTile = Instantiate(FakeTile, slot);
            return fakeTile;
        }

        // Tile Index에 따른 타일 생성
        tileTypes.Add(tileIndex);

        int newType;

        GameObject newTile = Instantiate(Tile, slot);

        switch (tileIndex)
        {
            case 1:
                newType = 10; // ─모양
                break;
            case 2:
                newType = 5; // │모양
                break;
            case 3:
                newType = 6; // ┌모양
                break;
            case 4:
                newType = 12; // ┐모양
                break;
            case 5:
                newType = 9; // ┘모양
                break;
            case 6:
                newType = 3; // └모양
                break;
            case 7:
                newType = 7; // ├모양 1101
                break;
            case 8:
                newType = 11; // ┴모양 1110
                break;
            case 9:
                newType = 14; // ┬모양 0111
                break;
            case 10:
                newType = 13; // ┤모양 1011
                break;
            case 11:
                newType = 15; // ┼모양 1111
                break;
            default:
                newType = 0;
                break;
        }

        // 타일 이미지 변경 및 초기화
        Sprite tileSprite = tileSpriteAtlas.GetSprite("Tile" + tileIndex);
        newTile.GetComponent<Image>().sprite = tileSprite;
        newTile.GetComponent<TileDraggable>().Init(newType, _boardSlot);
        return newTile;
    }

    private bool IsAllSame()
    {
        if (tileTypes.Count < 3) return false;
        return (tileTypes[0] == tileTypes[1]) && (tileTypes[1] == tileTypes[2]);
    }

    public void GenerateCustomTiles(int[] customIndex)
    {
        // Offer Slot에 커스텀 타일 생성
        DeleteAllSlotTiles();
        tileCount = 3;
        tileTypes.Clear();

        for (int i = 0; i < 3; i++)
        {
            GenerateTile(_offerSlot[i], customIndex[i]);
        }
    }

    public void GenerateCustomBoard(int[] customIndex)
    {
        // Board Slot에 커스텀 타일 생성
        BoardCheck boardCheck = GetComponent<BoardCheck>();
        DeleteAllBoardTiles();
        boardCheck.displayedTileCount = 0;

        for (int i = 0; i < _boardSlot.Length; i++)
        {
            // 빈 칸
            if (customIndex[i] == 0) continue;

            // 타일 생성
            GameObject newTile = GenerateTile(_boardSlot[i].transform, customIndex[i]);

            int idx = newTile.transform.parent.GetComponent<BoardSlot>().GetIdx();
            TileDraggable tileDraggable = newTile.GetComponent<TileDraggable>();
            CanvasGroup canvasGroup = newTile.GetComponent<CanvasGroup>();

            BoardCheck.adj[idx / 5 + 1, idx % 5 + 1] = tileDraggable.TileType;

            // Fake 타일 아닐 시 카운트
            if (tileDraggable.TileType != -1) boardCheck.displayedTileCount += 1;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void DeleteAnyTiles()
    {
        // 씬 내에 존재하는 모든 타일 제거
        TileDraggable[] exitTile = FindObjectsByType<TileDraggable>(FindObjectsSortMode.None);
        for (int i = 0; i < exitTile.Length; i++)
        {
            Destroy(exitTile[i].gameObject);
        }
    }
}
