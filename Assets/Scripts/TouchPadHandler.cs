using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class TouchPadHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Transform OfferSlot1;
    [SerializeField] private Transform OfferSlot2;
    [SerializeField] private Transform OfferSlot3;
    [SerializeField] private float sensitivity; // 감도
    private float dragOffsetX = 0f; // 손가락과 타일의 x축 거리
    private float dragOffsetY = 0f; // 손가락보다 위쪽에서 이동할 거리
    public float doubleTapTime = 0.3f; // 더블 터치 감지 시간
    public float slideThreshold = 100f; // 슬라이드 감지 거리

    public List<Transform> tiles = new List<Transform>(); // offerslot에 있는 타일 리스트
    private List<BoardSlot> boardSlots = new List<BoardSlot>(); // 보드 슬롯 리스트
    private int selectedTileIndex = 0; // 현재 선택된 타일 인덱스
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private Vector2 tileStartPos;
    private bool isDragging = false;
    private bool isSliding = false;
    private int tapCount = 0;
    private float lastTapTime = 0f;
    //마지막으로 감지된 슬롯.. 마우스 커서 달라져서
    private BoardSlot lastHoveredSlot = null;

    private void Start()
    {
        while (tiles.Count < 3)
        {
            tiles.Add(null);
        }
        // offerslot의 모든 자식(타일) 가져오기
        // offerslot에 타일이 없으면 리스트에 추가하지 않음.. 씬 시작 시 TileGenerator가 먼저 실행되어야 함
        //RerollTileList();

        // 첫 번째 타일 선택
        UpdateSelectedTile(0);

        // 보드 슬롯 가져오기
        boardSlots = new List<BoardSlot>(FindObjectsByType<BoardSlot>(FindObjectsSortMode.None));
    }

    public void RerollTileList()
    {
        StartCoroutine(RerollTileListCoroutine());
    }

    IEnumerator RerollTileListCoroutine()
    {
        yield return null; //1프레임 늦춰서 참조꼬임 방지
        tiles.Clear();

        //none이면 childCount가 있다고 치는구나?
        if (OfferSlot1.childCount > 0)
        {
            tiles.Add(OfferSlot1.GetChild(0)); // 리스트가 비어있으므로 무조건 Add()
        }
        else
        {
            tiles.Add(null); // 인덱스를 유지하려면 null 추가
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


    public void OnPointerDown(PointerEventData eventData)
    {
        float currentTime = Time.time;

        // 슬라이드 감지를 위해 터치 시작 위치 저장
        touchStartPos = eventData.position;

        // 두 번째 터치인지 확인
        if (currentTime - lastTapTime <= doubleTapTime)
        {
            tapCount++;
        }
        else
        {
            tapCount = 1;
        }

        lastTapTime = currentTime;

        if (tapCount < 2) return; // 두 번째 터치 전까지는 드래그 시작 X

        // 드래그 시작
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

        // 손가락의 현재 위치를 가져오고, 타일을 위쪽에서 이동
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
                // 보드 슬롯에 타일 배치
                lastHoveredSlot.PlaceTile(tiles[selectedTileIndex].gameObject);
            }

            if(tiles[selectedTileIndex].GetComponent<TileDraggable>().EndDrag())
            {
                // 배치된 tiles는 리스트에서 제거
                tiles[selectedTileIndex] = null;
            }

            // 터치 카운트 초기화 (연속 터치 방지)
            tapCount = 0;
        }
        else if(isSliding)
        {
            isSliding = false;
            // 슬라이드 감지
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

    private void DetectBoardSlot(Vector2 tilePosition)
    {
        BoardSlot hoveredSlot = null;

        // 존나 비효율적.. 리팩토링 필요.........
        foreach (BoardSlot slot in boardSlots) // 미리 저장된 리스트에서 검색
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

        // 선택된 타일을 변경
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
