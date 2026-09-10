using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string namaAlat; 
    private bool SetengahJadi = false;
    private Image imageTarget;
    private SpriteRenderer spriteTarget;
    private Color warnaAsli = Color.white; // Default warna jika tidak ada komponen warna[cite: 1]

    private void Awake()
    {
        imageTarget = GetComponent<Image>(); 
        if (imageTarget != null) 
        {
            warnaAsli = imageTarget.color; 
        }
        else
        {
            spriteTarget = GetComponent<SpriteRenderer>(); 
            if (spriteTarget != null) 
            {
                warnaAsli = spriteTarget.color; 
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        if (eventData.pointerDrag != null) 
        {
            UbahWarna(Color.green); 
        }
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        UbahWarna(warnaAsli); 
    }

    public void OnDrop(PointerEventData eventData) 
    {
        UbahWarna(warnaAsli);

    

        if (eventData.pointerDrag != null) 
        {
            CookingAppliance appliance = GetComponent<CookingAppliance>(); 
            if (appliance == null) return;

            DraggableApplianceUI uiAppliance = eventData.pointerDrag.GetComponent<DraggableApplianceUI>();
            if (uiAppliance != null && appliance.isStoveBase)
            {
                
                if (uiAppliance.appliancePrefab2D != null)
                {
                    // 1. Munculkan panci 2D yang asli
                    GameObject panciBaru = Instantiate(uiAppliance.appliancePrefab2D);
                    CookingAppliance panciAppliance = panciBaru.GetComponent<CookingAppliance>();

                    // 2. Coba pasangkan ke kompor
                    bool berhasilMount = appliance.MountAppliance(panciAppliance);
                    if (berhasilMount)
                    {
                        uiAppliance.isDroppedSuccessfully = true; 
                    }
                    else
                    {
                        Destroy(panciBaru);
                    }
                }
                return; // Selesai urusan pasang alat, stop sampai di sini
            }

            // --- BARU: DETEKSI CETAKAN TOOL ---
            CetakanTool cetakan = eventData.pointerDrag.GetComponent<CetakanTool>();
            if (cetakan != null)
            {
                CookingAppliance targetPenerima = appliance.GetMountedAppliance() != null ? appliance.GetMountedAppliance() : appliance;

                if (!cetakan.sudahDicelup)
                {
                    if (targetPenerima.AdaAdonanSiapPakai())
                    {
                        cetakan.CelupkanKeAdonan();
                        targetPenerima.ResetSetelahDiambil(); // Mangkuk kosong lagi, harus di-mix ulang buat batch berikutnya
                    }
                    else Debug.Log("Belum ada adonan siap di sini.");
}
                else
                {
                    targetPenerima.AddIngredient(cetakan.adonanData); // masuk ke pipeline yang SUDAH ADA
                    cetakan.ResetSetelahDigoreng(); // bukan Destroy() — alat balik sendiri lewat OnEndDrag
                }
                return;
            }

            // --- LOGIKA LAMA: DETEKSI BAHAN MASUK ---
            IngredientData bahanYangMasuk = null; 
            bool isUIItem = false;

            DraggableItem itemUI = eventData.pointerDrag.GetComponent<DraggableItem>(); 
            if (itemUI != null && itemUI.dataBahan != null) 
            {
                bahanYangMasuk = itemUI.dataBahan; 
                isUIItem = true; 
            }
            else
            {
                DraggableItem2D item2D = eventData.pointerDrag.GetComponent<DraggableItem2D>(); 
                FoodCustomizationController foodCustom = GetComponent<FoodCustomizationController>();
                if (item2D != null && item2D.dataBahan != null && foodCustom != null) 
                {
                    bahanYangMasuk = item2D.dataBahan; 
                    isUIItem = false; 
                    foodCustom.AmbilResult(item2D.customization);
                }
            }

            if (bahanYangMasuk != null) 
            {
                // Jika objek ini adalah kompor yang punya alat di atasnya, teruskan bahannya ke alat tersebut!
                CookingAppliance targetPenerima = appliance.GetMountedAppliance() != null ? appliance.GetMountedAppliance() : appliance;


                Debug.Log($"Berhasil! {bahanYangMasuk.ingredientName} dimasukkan ke {namaAlat}"); 

                
                targetPenerima.AddIngredient(bahanYangMasuk); 
                
                // Tandai berhasil drop untuk 2D
                if (!isUIItem) 
                {
                    DraggableItem2D item2D = eventData.pointerDrag.GetComponent<DraggableItem2D>();
                    if (item2D != null) item2D.isDroppedSuccessfully = true;

                    Destroy(eventData.pointerDrag.gameObject); 
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

    public void AmbilCustomFood()
    {
        SetengahJadi = true;
    }
}