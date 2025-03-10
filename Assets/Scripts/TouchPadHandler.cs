using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TouchPadHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Transform OfferSlot1;
    [SerializeField] private Transform OfferSlot2;
    [SerializeField] private Transform OfferSlot3;
    private float dragOffsetX = 0f; // �հ����� Ÿ���� x�� �Ÿ�
    private float dragOffsetY = 0f; // �հ������� ���ʿ��� �̵��� �Ÿ�
    public float doubleTapTime = 0.3f; // ���� ��ġ ���� �ð�
    public float slideThreshold = 100f; // �����̵� ���� �Ÿ�

    public List<Transform> tiles = new List<Transform>(); // offerslot�� �ִ� Ÿ�� ����Ʈ
    private int selectedTileIndex = 0; // ���� ���õ� Ÿ�� �ε���
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private Vector2 tileStartPos;
    private bool isDragging = false;
    private bool isSliding = false;
    private int tapCount = 0;
    private float lastTapTime = 0f;

    private void Start()
    {
        // offerslot�� ��� �ڽ�(Ÿ��) ��������
        // offerslot�� Ÿ���� ������ ����Ʈ�� �߰����� ����.. �� ���� �� TileGenerator�� ���� ����Ǿ�� ��
        AddTileList();

        // ù ��° Ÿ�� ����
        UpdateSelectedTile(0);
    }

    private void AddTileList()
    {
        if (OfferSlot1.childCount > 0) tiles.Add(OfferSlot1.GetChild(0));
        if (OfferSlot2.childCount > 0) tiles.Add(OfferSlot2.GetChild(0));
        if (OfferSlot3.childCount > 0) tiles.Add(OfferSlot3.GetChild(0));
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        float currentTime = Time.time;

        // �����̵� ������ ���� ��ġ ���� ��ġ ����
        touchStartPos = eventData.position;

        // �� ��° ��ġ���� Ȯ��
        if (currentTime - lastTapTime <= doubleTapTime)
        {
            tapCount++;
        }
        else
        {
            tapCount = 1;
        }

        lastTapTime = currentTime;

        if (tapCount < 2) return; // �� ��° ��ġ �������� �巡�� ���� X

        // �巡�� ����
        if (tiles.Count > 0)
        {
            tileStartPos = tiles[selectedTileIndex].position;
            dragOffsetX = tileStartPos.x - eventData.position.x;
            dragOffsetY = tileStartPos.y - eventData.position.y;
            tiles[selectedTileIndex].GetComponent<TileDraggable>().BeginDrag();
            isDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            isSliding = true;
            return;
        }

        // �հ����� ���� ��ġ�� ��������, Ÿ���� ���ʿ��� �̵�
        Vector2 currentTouchPos = eventData.position;
        Vector2 newTilePos = new Vector2(currentTouchPos.x + dragOffsetX, currentTouchPos.y + dragOffsetY);

        tiles[selectedTileIndex].position = newTilePos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;

            tiles[selectedTileIndex].GetComponent<TileDraggable>().EndDrag();

            // ��ġ ī��Ʈ �ʱ�ȭ (���� ��ġ ����)
            tapCount = 0;
        }
        else if(isSliding)
        {
            isSliding = false;
            // �����̵� ����
            touchEndPos = eventData.position;
            float slideDistance = touchEndPos.x - touchStartPos.x;

            if (Mathf.Abs(slideDistance) > slideThreshold)
            {
                if (slideDistance > 0)
                {
                    SelectPreviousTile();
                }
                else
                {
                    SelectNextTile();
                }
            }
        }

        isSliding = false;
    }

    private void UpdateSelectedTile(int index)
    {
        if (index == 0)
        {
            OfferSlot1.GetComponent<OfferSlot>().SelectedColor();
            OfferSlot2.GetComponent<OfferSlot>().ResetColor();
            OfferSlot3.GetComponent<OfferSlot>().ResetColor();
        }
        else if (index == 1)
        {
            OfferSlot1.GetComponent<OfferSlot>().ResetColor();
            OfferSlot2.GetComponent<OfferSlot>().SelectedColor();
            OfferSlot3.GetComponent<OfferSlot>().ResetColor();
        }
        else if (index == 2)
        {
            OfferSlot1.GetComponent<OfferSlot>().ResetColor();
            OfferSlot2.GetComponent<OfferSlot>().ResetColor();
            OfferSlot3.GetComponent<OfferSlot>().SelectedColor();
        }


        if (tiles.Count == 0) return;

        // ���õ� Ÿ���� ����
        selectedTileIndex = Mathf.Clamp(index, 0, tiles.Count - 1);
    }

    private void SelectNextTile()
    {
        UpdateSelectedTile(selectedTileIndex + 1);
    }

    private void SelectPreviousTile()
    {
        UpdateSelectedTile(selectedTileIndex - 1);
    }
}
