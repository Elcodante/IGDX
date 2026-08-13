using UnityEngine;
using System.Collections;

public class UIPanelSlideToggle : MonoBehaviour
{
    [Header("Target UI Panel")]
    public RectTransform panelRect;

    [Header("Pengaturan Animasi")]
    public float duration = 0.35f;       // Durasi animasi (detik)
    public float hideOffsetY = 1000f;   // Jarak meluncur ke bawah (dalam pixel)
    public bool startHidden = true;     // Apakah panel langsung tersembunyi saat game mulai?

    [Header("Kurva Gerakan (Kecepatan / Easing)")]
    // Default kurva EaseInOut agar gerakan halus di awal dan akhir
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector2 shownPosition;
    private Vector2 hiddenPosition;
    private bool isHidden;
    private bool isAnimating = false;

    void Awake()
    {
        if (panelRect == null) panelRect = GetComponent<RectTransform>();

        // 1. Rekam posisi pas di Editor sebagai posisi "Tampil" (Shown)
        shownPosition = panelRect.anchoredPosition;

        // 2. Hitung posisi "Sembunyi" (Hidden) di bawah layar
        hiddenPosition = shownPosition + new Vector2(0, -hideOffsetY);

        // 3. Atur posisi awal saat game dimulai
        if (startHidden)
        {
            panelRect.anchoredPosition = hiddenPosition;
            isHidden = true;
        }
        else
        {
            isHidden = false;
        }
    }

    // FUNGSI UTAMA: Panggil fungsi ini di OnClick Button Toggle kamu!
    public void TogglePanel()
    {
        if (isAnimating) return; // Mencegah spam klik saat animasi berjalan

        if (isHidden)
        {
            ShowPanel();
        }
        else
        {
            HidePanel();
        }
    }

    public void ShowPanel()
    {
        if (isAnimating || !isHidden) return;
        StartCoroutine(AnimateSlide(hiddenPosition, shownPosition, false));
    }

    public void HidePanel()
    {
        if (isAnimating || isHidden) return;
        StartCoroutine(AnimateSlide(shownPosition, hiddenPosition, true));
    }

    private IEnumerator AnimateSlide(Vector2 startPos, Vector2 targetPos, bool targetIsHidden)
    {
        isAnimating = true;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Evaluasi kurva biar gerakannya punya akselerasi yang mulus
            float curveValue = slideCurve.Evaluate(t);

            panelRect.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, curveValue);
            yield return null;
        }

        panelRect.anchoredPosition = targetPos;
        isHidden = targetIsHidden;
        isAnimating = false;
    }
}