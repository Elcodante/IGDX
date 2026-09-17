using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ScoreUIManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public Slider progressBar;

    [Header("Referensi Bintang")]
    public Image[] bintangImages;
    public Sprite bintangKosong;
    public Sprite bintangPenuh;

    [Header("Target Skor untuk Bintang (Sesuai dengan LevelEndManager")]
    public float maksimalSkor = 300f; // Skor maksimal untuk mendapatkan 3 bintang
    public int skorBintang1 = 100; // Skor untuk mendapatkan 1 bintang
    public int skorBintang2 = 200; // Skor untuk mendapatkan 2 bintang
    public int skorBintang3 = 300; // Skor untuk mendapatkan 3 bintang

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
        progressBar.maxValue = maksimalSkor;
        progressBar.value = 0f;
        targetSkor = 0f;

        for(int i = 0; i < bintangImages.Length; i++)
        {
            bintangImages[i].sprite = bintangKosong;
            bintangTercapai[i] = false;
        }
    }

    private void Update()
    {
        if(progressBar.value != targetSkor)
        {
            progressBar.value = Mathf.MoveTowards(progressBar.value, targetSkor, Time.deltaTime * 5f);
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

        while(waktu < durasi)
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
