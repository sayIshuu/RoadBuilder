using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum Colors { WHITE, RED, MAGENTA, YELLOW };

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private TouchPadHandler touchPadHandler;
    [SerializeField] private Transform InventorySlot1;
    [SerializeField] private Transform InventorySlot2;
    [SerializeField] private Transform InventorySlot3;
    [SerializeField] private GameObject Tile;

    private int tileCount = 0;
    private List<int> tileTypes = new List<int>();

    private void Awake()
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


    public void MinusTileCount()
    {
        tileCount --;
    }

    private void Generate()
    {
        DeleteTile(InventorySlot1);
        DeleteTile(InventorySlot2);
        DeleteTile(InventorySlot3);

        // 모두 같은 타일이 나오지 않을 때까지 계속 반복하는 루프
        while (true)
        {
            // 루프가 돌 때마다 상태를 초기화
            tileCount = 3;
            tileTypes.Clear();

            // 타일 3개를 생성
            TileGenerate(InventorySlot1);
            TileGenerate(InventorySlot2);
            TileGenerate(InventorySlot3);

            // 생성 성공
            if (!IsAllSame())
            {
                break;
            }

            // 만약 모두 같다면, 다시 생성하기 전에 방금 만든 타일들을 삭제
            Debug.Log("All same tiles generated.");
            DeleteTile(InventorySlot1);
            DeleteTile(InventorySlot2);
            DeleteTile(InventorySlot3);
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

    private void TileGenerate(Transform slot)
    {
        int randNum = GetRandNum();

        tileTypes.Add(randNum);

        int newType;

        GameObject newTile = Instantiate(Tile, slot);
        //newTile.transform.SetParent(slot);
        Transform[] childList = newTile.GetComponentsInChildren<Transform>();

        switch (randNum)
        {
            case 1:
                newType = 10; // ─모양
                ChangeColor(Colors.WHITE, childList[4], childList[5],childList[6]);
                break;
            case 2:
                newType = 5; // │모양
                ChangeColor(Colors.WHITE, childList[2], childList[5], childList[8]);
                break;
            case 3:
                newType = 6; // ┌모양
                ChangeColor(Colors.WHITE, childList[5], childList[6], childList[8]);
                break;
            case 4:
                newType = 12; // ┐모양
                ChangeColor(Colors.WHITE, childList[4], childList[5], childList[8]);
                break;
            case 5:
                newType = 9; // ┘모양
                ChangeColor(Colors.WHITE, childList[2], childList[4], childList[5]);
                break;
            case 6:
                newType = 3; // └모양
                ChangeColor(Colors.WHITE, childList[2], childList[5], childList[6]);
                break;
            case 7:
                newType = 7; // ┬모양 0111
                ChangeColor(Colors.WHITE, childList[2], childList[5], childList[6], childList[8]);
                ChangeColor(Colors.MAGENTA, childList[1], childList[3], childList[4], childList[7], childList[9]);
                break;
            case 8:
                newType = 11; // ┤모양 1011
                ChangeColor(Colors.WHITE, childList[2], childList[4], childList[5], childList[6]);
                ChangeColor(Colors.MAGENTA, childList[1], childList[3], childList[7], childList[8], childList[9]);
                break;
            case 9:
                newType = 14; // ┴모양 1110
                ChangeColor(Colors.WHITE, childList[4], childList[5], childList[6], childList[8]);
                ChangeColor(Colors.MAGENTA, childList[1], childList[2], childList[3], childList[7], childList[9]);
                break;
            case 10:
                newType = 13; // ├모양 1101
                ChangeColor(Colors.WHITE, childList[2], childList[4], childList[5], childList[8]);
                ChangeColor(Colors.MAGENTA, childList[1], childList[3], childList[6], childList[7], childList[9]);
                break;
            case 11:
                newType = 15; // ┼모양 1111
                ChangeColor(Colors.WHITE, childList[2], childList[4], childList[5], childList[6], childList[8]);
                ChangeColor(Colors.YELLOW, childList[1], childList[3], childList[7], childList[9]);
                break;
            default:
                newType = 0;
                break;
        }

        newTile.GetComponent<TileDraggable>().tileType = newType;
    }

    private void ChangeColor(Colors colorType, params Transform[] colorList)
    {
        Color newColor = Color.black;

        switch (colorType)
        {
            case Colors.WHITE:
                newColor = Color.white;
                break;
            case Colors.RED:
                newColor = Color.red;
                break;
            case Colors.MAGENTA:
                newColor = new Color32(155, 165, 248, 255);
                break;
            case Colors.YELLOW:
                newColor = new Color32(255, 83, 110, 255);
                break;
        }

        foreach(Transform elem in colorList)
        {
            elem.GetComponent<Image>().color = newColor;
        }
    }

    private bool IsAllSame()
    {
        if (tileTypes.Count < 3) return false;
        return (tileTypes[0] == tileTypes[1]) && (tileTypes[1] == tileTypes[2]);
    }
}
