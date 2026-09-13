using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UITypewriter : MonoBehaviour
{
    [Tooltip("Kecepatan muncul tiap huruf (semakin kecil semakin cepat)")]
    public float kecepatanKetik = 0.02f;

    private TextMeshProUGUI teksUI;
    private Coroutine rutinitasKetik;

    private void Awake()
    {
        teksUI = GetComponent<TextMeshProUGUI>();
    }

    public void KetikTeks(string teksLengkap)
    {
        if (rutinitasKetik != null) StopCoroutine(rutinitasKetik);
        rutinitasKetik = StartCoroutine(ProsesKetik(teksLengkap));
    }

    public void BersihkanTeks()
    {
        if (teksUI == null) teksUI = GetComponent<TextMeshProUGUI>();
        teksUI.text = "";
    }

    private IEnumerator ProsesKetik(string teksLengkap)
    {
        teksUI.text = "";
        foreach (char huruf in teksLengkap)
        {
            teksUI.text += huruf;

            // Gunakan unscaledDeltaTime agar animasi ketik tetap jalan walau game di-pause (jika timeScale 0)
            yield return new WaitForSecondsRealtime(kecepatanKetik);
        }
    }
}