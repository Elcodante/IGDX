using System.Collections;
using UnityEngine;

public class UIanimated : MonoBehaviour
{
    public RectTransform uiElement;
    public float duration = 1f;
    public Vector2 offsetStart = new Vector2(0, -300);
    public float delay = 0f;

    private Vector2 targetPosition;
    private Coroutine animRoutine;
    private bool sudahSetup = false;

    void Awake()
    {
        if(uiElement == null)
        {
            uiElement = GetComponent<RectTransform>();
        }

        // Simpan posisi target hanya sekali di awal
        targetPosition = uiElement.anchoredPosition;
        sudahSetup = true;
    }

    private void KunciPosisiTarget()
    {
        if (sudahSetup) return; 

        if (uiElement == null)
        {
            uiElement = GetComponent<RectTransform>();
        }

        targetPosition = uiElement.anchoredPosition;
        sudahSetup = true;
    }

    void OnEnable()
    {
        KunciPosisiTarget();

        // Reset posisi
        uiElement.anchoredPosition = targetPosition + offsetStart;

        // Mulai animasi
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(AnimateWithDelay());
    }

    IEnumerator AnimateWithDelay()
    {
        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
        }
            
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            uiElement.anchoredPosition = Vector2.Lerp(targetPosition + offsetStart, targetPosition, t);
            yield return null;
        }

        uiElement.anchoredPosition = targetPosition; // pastikan akhir tepat
    }
}
