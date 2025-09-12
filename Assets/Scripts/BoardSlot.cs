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
        startColor = GetComponent<Image>().color;
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public void HighlightSlot()
    {
        startColor = image.color;
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
