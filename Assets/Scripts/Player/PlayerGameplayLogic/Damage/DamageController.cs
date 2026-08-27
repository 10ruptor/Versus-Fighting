using UnityEngine;
using TMPro;
using System;

public class DamageController : MonoBehaviour
{
    [SerializeField] private float startingPercent = 0f;
    [SerializeField] private TextMeshProUGUI percentText;

    public void Initialize(TextMeshProUGUI percentText)
    {
        this.percentText = percentText;
    }

    private float currentPercent;
    public float CurrentPercent => currentPercent;

    public event Action<float> OnPercentChanged;

    private void Awake()
    {
        currentPercent = startingPercent;
    }

    private void Start()
    {
        UpdateDisplay();
    }

    public void AddDamage(float damage)
    {
        currentPercent += damage;
        UpdateDisplay();
        OnPercentChanged?.Invoke(currentPercent);
    }

    public void ResetPercent()
    {
        currentPercent = startingPercent;
        UpdateDisplay();
        OnPercentChanged?.Invoke(currentPercent);
    }

    private void UpdateDisplay()
    {
        if (!percentText) return;
        percentText.text = Mathf.RoundToInt(currentPercent) + "%";
    }
}