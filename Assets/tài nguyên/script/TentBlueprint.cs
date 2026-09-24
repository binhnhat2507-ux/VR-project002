using UnityEngine;
using TMPro; // Dùng cho TextMeshPro

public class TentBlueprint : MonoBehaviour
{
    [Header("Cài đặt Lều")]
    public int requiredWood = 5; // Số củi cần thiết
    private int currentWood = 0;
    public GameObject realTentPrefab; // Kéo Prefab lều thật vào đây
    public string woodTag = "Wood"; // Nhãn của củi để hệ thống nhận diện

    [Header("Giao diện")]
    public TextMeshPro textUI; // Hiển thị số củi (0 / 5)

    private void Start()
    {
        UpdateUI();
    }

    // Hàm này tự động kích hoạt khi có một vật thể có Rigidbody chạm vào vùng Trigger của lều
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem vật thể chạm vào có Tag là "Wood" hay không
        if (other.CompareTag(woodTag))
        {
            // 1. Phá hủy khúc củi ngay lập tức để tạo cảm giác "đã nạp nguyên liệu"
            Destroy(other.gameObject);
            
            // 2. Gọi hàm tăng số lượng củi
            AddWood();
        }
    }

    public void AddWood()
    {
        currentWood++;
        UpdateUI();
        
        // Kiểm tra xem đã đủ củi chưa
        if (currentWood >= requiredWood)
        {
            BuildRealTent();
        }
    }

    private void BuildRealTent()
    {
        // Sinh ra lều thật tại vị trí và góc quay của lều mờ
        Instantiate(realTentPrefab, transform.position, transform.rotation);
        
        // Hủy bản vẽ (lều mờ) kèm theo UI của nó
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        if (textUI != null)
        {
            textUI.text = currentWood + " / " + requiredWood;
        }
    }
}