using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipBahan : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
     private FoodCustomizationController foodCustom;

    private void Awake()
    {
        foodCustom = GetComponentInParent<FoodCustomizationController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (FoodTooltipUI.Instance != null && foodCustom != null)
        {
            CustomizationResult result = foodCustom.GetResult();

            bool adaCustomization =
            result.manis != Tingkat.TidakAda ||
            result.lembut != Tingkat.TidakAda ||
            result.gurih != Tingkat.TidakAda ||
            result.isian != Tingkat.TidakAda;

            if (adaCustomization)
            {
                string teks = OrderTextHelper.BuatTeksCustomization(result);

                FoodTooltipUI.Instance.TampilkanTooltip(teks);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (FoodTooltipUI.Instance != null)
        {
            FoodTooltipUI.Instance.SembunyikanTooltip();
        }
    }
}