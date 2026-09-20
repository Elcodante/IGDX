using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI Popup Tutorial")]
    public GameObject panelTutorial;
    public Image gambarTutorial;
    public TextMeshProUGUI teksTutorial;
    public Button tombolMengerti;

    [System.Serializable]
    public struct TutorialData
    {
        public CookingMechanicType mekanik;
        public Sprite gambar;
        [TextArea(2, 4)] public string teks;
    }

    [Header("Database Tutorial per Mekanik")]
    public TutorialData[] daftarTutorial;

    private Action onTutorialClosedCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (panelTutorial != null) panelTutorial.SetActive(false);
        if (tombolMengerti != null) tombolMengerti.onClick.AddListener(TutupTutorial);
    }

    private const string PREFIX_KEY = "TutorialSeen_";

    public bool SudahPernahLihat(CookingMechanicType mekanik)
    {
        return PlayerPrefs.GetInt(PREFIX_KEY + mekanik.ToString(), 0) == 1;
    }

    private void TandaiSudahLihat(CookingMechanicType mekanik)
    {
        PlayerPrefs.SetInt(PREFIX_KEY + mekanik.ToString(), 1);
        PlayerPrefs.Save();
    }

    // Panggil ini dari CookingAppliance sebelum StartMinigame beneran jalan
    public void TampilkanTutorialJikaPerlu(CookingMechanicType mekanik, Action lanjutkanAksi)
    {
        if (SudahPernahLihat(mekanik))
        {
            lanjutkanAksi?.Invoke(); // langsung lanjut, gak perlu tutorial lagi
            return;
        }

        TutorialData data = default;
        bool ketemu = false;
        foreach (var t in daftarTutorial)
        {
            if (t.mekanik == mekanik) { data = t; ketemu = true; break; }
        }

        if (!ketemu)
        {
            Debug.LogWarning($"[TUTORIAL] Gak ada data tutorial untuk mekanik {mekanik}, skip tutorial.");
            TandaiSudahLihat(mekanik);
            lanjutkanAksi?.Invoke();
            return;
        }

        onTutorialClosedCallback = lanjutkanAksi;

        if (gambarTutorial != null) gambarTutorial.sprite = data.gambar;
        if (teksTutorial != null) teksTutorial.text = data.teks;
        if (panelTutorial != null) panelTutorial.SetActive(true);

        TandaiSudahLihat(mekanik); // ditandai begitu ditampilkan, bukan begitu ditutup
    }

    private void TutupTutorial()
    {
        if (panelTutorial != null) panelTutorial.SetActive(false);

        onTutorialClosedCallback?.Invoke();
        onTutorialClosedCallback = null;
    }
    
    [ContextMenu("Reset Semua Tutorial")]
    public void ResetSemuaTutorial()
    {
        foreach (CookingMechanicType mekanik in System.Enum.GetValues(typeof(CookingMechanicType)))
        {
            PlayerPrefs.DeleteKey(PREFIX_KEY + mekanik.ToString());
        }
        PlayerPrefs.Save();
        Debug.Log("Semua status tutorial di-reset.");
    }
}