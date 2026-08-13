using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class ScorePopup : MonoBehaviour
{
    [Header("Referensi UI")]
    public Image ikonMakanan;
    public TextMeshProUGUI teksSkor;

    [Header("Pengaturan Animasi")]
    public float durasiFade = 0.3f;
    public float durasiTampil = 1.5f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        Debug.Log("[DEBUG 6] ScorePopup Awake dipanggil. Tampilan disembunyikan (Alpha = 0).");
    }

    public void Setup(int skor, Sprite ikon)
    {
        Debug.Log("[DEBUG 7] ScorePopup menerima data. Memasukkan teks dan gambar...");

        if (teksSkor == null) Debug.LogError("[ERROR D] teksSkor BELUM DIISI di dalam Prefab ScorePopup!");
        else teksSkor.text = "+" + skor.ToString();

        if (ikonMakanan == null) Debug.LogError("[ERROR E] ikonMakanan BELUM DIISI di dalam Prefab ScorePopup!");
        else ikonMakanan.sprite = ikon;

        Debug.Log("[DEBUG 8] Data masuk. Memulai Coroutine Animasi Pudar...");
        StartCoroutine(AnimasikanPopup());
    }

    private IEnumerator AnimasikanPopup()
    {
        Debug.Log("[DEBUG 9] Animasi Fade In DIMULAI...");
        float timer = 0;
        while (timer < durasiFade)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / durasiFade);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        Debug.Log("[DEBUG 10] Animasi Fade In SELESAI. Menunggu di layar...");

        yield return new WaitForSeconds(durasiTampil);

        Debug.Log("[DEBUG 11] Waktu tampil habis. Mulai Fade Out...");
        timer = 0;
        while (timer < durasiFade)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / durasiFade);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        Debug.Log("[DEBUG 12] Animasi Selesai. Menghancurkan objek ini dari memori.");
        Destroy(gameObject);
    }
}