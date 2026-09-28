using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class PlayerStatsUI : MonoBehaviour
{
    [Header("Player in this scene")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Survival bars")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider thirstSlider;

    [Header("Value labels (optional)")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text hungerText;
    [SerializeField] private TMP_Text thirstText;

    private void Start()
    {
        // Supports a HUD placed under the player's XR camera.
        if (playerStats == null)
            playerStats = GetComponentInParent<PlayerStats>();

        if (playerStats == null || healthSlider == null ||
            hungerSlider == null || thirstSlider == null)
        {
            Debug.LogWarning("PlayerStatsUI: Assign Player Stats and all three Sliders in the Inspector.", this);
            enabled = false;
            return;
        }

        ConfigureBar(healthSlider);
        ConfigureBar(hungerSlider);
        ConfigureBar(thirstSlider);
        if (healthText != null) healthText.raycastTarget = false;
        if (hungerText != null) hungerText.raycastTarget = false;
        if (thirstText != null) thirstText.raycastTarget = false;
        RefreshBars();
    }

    private static void ConfigureBar(Slider slider)
    {
        slider.minValue = 0f;
        slider.maxValue = PlayerStats.MaxStat;
        slider.wholeNumbers = false;
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;
        slider.navigation = new Navigation { mode = Navigation.Mode.None };

        // Status bars should not intercept controller UI rays.
        foreach (Graphic graphic in slider.GetComponentsInChildren<Graphic>(true))
            graphic.raycastTarget = false;
    }

    private void LateUpdate()
    {
        // Run after PlayerStats.Update so the bars display this frame's values.
        if (playerStats != null)
            RefreshBars();
    }

    private void RefreshBars()
    {
        // Updating the HUD must not invoke gameplay callbacks on the Sliders.
        healthSlider.SetValueWithoutNotify(playerStats.Health);
        hungerSlider.SetValueWithoutNotify(playerStats.Hunger);
        thirstSlider.SetValueWithoutNotify(playerStats.Thirst);

        UpdateValueText(healthText, playerStats.Health);
        UpdateValueText(hungerText, playerStats.Hunger);
        UpdateValueText(thirstText, playerStats.Thirst);
    }

    private static void UpdateValueText(TMP_Text label, float value)
    {
        // Round only the label; stats and bars retain their full precision.
        if (label != null)
            label.SetText("{0:0} / {1:0}", Mathf.RoundToInt(value), PlayerStats.MaxStat);
    }
}
