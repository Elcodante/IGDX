using UnityEngine;

public class NPCScoreDisplay : MonoBehaviour
{
    [Header("Referensi Tampilan")]
    public ScorePopup popupPrefab;
    public Transform popupContainer;

    public void MunculkanSkor(int skor, Sprite ikon)
    {
        Debug.Log($"[DEBUG 3] NPCScoreDisplay mulai memproses skor: {skor}");

        if (popupPrefab == null)
        {
            Debug.LogError("[ERROR B] popupPrefab BELUM DIISI! Tarik prefab 'Skor_Popup' ke kolom ini di Inspector NPC.");
            return; // Berhenti di sini jika kosong
        }

        if (popupContainer == null)
        {
            Debug.LogError("[ERROR C] popupContainer BELUM DIISI! Tarik 'Wadah_Skor_Canvas' dari atas kepala NPC ke kolom ini.");
            return; // Berhenti di sini jika kosong
        }

        Debug.Log("[DEBUG 4] Prefab & Container aman. Mulai membuat kloningan UI...");
        ScorePopup popupBaru = Instantiate(popupPrefab, popupContainer);

        if (popupBaru != null)
        {
            Debug.Log("[DEBUG 5] Kloningan UI berhasil terbuat di dalam Canvas. Memanggil Setup()...");
            popupBaru.Setup(skor, ikon);
        }
    }
}