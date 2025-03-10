using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OfferSlot : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

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
        image.color = Color.yellow;
    }

    public void ResetColor()
    {
        image.color = startColor;
    }
}
