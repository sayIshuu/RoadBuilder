using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardSlot : MonoBehaviour //, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image image;
    private RectTransform rect;
    private int idx;
    private Color startColor;

    private void Awake()
    {
        //시작 색상 저장
        startColor = GetComponent<Image>().color;
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public void HighlightSlot()
    {
        image.color = new Color32(220, 255, 146, 255);
    }

    public void ResetSlotColor()
    {
        image.color = startColor;
    }

    public bool IsPositionOverSlot(Vector2 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect, position);
    }

    // 기존 OnDrop 기능을 함수화하여 직접 호출 가능하도록 변경
    public void PlaceTile(GameObject tile)
    {
        if (tile != null)
        {
            tile.transform.SetParent(transform);
            tile.GetComponent<RectTransform>().position = rect.position;
        }
    }

    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = startColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;
        }
    }
    */

    public void SetIdx(int x)
    {
        idx = x;
    }

    public int GetIdx()
    {
        return idx;
    }
}
