using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class CookingPotWater : MonoBehaviour
{
    [SerializeField] private CookingPot cookingPot;
    [SerializeField] private Transform playerHead;
    [SerializeField] private WaterBucket bucket;
    [SerializeField] private GameObject waterInsidePot;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField, Min(0.1f)] private float interactionDistance = 2.5f;

    public bool HasWater => cookingPot != null && cookingPot.HasWater;

    private void Awake()
    {
        if (cookingPot == null) cookingPot = GetComponentInParent<CookingPot>();
        if (waterInsidePot != null) waterInsidePot.SetActive(false);
        if (interactionText != null) interactionText.enabled = false;
    }

    private void Update()
    {
        if (waterInsidePot != null) waterInsidePot.SetActive(HasWater);
        bool canPour = CanPour();
        if (interactionText != null)
        {
            interactionText.enabled = canPour;
            if (canPour) interactionText.text = "[E] Do nuoc vao noi";
        }

        if (canPour && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            Pour();
    }

    // The cooking script can read HasWater before starting a recipe.
    public bool Pour()
    {
        if (!CanPour() || !cookingPot.TryPourWater(bucket)) return false;
        if (waterInsidePot != null) waterInsidePot.SetActive(true);
        return true;
    }

    private bool CanPour()
    {
        return cookingPot != null && cookingPot.CanPourWater(bucket) &&
            isActiveAndEnabled && Time.timeScale > 0f && !HasWater &&
            playerHead != null && bucket != null && bucket.IsHeld && bucket.HasWater &&
            bucket.LastFilledFrame != Time.frameCount &&
            (playerHead.position - transform.position).sqrMagnitude <=
            interactionDistance * interactionDistance;
    }

    private void OnDisable()
    {
        if (interactionText != null) interactionText.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
