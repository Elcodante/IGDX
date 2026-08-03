using UnityEngine;
using UnityEngine.UI;

public class UIShelfController : MonoBehaviour
{
    [Header("Referensi Panel Utama")]
    public RectTransform panelBahan;
    public RectTransform panelAlat;

    [Header("Referensi Scroll View (Objek 'View')")]
    public ScrollRect scrollBahan;
    public ScrollRect scrollAlat;

    [Header("Pengaturan Panah")]
    public float jarakScroll = 0.25f;

    public void OpenTabBahan()
    {
        if (panelBahan != null) panelBahan.SetAsLastSibling();
    }

    public void OpenTabAlat()
    {
        if (panelAlat != null) panelAlat.SetAsLastSibling();
    }

    public void ScrollBahanKeKanan()
    {
        if (scrollBahan != null)
        {
            float posisiBaru = scrollBahan.horizontalNormalizedPosition + jarakScroll;
            scrollBahan.horizontalNormalizedPosition = Mathf.Clamp01(posisiBaru);
        }
    }

    public void ScrollBahanKeKiri()
    {
        if (scrollBahan != null)
        {
            float posisiBaru = scrollBahan.horizontalNormalizedPosition - jarakScroll;
            scrollBahan.horizontalNormalizedPosition = Mathf.Clamp01(posisiBaru);
        }
    }

    public void ScrollAlatKeKanan()
    {
        if (scrollAlat != null)
        {
            float posisiBaru = scrollAlat.horizontalNormalizedPosition + jarakScroll;
            scrollAlat.horizontalNormalizedPosition = Mathf.Clamp01(posisiBaru);
        }
    }

    public void ScrollAlatKeKiri()
    {
        if (scrollAlat != null)
        {
            float posisiBaru = scrollAlat.horizontalNormalizedPosition - jarakScroll;
            scrollAlat.horizontalNormalizedPosition = Mathf.Clamp01(posisiBaru);
        }
    }
}