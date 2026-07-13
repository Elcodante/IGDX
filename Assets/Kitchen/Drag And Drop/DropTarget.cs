using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string namaAlat; 
    
    private Image imageTarget;
    private SpriteRenderer spriteTarget;
    private Color warnaAsli = Color.white; // Default warna jika tidak ada komponen warna

    private void Awake()
    {
        //cari komponen Image 
        imageTarget = GetComponent<Image>();
        if (imageTarget != null)
        {
            warnaAsli = imageTarget.color;
        }
        else
        {
            //cari SpriteRenderer 
            spriteTarget = GetComponent<SpriteRenderer>();
            if (spriteTarget != null)
            {
                warnaAsli = spriteTarget.color;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Jika sedang men-drag sesuatu, ubah warna alat jadi hijau
        if (eventData.pointerDrag != null)
        {
            UbahWarna(Color.green);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Kembalikan ke warna asli saat kursor/jari keluar dari area
        UbahWarna(warnaAsli);
    }

    public void OnDrop(PointerEventData eventData)
    {
        UbahWarna(warnaAsli); 

        if (eventData.pointerDrag != null)
        {
            IngredientData bahanYangMasuk = null;
            bool isUIItem = false; // Penanda apakah item berasal dari UI

            //Cek apakah yang masuk adalah UI
            DraggableItem itemUI = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (itemUI != null && itemUI.dataBahan != null)
            {
                bahanYangMasuk = itemUI.dataBahan;
                isUIItem = true; 
            }
            //Jika bukan UI, cek apakah yang masuk adalah 2D 
            else
            {
                DraggableItem2D item2D = eventData.pointerDrag.GetComponent<DraggableItem2D>();
                if (item2D != null && item2D.dataBahan != null)
                {
                    bahanYangMasuk = item2D.dataBahan;
                    isUIItem = false; 
                }
            }

            // Jika valid ada bahan yang masuk
            if (bahanYangMasuk != null)
            {
                Debug.Log($"Berhasil! {bahanYangMasuk.ingredientName} dimasukkan ke {namaAlat}");
                
                CookingAppliance appliance = GetComponent<CookingAppliance>();
                if (appliance != null)
                {
                    appliance.AddIngredient(bahanYangMasuk);
                    
                    if (!isUIItem)
                    {
                        // Hanya hancurkan jika itu objek 2D overworld
                        Destroy(eventData.pointerDrag.gameObject); 
                    }
                }
            }
        }
    }

    private void UbahWarna(Color warnaBaru)
    {
        if (imageTarget != null)
        {
            imageTarget.color = warnaBaru;
        }
        else if (spriteTarget != null)
        {
            spriteTarget.color = warnaBaru;
        }
    }
}