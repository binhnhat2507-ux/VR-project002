using UnityEngine;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    public const float MaxStat = 100f;

    [Header("Survival rates (points per second)")]
    [SerializeField, Min(0f)] private float hungerDrainPerSecond = 0.2f;
    [SerializeField, Min(0f)] private float thirstDrainPerSecond = 0.4f;
    [SerializeField, Min(0f)] private float healthDrainPerSecond = 2f;

    // Serialized backing fields make the current values visible in the Inspector.
    [field: SerializeField, Range(0f, MaxStat)]
    public float Health { get; private set; } = MaxStat;
    [field: SerializeField, Range(0f, MaxStat)]
    public float Hunger { get; private set; } = MaxStat;
    [field: SerializeField, Range(0f, MaxStat)]
    public float Thirst { get; private set; } = MaxStat;

    private void Awake()
    {
        Health = Hunger = Thirst = MaxStat;
        OnValidate();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        Hunger = Mathf.Clamp(Hunger - hungerDrainPerSecond * deltaTime, 0f, MaxStat);
        Thirst = Mathf.Clamp(Thirst - thirstDrainPerSecond * deltaTime, 0f, MaxStat);

        // Apply damage once, even when both hunger and thirst are empty.
        if (Hunger <= 0f || Thirst <= 0f)
            TakeDamage(healthDrainPerSecond * deltaTime);
    }

    public void AddHunger(float amount)
    {
        if (IsValidAmount(amount))
            Hunger = Mathf.Clamp(Hunger + amount, 0f, MaxStat);
    }

    public void AddThirst(float amount)
    {
        if (IsValidAmount(amount))
            Thirst = Mathf.Clamp(Thirst + amount, 0f, MaxStat);
    }

    public void Heal(float amount)
    {
        if (IsValidAmount(amount))
            Health = Mathf.Clamp(Health + amount, 0f, MaxStat);
    }

    public void TakeDamage(float amount)
    {
        if (IsValidAmount(amount))
            Health = Mathf.Clamp(Health - amount, 0f, MaxStat);
    }

    private static bool IsValidAmount(float amount)
    {
        return amount >= 0f && !float.IsNaN(amount) && !float.IsInfinity(amount);
    }

    private void OnValidate()
    {
        Health = IsValidAmount(Health) ? Mathf.Clamp(Health, 0f, MaxStat) : 0f;
        Hunger = IsValidAmount(Hunger) ? Mathf.Clamp(Hunger, 0f, MaxStat) : 0f;
        Thirst = IsValidAmount(Thirst) ? Mathf.Clamp(Thirst, 0f, MaxStat) : 0f;

        if (!IsValidAmount(hungerDrainPerSecond)) hungerDrainPerSecond = 0.2f;
        if (!IsValidAmount(thirstDrainPerSecond)) thirstDrainPerSecond = 0.4f;
        if (!IsValidAmount(healthDrainPerSecond)) healthDrainPerSecond = 2f;

        // Thirst always drains faster than hunger, including Inspector edits.
        thirstDrainPerSecond = Mathf.Max(thirstDrainPerSecond, hungerDrainPerSecond + 0.01f);
    }
}
