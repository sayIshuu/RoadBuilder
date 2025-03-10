using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class TouchPadHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Transform OfferSlot1;
    [SerializeField] private Transform OfferSlot2;
    [SerializeField] private Transform OfferSlot3;
    [SerializeField] private float sensitivity; // ����
    [SerializeField] private float holdThreshold = 0.2f; // ��� ������ �� �巡�� ���� �ð�
    private float dragOffsetX = 0f; // �հ����� Ÿ���� x�� �Ÿ�
    private float dragOffsetY = 0f; // �հ������� ���ʿ��� �̵��� �Ÿ�
    //public float doubleTapTime = 0.3f; // ���� ��ġ ���� �ð�
    [SerializeField] private float slideThreshold = 100f; // �����̵� ���� �Ÿ�

    public List<Transform> tiles = new List<Transform>(); // offerslot�� �ִ� Ÿ�� ����Ʈ
    private List<BoardSlot> boardSlots = new List<BoardSlot>(); // ���� ���� ����Ʈ
    private int selectedTileIndex = 0; // ���� ���õ� Ÿ�� �ε���
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private Vector2 tileStartPos;
    private bool isDragging = false;
    private bool isSliding = false;
    private bool isHolding = false; // ��� ������ ������ ����
    private float holdStartTime = 0f; // ��� ������ ������ �ð�
    //private int tapCount = 0;
    //private float lastTapTime = 0f;
    //���������� ������ ����.. ���콺 Ŀ�� �޶�����
    private BoardSlot lastHoveredSlot = null;

    private void Start()
    {
        while (tiles.Count < 3)
        {
            tiles.Add(null);
        }
        // offerslot�� ��� �ڽ�(Ÿ��) ��������
        // offerslot�� Ÿ���� ������ ����Ʈ�� �߰����� ����.. �� ���� �� TileGenerator�� ���� ����Ǿ�� ��
        //RerollTileList();

        // ù ��° Ÿ�� ����
        UpdateSelectedTile(0);

        // ���� ���� ��������
        boardSlots = new List<BoardSlot>(FindObjectsByType<BoardSlot>(FindObjectsSortMode.None));
    }

    public void RerollTileList()
    {
        StartCoroutine(RerollTileListCoroutine());
    }

    IEnumerator RerollTileListCoroutine()
    {
        yield return null; //1������ ���缭 �������� ����
        tiles.Clear();

        //none�̸� childCount�� �ִٰ� ġ�±���?
        if (OfferSlot1.childCount > 0)
        {
            tiles.Add(OfferSlot1.GetChild(0)); // ����Ʈ�� ��������Ƿ� ������ Add()
        }
        else
        {
            tiles.Add(null); // �ε����� �����Ϸ��� null �߰�
        }
        if (OfferSlot2.childCount > 0)
        {
            tiles.Add(OfferSlot2.GetChild(0));
        }
        else
        {
            tiles.Add(null);
        }
        if (OfferSlot3.childCount > 0)
        {
            tiles.Add(OfferSlot3.GetChild(0));
        }
        else
        {
            tiles.Add(null);
        }
    }

    /* �ι�Ŭ�� ����
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
            if (tiles[selectedTileIndex] == null) return;
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
        Vector2 newTilePos = new Vector2(currentTouchPos.x + dragOffsetX + ((currentTouchPos.x - touchStartPos.x) * sensitivity), 
                                    currentTouchPos.y + dragOffsetY + ((currentTouchPos.y - touchStartPos.y) * sensitivity));

        tiles[selectedTileIndex].position = newTilePos;

        DetectBoardSlot(newTilePos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            if (lastHoveredSlot != null)
            {
                // ���� ���Կ� Ÿ�� ��ġ
                lastHoveredSlot.PlaceTile(tiles[selectedTileIndex].gameObject);
            }

            if(tiles[selectedTileIndex].GetComponent<TileDraggable>().EndDrag())
            {
                // ��ġ�� tiles�� ����Ʈ���� ����
                tiles[selectedTileIndex] = null;
            }

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
                    SelectNextTile();
                }
                else
                {
                    SelectPreviousTile();
                }
            }
        }

        isSliding = false;
    }

    private void DetectBoardSlot(Vector2 tilePosition)
    {
        BoardSlot hoveredSlot = null;

        // ���� ��ȿ����.. �����丵 �ʿ�.........
        foreach (BoardSlot slot in boardSlots) // �̸� ����� ����Ʈ���� �˻�
        {
            if (slot.IsPositionOverSlot(tilePosition))
            {
                hoveredSlot = slot;
                break;
            }
        }

        if (hoveredSlot != lastHoveredSlot)
        {
            if (lastHoveredSlot != null)
            {
                lastHoveredSlot.ResetSlotColor();
            }

            if (hoveredSlot != null)
            {
                hoveredSlot.HighlightSlot();
            }

            lastHoveredSlot = hoveredSlot;
        }
    }
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStartPos = eventData.position;
        holdStartTime = Time.time; // ��ġ�� �ð� ���
        isHolding = true; // ��� ������ ����

        StartCoroutine(CheckHoldForDrag(eventData)); // ���� �ð� �� �巡�� ���� üũ
    }

    private IEnumerator CheckHoldForDrag(PointerEventData eventData)
    {
        yield return new WaitForSeconds(holdThreshold); // ���� �ð� ��ٸ�

        if (isHolding && !isSliding) // ��ġ�� �����ϰ� �����̵带 ���� �ʾ����� �巡�� ����
        {
            StartDrag(eventData);
        }
    }

    private void StartDrag(PointerEventData eventData)
    {
        if (tiles.Count > 0)
        {
            if (tiles[selectedTileIndex] == null) return;
            tileStartPos = tiles[selectedTileIndex].position;
            dragOffsetX = tileStartPos.x - eventData.position.x;
            dragOffsetY = tileStartPos.y - eventData.position.y;
            tiles[selectedTileIndex].GetComponent<TileDraggable>().BeginDrag();
            isDragging = true;
            isHolding = false; // ��� ������ �Ϸ�
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector2 currentTouchPos = eventData.position;
            Vector2 newTilePos = new Vector2(currentTouchPos.x + dragOffsetX + ((currentTouchPos.x - touchStartPos.x) * sensitivity),
                                        currentTouchPos.y + dragOffsetY + ((currentTouchPos.y - touchStartPos.y) * sensitivity));

            tiles[selectedTileIndex].position = newTilePos;
            DetectBoardSlot(newTilePos);
        }
        else if (!isDragging && isHolding) // ��� ������ ���� �ٷ� �̵��ϸ� �����̵�� ��ȯ
        {
            isHolding = false;
            isSliding = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;

        if (isDragging)
        {
            isDragging = false;
            if (lastHoveredSlot != null)
            {
                lastHoveredSlot.PlaceTile(tiles[selectedTileIndex].gameObject);
            }

            if (tiles[selectedTileIndex].GetComponent<TileDraggable>().EndDrag())
            {
                tiles[selectedTileIndex] = null;
                if (lastHoveredSlot != null)
                {
                    lastHoveredSlot.ResetSlotColor();
                }
            }
        }
        else if (isSliding)
        {
            isSliding = false;
            touchEndPos = eventData.position;
            float slideDistance = touchEndPos.x - touchStartPos.x;

            if (Mathf.Abs(slideDistance) > slideThreshold)
            {
                if (slideDistance > 0)
                {
                    SelectNextTile();
                }
                else
                {
                    SelectPreviousTile();
                }
            }
        }
    }

    private void DetectBoardSlot(Vector2 tilePosition)
    {
        BoardSlot hoveredSlot = null;

        foreach (BoardSlot slot in boardSlots)
        {
            if (slot.IsPositionOverSlot(tilePosition))
            {
                hoveredSlot = slot;
                break;
            }
        }

        if (hoveredSlot != lastHoveredSlot)
        {
            if (lastHoveredSlot != null)
            {
                lastHoveredSlot.ResetSlotColor();
            }

            if (hoveredSlot != null)
            {
                hoveredSlot.HighlightSlot();
            }

            lastHoveredSlot = hoveredSlot;
        }
    }

    private void UpdateSelectedTile(int index)
    {
        index = (index + 3) % 3;
        selectedTileIndex = index;

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
