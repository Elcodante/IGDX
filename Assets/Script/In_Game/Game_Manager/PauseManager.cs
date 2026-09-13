using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject panelSettings;
    public GameObject settingsButton;
    public GameObject tombolNavigasi;

    [Header("Pengaturan Animasi")]
    public float durasiAnimasi = 0.2f;

    public static bool isPaused { get; private set; }
    private bool sedangBeranimasi = false;

    void Start()
    {
        // Lakukan penyiapan instan tanpa animasi saat game baru dimulai
        isPaused = false;
        Time.timeScale = 1f;

        panelSettings.transform.localScale = Vector3.zero;
        panelSettings.SetActive(false);

        settingsButton.SetActive(true);
        tombolNavigasi.SetActive(true);
    }

    public void Pausegame()
    {
        if (sedangBeranimasi) return; // Jangan tumpuk perintah jika masih animasi

        isPaused = true;

        // Hentikan waktu game duluan
        Time.timeScale = 0f;

        settingsButton.SetActive(false);
        tombolNavigasi.SetActive(false);

        // Mulai animasi membesar dari 0 ke 1
        StartCoroutine(AnimasiSkalaPanel(Vector3.zero, Vector3.one, true));
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (sedangBeranimasi) return;

        isPaused = false;

        settingsButton.SetActive(true);
        tombolNavigasi.SetActive(true);

        // Mulai animasi mengecil dari 1 ke 0
        StartCoroutine(AnimasiSkalaPanel(Vector3.one, Vector3.zero, false));
        Debug.Log("Game Resumed");
    }

    private IEnumerator AnimasiSkalaPanel(Vector3 skalaAwal, Vector3 skalaTujuan, bool isMembuka)
    {
        sedangBeranimasi = true;

        if (isMembuka)
        {
            panelSettings.SetActive(true);
        }

        panelSettings.transform.localScale = skalaAwal;
        float waktu = 0f;

        while (waktu < durasiAnimasi)
        {
            // PENTING: Gunakan unscaledDeltaTime agar animasi kebal dari Time.timeScale = 0
            waktu += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, waktu / durasiAnimasi); // SmoothStep agar gerakan tidak kaku

            panelSettings.transform.localScale = Vector3.Lerp(skalaAwal, skalaTujuan, t);
            yield return null;
        }

        panelSettings.transform.localScale = skalaTujuan; // Kunci presisi di titik akhir

        if (!isMembuka)
        {
            panelSettings.SetActive(false);

            // Jendela sudah mengecil sepenuhnya, sekarang aman untuk menjalankan waktu game lagi
            Time.timeScale = 1f;
        }

        sedangBeranimasi = false;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                Pausegame();
            }
        }
    }
}