using UnityEngine;
using TMPro; // Dùng cho TextMeshPro

public class StoveBlueprint : MonoBehaviour
{
    [Header("Cài đặt Bếp")]
    public int requiredWood = 3; // Số củi cần để nhóm bếp (ví dụ: 3 khúc)
    private int currentWood = 0;
    public GameObject realStovePrefab; // Kéo Prefab bếp/đống lửa thật vào đây
    public string woodTag = "Wood"; // Nhãn nhận diện củi

    [Header("Giao diện")]
    public TextMeshPro textUI; // Hiển thị số củi (0 / 3)

    private void Start()
    {
        UpdateUI();
    }

    // Tự động nhận củi khi người chơi thả củi vào vùng Trigger của bếp
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là củi hay không
        if (other.CompareTag(woodTag))
        {
            // 1. Xóa khúc củi
            Destroy(other.gameObject);
            
            // 2. Tăng số củi đã nộp
            AddWood();
        }
    }

    public void AddWood()
    {
        currentWood++;
        UpdateUI();
        
        // Kiểm tra điều kiện hoàn thành
        if (currentWood >= requiredWood)
        {
            BuildRealStove();
        }
    }

    private void BuildRealStove()
    {
        // 1. Sinh ra bếp/đống lửa hoàn chỉnh tại vị trí và hướng quay của bản vẽ
        Instantiate(realStovePrefab, transform.position, transform.rotation);
        
        // 2. Xóa bản vẽ bếp mờ và Text UI đi
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