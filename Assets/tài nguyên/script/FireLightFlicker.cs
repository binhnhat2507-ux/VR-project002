using UnityEngine;

public class FireLightFlicker : MonoBehaviour
{
    [Header("Fire Light Settings")]
    public Light fireLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    
    [Tooltip("Tốc độ nhấp nháy của lửa")]
    public float flickerSpeed = 0.1f;

    [Header("Light Movement (Optional)")]
    public bool enableMovement = true;
    public float moveSpeed = 1f;
    public float moveRadius = 0.05f;

    private float baseIntensity;
    private Vector3 basePosition;
    private float randomOffset;

    void Start()
    {
        if (fireLight == null)
        {
            fireLight = GetComponent<Light>();
        }

        if (fireLight != null)
        {
            baseIntensity = fireLight.intensity;
            basePosition = fireLight.transform.localPosition;
            randomOffset = Random.Range(0f, 100f);
        }
    }

    void Update()
    {
        if (fireLight == null) return;

        // Nhấp nháy cường độ sáng (dùng Perlin Noise cho tự nhiên)
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + randomOffset, 0f);
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // Lắc nhẹ vị trí nguồn sáng tạo hiệu ứng bóng đổ chuyển động
        if (enableMovement)
        {
            float moveX = (Mathf.PerlinNoise(Time.time * moveSpeed + randomOffset, 10f) - 0.5f) * moveRadius;
            float moveY = (Mathf.PerlinNoise(10f, Time.time * moveSpeed + randomOffset) - 0.5f) * moveRadius;
            float moveZ = (Mathf.PerlinNoise(Time.time * moveSpeed + randomOffset, Time.time * moveSpeed) - 0.5f) * moveRadius;

            fireLight.transform.localPosition = basePosition + new Vector3(moveX, moveY, moveZ);
        }
    }
}
