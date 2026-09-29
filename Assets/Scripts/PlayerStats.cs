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
    public bool IsDead { get; private set; }

    private void Awake()
    {
        Health = Hunger = Thirst = MaxStat;
        OnValidate();
        if (GetComponent<SurvivalGameOver>() == null)
            gameObject.AddComponent<SurvivalGameOver>();
    }

    private void Update()
    {
        if (IsDead) return;
        if (Health <= 0f) { Die(); return; }
        float deltaTime = Time.deltaTime;
        Hunger = Mathf.Clamp(Hunger - hungerDrainPerSecond * deltaTime, 0f, MaxStat);
        Thirst = Mathf.Clamp(Thirst - thirstDrainPerSecond * deltaTime, 0f, MaxStat);

        int emptyNeeds = (Hunger <= 0f ? 1 : 0) + (Thirst <= 0f ? 1 : 0);
        if (emptyNeeds > 0)
            TakeDamage(healthDrainPerSecond * emptyNeeds * deltaTime);
    }

    public void AddHunger(float amount)
    {
        if (!IsDead && IsValidAmount(amount))
            Hunger = Mathf.Clamp(Hunger + amount, 0f, MaxStat);
    }

    public void AddThirst(float amount)
    {
        if (!IsDead && IsValidAmount(amount))
            Thirst = Mathf.Clamp(Thirst + amount, 0f, MaxStat);
    }

    public void Heal(float amount)
    {
        if (!IsDead && IsValidAmount(amount))
            Health = Mathf.Clamp(Health + amount, 0f, MaxStat);
    }

    public void TakeDamage(float amount)
    {
        if (!IsDead && IsValidAmount(amount))
        {
            Health = Mathf.Clamp(Health - amount, 0f, MaxStat);
            if (Health <= 0f) Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        Time.timeScale = 0f;
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
