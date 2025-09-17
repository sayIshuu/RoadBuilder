using System.Collections.Generic;
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
    private List<BoardSlot> boardSlots = new List<BoardSlot>(); // ���� ���� ����Ʈ
    private BoardSlot currentHover;                         // ���� ȣ�� �� ����

    public int tileType;

    private void Awake()
    {
        canvas = FindFirstObjectByType<GameCanvas>().GetComponent<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        tileDraggable = GetComponent<TileDraggable>();
        tileGenerator = GameObject.Find("TileGenerator");
    }

    private void Start()
    {
        boardSlots = new List<BoardSlot>(FindObjectsByType<BoardSlot>(FindObjectsSortMode.None));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //SoundManager.Instance.PlaySelectSound();
        SoundManager.Instance.PlaySlideSound();

        previousParent = transform.parent;

        transform.SetParent(canvas);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;

        DetectBoardSlot(eventData.position);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        if (currentHover != null)
        {
            // ���� ��ġ
            currentHover.PlaceTile(gameObject);

            // ���� ���̶���Ʈ ����
            currentHover.ResetSlotColor();
            currentHover = null;
        }

        if (!transform.parent.CompareTag("Board") || transform.parent.childCount > 1)
        {
            transform.SetParent(previousParent);
            rect.position = previousParent.GetComponent<RectTransform>().position;
            SoundManager.Instance.PlayDisplaySound();
            //return false;
        }
        else
        {
            int idx = transform.parent.GetComponent<BoardSlot>().GetIdx();
            BoardCheck.adj[idx / 5 + 1, idx % 5 + 1] = tileType;
            tileGenerator = GameObject.Find("TileGenerator");
            tileGenerator.GetComponent<BoardCheck>().displayedTileCount += 1;
            tileDraggable.enabled = false;

            tileGenerator.GetComponent<TileGenerator>().MinusTileCount();
            tileGenerator.GetComponent<BoardCheck>().Check();

            SoundManager.Instance.PlayDisplaySound();
            //return true;
        }
    }

    private void DetectBoardSlot(Vector2 screenPos)
    {
        BoardSlot hovered = null;
        foreach (var slot in boardSlots)
        {
            // ���� BoardSlot.IsPositionOverSlot(Vector2 screenPos) ����
            if (slot.IsPositionOverSlot(screenPos))
            {
                hovered = slot;
                break;
            }
        }

        if (hovered != currentHover)
        {
            if (currentHover != null) currentHover.ResetSlotColor();
            if (hovered != null) hovered.HighlightSlot();
            currentHover = hovered;
        }
    }
}
