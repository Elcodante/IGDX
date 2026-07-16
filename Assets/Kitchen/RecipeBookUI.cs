using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeBookUI : MonoBehaviour
{
    [System.Serializable]
    public struct RecipeInfo
    {
        public string namaMasakan;
        public Sprite iconMasakan;
        [TextArea(2, 5)] public string caraMasak; // TextArea agar kotak ketiknya lega
    }

    [Header("Referensi UI")]
    public GameObject menuPanel;          // Panel utama buku resep
    public Transform contentContainer;    // Tempat resep ngumpul 
    public GameObject recipeItemPrefab;   // Cetakan UI per resep

    [Header("Database Resep")]
    public List<RecipeInfo> daftarResep;

    private void Start()
    {
        // Pastikan menu tertutup di awal
        if (menuPanel != null) menuPanel.SetActive(false);

        // Cetak semua resep ke dalam buku
        PopulateMenu();
    }

    // Fungsi ini akan dipanggil oleh Tombol Show/Hide
    public void ToggleMenu()
    {
        if (menuPanel != null)
        {
            // Jika aktif, matikan. Jika mati, aktifkan.
            bool isActive = menuPanel.activeSelf;
            menuPanel.SetActive(!isActive);
        }
    }

    private void PopulateMenu()
    {
        if (recipeItemPrefab == null || contentContainer == null) return;

        // Bersihkan isi sebelumnya (jika ada)
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // Bikin daftar resep satu per satu ke bawah
        foreach (var resep in daftarResep)
        {
            GameObject itemBaru = Instantiate(recipeItemPrefab, contentContainer);
            itemBaru.transform.localScale = Vector3.one; 

            // Cari objeknya dulu 
            Transform iconTransform = itemBaru.transform.Find("Icon_Masakan");
            Transform teksTransform = itemBaru.transform.Find("Text_CaraMasak");

            if (iconTransform == null)
            {
                Debug.LogError("ERROR: Tidak ada objek bernama 'Icon_Masakan' di dalam Prefab Template_Resep. Cek ejaan dan huruf besarnya!");
            }
            else
            {
                Image iconUI = iconTransform.GetComponent<Image>();
                if (iconUI != null) iconUI.sprite = resep.iconMasakan;
            }

            if (teksTransform == null)
            {
                Debug.LogError("ERROR: Tidak ada objek bernama 'Text_CaraMasak' di dalam Prefab Template_Resep. Cek ejaan dan huruf besarnya!");
            }
            else
            {
                TextMeshProUGUI teksUI = teksTransform.GetComponent<TextMeshProUGUI>();
                if (teksUI != null) teksUI.text = $"<b>{resep.namaMasakan}</b>\n{resep.caraMasak}";
            }
        }
    }
}