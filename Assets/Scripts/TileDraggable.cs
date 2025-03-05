using UnityEngine;
using UnityEngine.EventSystems;

public class TileDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform canvas;
    private Transform previousParent;
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private TileDraggable tileDraggable;
    private GameObject tileGenerator;

    public int tileType;

    private void Awake()
    {
        canvas = FindFirstObjectByType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        tileDraggable = GetComponent<TileDraggable>();
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

        if (transform.parent == canvas || transform.parent.childCount > 1)
        {
            transform.SetParent(previousParent);
            rect.position = previousParent.GetComponent<RectTransform>().position;
        }
        else
        {
            int idx = transform.parent.GetComponent<BoardSlot>().GetIdx();
            BoardCheck.arr[idx / 10, idx % 10] = tileType;
            tileGenerator = GameObject.Find("TileGenerator");
            tileGenerator.GetComponent<BoardCheck>().displayedTileCount += 1;
            tileDraggable.enabled = false;

            tileGenerator.GetComponent<TileGenerator>().MinusTileCount();
            tileGenerator.GetComponent<BoardCheck>().Check();

            //턴 증가
            TurnCounting.Instance.turnCount++;
            //턴에 해당하는 점수 충족 여부 확인 및 게임 종료 결정
            TurnCounting.Instance.CheckTrunAndGoal();
        }
    }
}
