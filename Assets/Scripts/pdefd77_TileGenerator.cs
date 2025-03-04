using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class pdefd77_TileGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform InventorySlot1;
    [SerializeField]
    private Transform InventorySlot2;
    [SerializeField]
    private Transform InventorySlot3;
    [SerializeField]
    private GameObject[] TileSet;

    private int tileCount = 0;

    public void Update()
    {
        if (tileCount == 0)
        {
            Generate();
        }
        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene("MainScene");
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

    public void TileGenerate(Transform slot)
    {
        int randNum = Random.Range(1, 7);
        int newType;

        GameObject newTile = Instantiate(TileSet[randNum-1], slot);
        newTile.transform.SetParent(slot);

        switch (randNum)
        {
            case 1:
                newType = 10;
                break;
            case 2:
                newType = 5;
                break;
            case 3:
                newType = 6;
                break;
            case 4:
                newType = 12;
                break;
            case 5:
                newType = 9;
                break;
            case 6:
                newType = 3;
                break;
            default:
                newType = 0;
                break;
        }

        newTile.GetComponent<pdefd77_TileDraggable>().tileType = newType;
    }
}
