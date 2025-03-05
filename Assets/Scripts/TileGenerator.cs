using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

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

    void Start()
    {
        RerollToggle.onValueChanged.AddListener(Reroll);
        RerollCount = 3;
    }

    public void Update()
    {
        if (tileCount == 0)
        {
            Generate();
        }
        if (Input.GetKeyDown("r"))
        {
            BoardCheck.score = 0;
            SceneManager.LoadScene("MainScene");
        }
    }

    public void Reroll(bool isOn)
    {
        if (RerollCount == 0)
        {
            Debug.Log("�Ұ���!");
        }
        else if (RerollCount == 1)
        {
            RerollCount--;

            deleteTile(InventorySlot1);
            deleteTile(InventorySlot2);
            deleteTile(InventorySlot3);

            Generate();

            RerollBackground.color = Color.red;
        }
        else
        {
            RerollCount--;

            deleteTile(InventorySlot1);
            deleteTile(InventorySlot2);
            deleteTile(InventorySlot3);

            Generate();
        }
    }


    public void deleteTile(Transform slot)
    {
        Transform tile;

        if (slot.childCount > 0)
        {
            tile = slot.GetChild(0);

            minusTileCount();

            Destroy(tile.gameObject);
        }
    }


    public void minusTileCount()
    {
        tileCount -= 1;
    }

    public void Generate()
    {
        tileCount = 3;
        TileGenerate(InventorySlot1);
        TileGenerate(InventorySlot2);
        TileGenerate(InventorySlot3);
    }

    private int getRandNum()
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

    public void TileGenerate(Transform slot)
    {
        int randNum = getRandNum();
        int newType;

        GameObject newTile = Instantiate(Tile, slot);
        newTile.transform.SetParent(slot);
        Transform[] childList = newTile.GetComponentsInChildren<Transform>();

        switch (randNum)
        {
            case 1:
                newType = 10;
                ChageColor(childList[4]); ChageColor(childList[5]); ChageColor(childList[6]);
                break;
            case 2:
                newType = 5;
                ChageColor(childList[2]); ChageColor(childList[5]); ChageColor(childList[8]);
                break;
            case 3:
                newType = 6;
                ChageColor(childList[5]); ChageColor(childList[6]); ChageColor(childList[8]);
                break;
            case 4:
                newType = 12;
                ChageColor(childList[4]); ChageColor(childList[5]); ChageColor(childList[8]);
                break;
            case 5:
                newType = 9;
                ChageColor(childList[2]); ChageColor(childList[4]); ChageColor(childList[5]);
                break;
            case 6:
                newType = 3;
                ChageColor(childList[2]); ChageColor(childList[5]); ChageColor(childList[6]);
                break;
            case 7:
                newType = 7; // 0111
                ChageColor(childList[2]); ChageColor(childList[5]); ChageColor(childList[8]); ChageColor(childList[6]);
                ChangeColorMagenta(childList[4]); ChangeColorMagenta(childList[1]); ChangeColorMagenta(childList[3]); ChangeColorMagenta(childList[7]); ChangeColorMagenta(childList[9]);
                break;
            case 8:
                newType = 11; // 1011
                ChageColor(childList[2]); ChageColor(childList[5]); ChageColor(childList[6]); ChageColor(childList[4]);
                ChangeColorMagenta(childList[8]); ChangeColorMagenta(childList[1]); ChangeColorMagenta(childList[3]); ChangeColorMagenta(childList[7]); ChangeColorMagenta(childList[9]);
                break;
            case 9:
                newType = 14; // 1110
                ChageColor(childList[4]); ChageColor(childList[5]); ChageColor(childList[8]); ChageColor(childList[6]);
                ChangeColorMagenta(childList[2]); ChangeColorMagenta(childList[1]); ChangeColorMagenta(childList[3]); ChangeColorMagenta(childList[7]); ChangeColorMagenta(childList[9]);
                break;
            case 10:
                newType = 13; // 1101
                ChageColor(childList[2]); ChageColor(childList[5]); ChageColor(childList[8]); ChageColor(childList[4]);
                ChangeColorMagenta(childList[6]); ChangeColorMagenta(childList[1]); ChangeColorMagenta(childList[3]); ChangeColorMagenta(childList[7]); ChangeColorMagenta(childList[9]);
                break;
            case 11:
                newType = 15; // 1111
                ChageColor(childList[2]); ChageColor(childList[5]); ChageColor(childList[8]); ChageColor(childList[4]); ChageColor(childList[6]);
                ChangeColorYellow(childList[1]); ChangeColorYellow(childList[3]); ChangeColorYellow(childList[7]); ChangeColorYellow(childList[9]);
                break;
            default:
                newType = 0;
                break;
        }

        newTile.GetComponent<TileDraggable>().tileType = newType;
    }

    public void ChageColor(Transform transform)
    {
        transform.GetComponent<Image>().color = Color.white;
    }

    public void ChangeColorMagenta(Transform transform)
    {
        transform.GetComponent<Image>().color = new Color32(127, 61, 242, 255);
    }

    public void ChangeColorYellow(Transform transform)
    {
        transform.GetComponent<Image>().color = new Color32(230, 216, 3, 255);
    }
}
