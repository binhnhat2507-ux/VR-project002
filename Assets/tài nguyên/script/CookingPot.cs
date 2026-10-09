using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class CookingPot : MonoBehaviour
{
    [Header("Recipe")]
    [Min(1)] public int requiredMushrooms = 2;
    public bool hasWater;
    public string mushroomTag = "Mushroom";
    [Min(0.1f)] public float cookTime = 5f;

    [Header("Interaction")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Transform playerHead;
    [SerializeField] private WaterBucket bucket;
    [SerializeField, Min(0.1f)] private float interactionDistance = 2.5f;
    [SerializeField, Min(0f)] private float promptHeight = 1.2f;
    [SerializeField, Min(0f)] private float hungerPerMeal = 30f;
    [SerializeField] private TMP_Text interactionText;
    public Image pieChartUI;

    private enum PotState { WaitingForIngredients, Cooking, Done }
    private PotState state;
    private int currentMushrooms;
    private float currentCookTime;
    private float nextReferenceSearch;
    private Collider ingredientTrigger;
    private bool ownsPrompt;
    private int lastMealFrame = -1;
    private readonly HashSet<int> consumedMushrooms = new HashSet<int>();

    public bool HasWater => hasWater;
    public bool IsReady => state == PotState.Done;
    public int MushroomCount => currentMushrooms;
    // The imported stove's pivot is far from its pot; use the trigger center.
    public Vector3 InteractionPosition => ingredientTrigger != null
        ? ingredientTrigger.bounds.center : transform.position;

    private void Awake()
    {
        ingredientTrigger = GetComponent<Collider>();
        ResetPot();
    }

    private void Start()
    {
        ResolveReferences();
        if (interactionText == null)
        {
            var prompt = new GameObject("CookingPrompt");
            prompt.transform.SetParent(transform, false);
            // Keep text at a readable world size despite the imported model's scale.
            Vector3 scale = transform.lossyScale;
            prompt.transform.localScale = new Vector3(
                1f / Mathf.Max(Mathf.Abs(scale.x), 0.001f),
                1f / Mathf.Max(Mathf.Abs(scale.y), 0.001f),
                1f / Mathf.Max(Mathf.Abs(scale.z), 0.001f));
            var text = prompt.AddComponent<TextMeshPro>();
            text.fontSize = 2.5f;
            text.alignment = TextAlignmentOptions.Center;
            text.rectTransform.sizeDelta = new Vector2(2.6f, 0.8f);
            interactionText = text;
            ownsPrompt = true;
        }
        interactionText.raycastTarget = false;
        UpdateUI();
    }

    private void ResolveReferences()
    {
        if (playerStats == null) playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerHead == null && Camera.main != null) playerHead = Camera.main.transform;
        if (bucket == null) bucket = FindFirstObjectByType<WaterBucket>();
        nextReferenceSearch = Time.unscaledTime + 1f;
    }

    private void Update()
    {
        if ((playerStats == null || playerHead == null || bucket == null) &&
            Time.unscaledTime >= nextReferenceSearch) ResolveReferences();

        if (Time.timeScale > 0f)
        {
            if (state == PotState.Cooking)
            {
                currentCookTime += Time.deltaTime;
                if (currentCookTime >= Mathf.Max(0.1f, cookTime)) state = PotState.Done;
            }
            // One press performs exactly one action.
            if (SurvivalInput.UsePressed)
            {
                if (IsReady) EatSoup();
                else TryPourWater(bucket);
            }
        }
        UpdateUI();
    }

    private bool IsPlayerNearby()
    {
        return playerHead != null &&
            (playerHead.position - InteractionPosition).sqrMagnitude <=
            interactionDistance * interactionDistance;
    }

    public bool CanPourWater(WaterBucket source)
    {
        return isActiveAndEnabled && Time.timeScale > 0f && IsPlayerNearby() &&
            state == PotState.WaitingForIngredients && !hasWater &&
            lastMealFrame != Time.frameCount && source != null && source.IsHeld &&
            source.HasWater && source.LastFilledFrame != Time.frameCount;
    }

    public bool TryPourWater(WaterBucket source)
    {
        if (!CanPourWater(source) || !source.Empty()) return false;
        AddWater();
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryAddMushroom(other);
    }

    public bool TryAddMushroom(Collider other)
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f || other == null ||
            state != PotState.WaitingForIngredients || currentMushrooms >= requiredMushrooms)
            return false;

        // Recognize the item component even when a scene overrides the prefab tag.
        var item = other.GetComponentInParent<MushroomItem>();
        GameObject food = item != null ? item.gameObject :
            (other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject);
        if (!food.activeInHierarchy || (item == null && !food.CompareTag(mushroomTag)) ||
            !consumedMushrooms.Add(food.GetInstanceID())) return false;

        // Disable immediately: multiple colliders must not count as multiple mushrooms.
        food.SetActive(false);
        Destroy(food);
        currentMushrooms++;
        CheckCanCook();
        return true;
    }

    // Retained for existing Unity events and other ingredient sources.
    public void AddWater()
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f || hasWater ||
            state != PotState.WaitingForIngredients) return;
        hasWater = true;
        CheckCanCook();
    }

    private void CheckCanCook()
    {
        if (state != PotState.WaitingForIngredients ||
            currentMushrooms < requiredMushrooms || !hasWater) return;
        currentCookTime = 0f;
        state = PotState.Cooking;
    }

    public bool EatSoup()
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f || !IsReady ||
            !IsPlayerNearby() || playerStats == null) return false;
        playerStats.AddHunger(hungerPerMeal);
        lastMealFrame = Time.frameCount;
        ResetPot();
        return true;
    }

    private void ResetPot()
    {
        currentMushrooms = 0;
        hasWater = false;
        currentCookTime = 0f;
        state = PotState.WaitingForIngredients;
        consumedMushrooms.Clear();
        if (pieChartUI != null)
        {
            pieChartUI.fillAmount = 0f;
            pieChartUI.gameObject.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        bool nearby = Time.timeScale > 0f && IsPlayerNearby();
        if (pieChartUI != null)
        {
            pieChartUI.raycastTarget = false;
            pieChartUI.gameObject.SetActive(nearby && state == PotState.Cooking);
            pieChartUI.fillAmount = Mathf.Clamp01(currentCookTime / Mathf.Max(0.1f, cookTime));
        }
        if (interactionText == null) return;
        interactionText.enabled = nearby;
        if (!nearby) return;
        if (ownsPrompt)
        {
            interactionText.transform.position = InteractionPosition + Vector3.up * promptHeight;
            interactionText.transform.rotation = playerHead.rotation;
        }
        if (IsReady) interactionText.text = $"{SurvivalInput.UseHint} An sup nam (+{hungerPerMeal:0} no)";
        else if (state == PotState.Cooking)
            interactionText.text = $"Dang nau... {Mathf.CeilToInt(Mathf.Max(0f, cookTime - currentCookTime))}s";
        else
        {
            interactionText.text = $"Nam: {currentMushrooms}/{requiredMushrooms} | Nuoc: {(hasWater ? "1/1" : "0/1")}";
            if (CanPourWater(bucket)) interactionText.text += "\n" + SurvivalInput.UseHint + " Do nuoc vao noi";
        }
    }

    private void OnDisable()
    {
        if (interactionText != null) interactionText.enabled = false;
        if (pieChartUI != null) pieChartUI.gameObject.SetActive(false);
    }
}
