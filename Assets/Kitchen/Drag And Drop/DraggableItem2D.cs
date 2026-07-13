using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public IngredientData dataBahan;
    private Collider2D col;
    private Vector3 offset;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public void SetupData(IngredientData dataBaru)
    {
        dataBahan = dataBaru;
        GetComponent<SpriteRenderer>().sprite = dataBahan.icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;
        offset = transform.position - mousePos;

        if (col != null) col.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;
        transform.position = mousePos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (col != null) col.enabled = true;
    }
}