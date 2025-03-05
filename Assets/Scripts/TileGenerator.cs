using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public enum Colors { WHITE, RED, MAGENTA, YELLOW };

public class TileGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform InventorySlot1;
    [SerializeField]
    private Transform InventorySlot2;
    [SerializeField]
    private Transform InventorySlot3;
    [SerializeField]
    private GameObject Tile;

    public Toggle RerollToggle;
    public Image RerollBackground;
    public int RerollCount;

    private int tileCount = 0;

    private void Start()
    {
        RerollToggle.onValueChanged.AddListener(Reroll);
        RerollCount = 3;
    }

    private void Update()
    {
        if (tileCount == 0)
        {
            Generate();
        }
    }

    public void Reset()
    {
        SoundManager.Instance.PlayDisplaySound();
        BoardCheck.score = 0;
        BoardCheck.gameover = false;
        SceneManager.LoadScene("MainScene");
    }

    public void Reroll(bool isOn)
    {
        if (RerollCount == 0)
        {
            SoundManager.Instance.PlayForbidSound();
        }
        else if (RerollCount == 1)
        {
            SoundManager.Instance.PlayDisplaySound();
            RerollCount--;

            DeleteTile(InventorySlot1);
            DeleteTile(InventorySlot2);
            DeleteTile(InventorySlot3);

            Generate();

            RerollBackground.color = Color.red;
        }
        else
        {
            SoundManager.Instance.PlayDisplaySound();
            RerollCount--;

            DeleteTile(InventorySlot1);
            DeleteTile(InventorySlot2);
            DeleteTile(InventorySlot3);

            Generate();
        }
    }


    private void DeleteTile(Transform slot)
    {
        Transform tile;

        if (slot.childCount > 0)
        {
            tile = slot.GetChild(0);

            MinusTileCount();

            Destroy(tile.gameObject);
        }
    }


    public void MinusTileCount()
    {
        tileCount -= 1;
    }

    private void Generate()
    {
        tileCount = 3;
        TileGenerate(InventorySlot1);
        TileGenerate(InventorySlot2);
        TileGenerate(InventorySlot3);
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
        int newType;

        GameObject newTile = Instantiate(Tile, slot);
        newTile.transform.SetParent(slot);
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
                newColor = new Color32(127, 61, 242, 255);
                break;
            case Colors.YELLOW:
                newColor = new Color32(230, 216, 3, 255);
                break;
        }

        foreach(Transform elem in colorList)
        {
            elem.GetComponent<Image>().color = newColor;
        }
    }
}
