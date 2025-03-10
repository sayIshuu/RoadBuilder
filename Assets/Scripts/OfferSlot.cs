using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OfferSlot : MonoBehaviour, IDropHandler
{
    private RectTransform rect;
    private Image image;
    private Color startColor;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        startColor = image.color;
    }

    // 마우스 커서 -> 슬롯으로 변경해야함.
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;
        }
    }

    public void SelectedColor()
    {
        //image.color = Color.yellow;
        // C8FF56로 색맞추기
        image.color = new Color32(200, 255, 86, 255);
    }

    public void ResetColor()
    {
        image.color = startColor;
    }
}
