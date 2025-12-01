using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static VibrationManager;

public class TileDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform canvas;
    private Transform previousParent;
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private TileGenerator tileGenerator;
    private BoardCheck boardCheck;

    private BoardSlot[] boardSlots;
    private BoardSlot currentHover;

    public int TileType { get; private set; }

    public event Action OnPlaced;

    private void Awake()
    {
        canvas = FindAnyObjectByType<GameCanvas>().GetComponent<Canvas>().transform;
        tileGenerator = FindAnyObjectByType<TileGenerator>();
        boardCheck = FindAnyObjectByType<BoardCheck>();

        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(int tileType, BoardSlot[] boardSlotArr)
    {
        TileType = tileType;
        boardSlots = boardSlotArr;
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
            BoardCheck.adj[idx / 5 + 1, idx % 5 + 1] = TileType;
            boardCheck.displayedTileCount += 1;
            enabled = false;

            tileGenerator.MinusTileCount();
            boardCheck.Check();
            TurnCounting.Instance.UpdateUI();

            SoundManager.Instance.PlayDisplaySound();
            VibrationManager.Instance.Vibrate(VibrationType.Peek);

            OnPlaced?.Invoke();
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

    private void OnDestroy()
    {
        if (currentHover != null) currentHover.ResetSlotColor();
        OnPlaced = null;
    }
}
