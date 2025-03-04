using UnityEngine;
using UnityEngine.EventSystems;

public class pdefd77_TileDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform canvas;
    private Transform previousParent;
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private pdefd77_TileDraggable tileDraggable;
    private GameObject tileGenerator;

    public int tileType;

    public void Awake()
    {
        canvas = FindFirstObjectByType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        tileDraggable = GetComponent<pdefd77_TileDraggable>();
        tileGenerator = GameObject.Find("TileGenerator");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previousParent = transform.parent;

        transform.SetParent(canvas);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == canvas || transform.parent.tag == "Inventory" || transform.parent.childCount > 1)
        {
            transform.SetParent(previousParent);
            rect.position = previousParent.GetComponent<RectTransform>().position;
        }
        else
        {
            int idx = transform.parent.GetComponent<pdefd77_BoardSlot>().getIdx();
            pdefd77_BoardCheck.arr[idx / 10, idx % 10] = tileType;
            tileGenerator = GameObject.Find("TileGenerator");
            tileGenerator.GetComponent<pdefd77_BoardCheck>().gameoverScore += 1;
            tileDraggable.enabled = false;

            tileGenerator.GetComponent<pdefd77_TileGenerator>().minusTileCount();
            tileGenerator.GetComponent<pdefd77_BoardCheck>().check();
        }
    }
}
