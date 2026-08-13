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

    void Awake()
    {
        // Simpan posisi target hanya sekali di awal
        targetPosition = uiElement.anchoredPosition;
    }

    void OnEnable()
    {
        // Reset posisi
        uiElement.anchoredPosition = targetPosition + offsetStart;

        // Mulai animasi
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(AnimateWithDelay());
    }

    IEnumerator AnimateWithDelay()
    {
        yield return new WaitForSeconds(delay);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            uiElement.anchoredPosition = Vector2.Lerp(targetPosition + offsetStart, targetPosition, t);
            yield return null;
        }

        uiElement.anchoredPosition = targetPosition; // pastikan akhir tepat
    }
}
