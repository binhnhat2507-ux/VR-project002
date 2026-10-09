using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class WaterSource : MonoBehaviour
{
    [Header("Player references")]
    [SerializeField] private PlayerStats playerStats;
    [Tooltip("Assign the Main Camera under the player's XR Origin.")]
    [SerializeField] private Transform playerHead;
    [SerializeField] private WaterBucket bucket;

    [Header("Drinking")]
    [SerializeField, Min(0.1f)] private float interactionDistance = 3f;
    [SerializeField, Min(0f)] private float thirstPerDrink = 25f;
    [SerializeField, Min(0f)] private float drinkCooldown = 1f;

    [Header("Optional prompt on the existing HUD")]
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private string drinkMessage = "[E] Uong nuoc (+25)";

    private float nextDrinkTime;

    private void Start()
    {
        // The prototype uses one bucket; tolerate an unassigned scene reference.
        if (bucket == null) bucket = FindFirstObjectByType<WaterBucket>();
        if (interactionText != null)
            interactionText.raycastTarget = false;

        if (playerStats == null || playerHead == null)
        {
            Debug.LogWarning("WaterSource: Assign Player Stats and the XR Main Camera as Player Head.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        bool nearby = IsPlayerNearby() && Time.timeScale > 0f;
        if (interactionText != null)
        {
            interactionText.enabled = nearby;
            if (nearby)
            {
                string message = drinkMessage.Replace("[E]", SurvivalInput.UseHint);
                interactionText.text = bucket != null && bucket.IsHeld
                    ? message + " | " + (bucket.HasWater ? "Xo da day" : SurvivalInput.FillHint + " Muc nuoc vao xo")
                    : message;
            }
        }

        if (!nearby) return;
        if (SurvivalInput.UsePressed) Drink();
        if (SurvivalInput.FillPressed) FillBucket();
    }

    public bool FillBucket()
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f || !IsPlayerNearby() ||
            bucket == null || !bucket.IsHeld)
            return false;

        return bucket.Fill();
    }

    // Can also be called by an XR interaction event later.
    // Keep the range/cooldown checks here so every input uses the same rules.
    public void Drink()
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f || !IsPlayerNearby() ||
            Time.time < nextDrinkTime)
            return;

        playerStats.AddThirst(thirstPerDrink);
        nextDrinkTime = Time.time + drinkCooldown;
    }

    private bool IsPlayerNearby()
    {
        if (playerStats == null || playerHead == null)
            return false;

        // Use the tracked camera because the simulator can move it within the rig.
        return (playerHead.position - transform.position).sqrMagnitude <=
            interactionDistance * interactionDistance;
    }

    private void OnDisable()
    {
        if (interactionText != null)
            interactionText.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
