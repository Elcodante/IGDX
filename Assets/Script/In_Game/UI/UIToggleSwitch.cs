using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIToggleSwitch : MonoBehaviour
{
    [Header("Referensi UI")]
    public RectTransform knobGeser;
    public Image gambarKnob;

    [Header("Pengaturan posisi (Batas kiri & kanan")]
    public float posisiXMenyala = 30f;
    public float posisiXMati = -30f;

    [Header("Pengaturan Aset Gambar")]
    public Sprite ikonMati;
    public Sprite ikonMenyala;

    [Header("Pengaturan Animasi")]
    public float durasiGeser = 0.15f;

    public bool isMenyala = true;

    private Coroutine rutinitasGeser;

    private void Start()
    {
        UpdateVisual(false);
    }

    public void KlikTogge()
    {
        isMenyala = !isMenyala;
        UpdateVisual(true);
    }

    private void UpdateVisual(bool pakaiAnimasi)
    {
        if (gambarKnob != null)
        {
            gambarKnob.sprite = isMenyala ? ikonMenyala : ikonMati;
        }

        float targetX = isMenyala ? posisiXMenyala : posisiXMati;
        Vector2 posisiTujuan = new Vector2(targetX, knobGeser.anchoredPosition.y);

        if (pakaiAnimasi)
        {
            if (rutinitasGeser != null) StopCoroutine(rutinitasGeser);
            rutinitasGeser = StartCoroutine(AnimasiGeser(posisiTujuan));
        }
        else
        {
            knobGeser.anchoredPosition = posisiTujuan;
        }
    }

    private IEnumerator AnimasiGeser(Vector2 tujuan)
    {
        Vector2 posisiAwal = knobGeser.anchoredPosition;
        float waktu = 0f;

        while (waktu < durasiGeser)
        {
            waktu += Time.unscaledDeltaTime;
            float t = waktu / durasiGeser;

            // Rumus SmoothStep agar pergeserannya melambat di akhir (tidak kaku)
            t = Mathf.SmoothStep(0f, 1f, t);

            knobGeser.anchoredPosition = Vector2.Lerp(posisiAwal, tujuan, t);
            yield return null;
        }

        knobGeser.anchoredPosition = tujuan;
    }
}
