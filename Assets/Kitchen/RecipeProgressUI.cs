using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeProgressUI : MonoBehaviour
{
    [Header("UI di Canvas")]
    [SerializeField] private GameObject recipeUI;
    [SerializeField] private Button buttonStart;
    [SerializeField] private Transform indikatorContainer;
    [SerializeField] private GameObject indikatorPrefab;

    [Header("Progress Bar")]
    [SerializeField] private RectTransform progressRect;
    [SerializeField] private float rightSaatKosong = 100f;
    [SerializeField] private float rightSaatPenuh = 0f;

    [Header("Button")]
    [SerializeField] private float opacityAktif = 1f;
    [SerializeField] private float opacityTidakAktif = 0.35f;

    private CanvasGroup buttonCanvasGroup;

    private void Awake()
    {
        SetupButton();

        if (recipeUI != null)
            recipeUI.SetActive(false);
    }

    private void SetupButton()
    {
        if (buttonStart == null)
            return;

        buttonCanvasGroup = buttonStart.GetComponent<CanvasGroup>();

        if (buttonCanvasGroup == null)
            buttonCanvasGroup = buttonStart.gameObject.AddComponent<CanvasGroup>();
    }

    public void UpdateUI(
        List<IngredientData> currentIngredients,
        List<IngredientData> requiredIngredients,
        bool recipeValid)
    {
        if (recipeUI == null)
            return;

        if (currentIngredients == null || currentIngredients.Count == 0)
        {
            recipeUI.SetActive(false);
            return;
        }

        recipeUI.SetActive(true);

        UpdateButton(recipeValid);
        UpdateIndicators(currentIngredients);
        UpdateProgress(currentIngredients, requiredIngredients);
    }

    private void UpdateButton(bool recipeValid)
    {
        if (buttonStart == null)
            return;

        if (buttonCanvasGroup == null)
            SetupButton();

        buttonCanvasGroup.alpha =
            recipeValid ? opacityAktif : opacityTidakAktif;

        buttonStart.interactable = recipeValid;
    }

    private void UpdateIndicators(List<IngredientData> ingredients)
    {
        if (indikatorContainer == null || indikatorPrefab == null)
            return;

        foreach (Transform child in indikatorContainer)
        {
            if (child.GetComponent<Button>() != null)
                continue;

            Destroy(child.gameObject);
        }

        Dictionary<IngredientData, int> jumlahBahan =
            new Dictionary<IngredientData, int>();

        foreach (IngredientData bahan in ingredients)
        {
            if (jumlahBahan.ContainsKey(bahan))
                jumlahBahan[bahan]++;
            else
                jumlahBahan[bahan] = 1;
        }

        foreach (var item in jumlahBahan)
        {
            GameObject iconBaru =
                Instantiate(indikatorPrefab, indikatorContainer);

            Image iconImage =
                iconBaru.GetComponentInChildren<Image>();

            TextMeshProUGUI qtyText =
                iconBaru.GetComponentInChildren<TextMeshProUGUI>();

            if (iconImage != null)
                iconImage.sprite = item.Key.icon;

            if (qtyText != null)
                qtyText.text = "x" + item.Value;
        }
    }

    private void UpdateProgress(
        List<IngredientData> currentIngredients,
        List<IngredientData> requiredIngredients)
    {
        if (progressRect == null ||
            requiredIngredients == null ||
            requiredIngredients.Count == 0)
            return;

        int jumlahCocok = 0;

        List<IngredientData> sisaBahan =
            new List<IngredientData>(currentIngredients);

        foreach (IngredientData bahanWajib in requiredIngredients)
        {
            if (sisaBahan.Contains(bahanWajib))
            {
                sisaBahan.Remove(bahanWajib);
                jumlahCocok++;
            }
        }

        float progress =
            (float)jumlahCocok / requiredIngredients.Count;

        progress = Mathf.Clamp01(progress);

        float right =
            Mathf.Lerp(
                rightSaatKosong,
                rightSaatPenuh,
                progress
            );

        Vector2 offsetMax = progressRect.offsetMax;
        offsetMax.x = -right;
        progressRect.offsetMax = offsetMax;
    }

    public void Hide()
    {
        if (recipeUI != null)
            recipeUI.SetActive(false);
    }
}