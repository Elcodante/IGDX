using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreUIManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public Slider progressBar;

    [Header("Referensi Data Level")]
    [Tooltip("Tarik GameObject yang memiliki LevelEndManager ke sini")]
    public LevelEndManager levelEndManager;

    [Header("Referensi Bintang")]
    public Image[] bintangImages;
    public Sprite bintangKosong;
    public Sprite bintangPenuh;

    // Variabel ini sekarang disembunyikan (private) karena angkanya akan
    // di-copy otomatis dari LevelEndManager.
    private float maksimalSkor;
    private int skorBintang1;
    private int skorBintang2;
    private int skorBintang3;

    private bool[] bintangTercapai = new bool[3];
    private float targetSkor = 0f;

    private void OnEnable()
    {
        ScoreManager.OnScoreBerubah += UpdateSkorBar;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreBerubah -= UpdateSkorBar;
    }

    private void Start()
    {
        // 1. SINKRONISASI DATA: Baca syarat menang dari LevelEndManager
        if (levelEndManager != null)
        {
            skorBintang1 = levelEndManager.skorsatuBintang;
            skorBintang2 = levelEndManager.skorduaBintang;
            skorBintang3 = levelEndManager.skortigaBintang;

            // Jadikan bintang 3 sebagai ujung akhir bar (100%)
            maksimalSkor = levelEndManager.skortigaBintang;
        }
        else
        {
            Debug.LogError("LevelEndManager belum dimasukkan ke ScoreUIManager! Bar skor tidak akan berfungsi.");
            return;
        }

        // 2. Terapkan data ke Slider
        progressBar.maxValue = maksimalSkor;
        progressBar.value = 0f;
        targetSkor = 0f;

        for (int i = 0; i < bintangImages.Length; i++)
        {
            bintangImages[i].sprite = bintangKosong;
            bintangTercapai[i] = false;
        }
    }

    private void Update()
    {
        if (progressBar.value != targetSkor)
        {
            progressBar.value = Mathf.Lerp(progressBar.value, targetSkor, Time.deltaTime * 5f);
        }
    }

    private void UpdateSkorBar(int skorBaru)
    {
        targetSkor = skorBaru;
        CekBintang(skorBaru);
    }

    private void CekBintang(int skorSekarang)
    {
        if (skorSekarang >= skorBintang1 && !bintangTercapai[0])
        {
            AktifkanBintang(0);
        }
        if (skorSekarang >= skorBintang2 && !bintangTercapai[1])
        {
            AktifkanBintang(1);
        }
        if (skorSekarang >= skorBintang3 && !bintangTercapai[2])
        {
            AktifkanBintang(2);
        }
    }

    private void AktifkanBintang(int index)
    {
        bintangTercapai[index] = true;
        bintangImages[index].sprite = bintangPenuh;

        StartCoroutine(AnimasiPopBintang(bintangImages[index].transform));
    }

    private IEnumerator AnimasiPopBintang(Transform bintangTransform)
    {
        Vector3 skalaAwal = Vector3.one;
        Vector3 skalaMembesar = new Vector3(1.5f, 1.5f, 1f);
        float durasi = 0.2f;
        float waktu = 0f;

        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            bintangTransform.localScale = Vector3.Lerp(skalaAwal, skalaMembesar, waktu / durasi);
            yield return null;
        }

        waktu = 0f;

        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            bintangTransform.localScale = Vector3.Lerp(skalaMembesar, skalaAwal, waktu / durasi);
            yield return null;
        }

        bintangTransform.localScale = skalaAwal;
    }
}