using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodAreaSpawner3D : MonoBehaviour
{
    [Header("Cài đặt Củi")]
    public GameObject woodPrefab; // Nhớ gán Prefab có Rigidbody & XR Grab Interactable
    public int maxWoodOnMap = 10;
    public float spawnInterval = 3f;

    [Header("Cài đặt Vùng Random")]
    public Vector2 spawnAreaSize = new Vector2(30f, 30f); // Chiều rộng (X) và dài (Z) của vùng
    public LayerMask groundLayer; // Layer mặt đất (Bắt buộc phải chọn đúng)
    
    [Tooltip("Khoảng cách nâng lên so với mặt đất để củi rơi tự nhiên, tránh kẹt Mesh")]
    public float spawnHeightOffset = 0.5f; 

    private List<GameObject> activeWoodList = new List<GameObject>();

    void Start()
    {
        if (woodPrefab == null)
        {
            Debug.LogError("Chưa gán Prefab củi!");
            return;
        }
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            
            // Xóa các khúc củi đã bị nhặt/tiêu hủy ra khỏi danh sách
            activeWoodList.RemoveAll(wood => wood == null);

            // Nếu số lượng củi chưa đạt giới hạn thì sinh thêm
            if (activeWoodList.Count < maxWoodOnMap)
            {
                SpawnWoodInArea();
            }
        }
    }

    void SpawnWoodInArea()
    {
        // 1. Tính toán tọa độ ngẫu nhiên trên mặt phẳng X và Z
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

        // 2. Tạo điểm xuất phát của tia raycast (Tọa độ X, Z ngẫu nhiên, đưa lên cao Y = 50)
        Vector3 rayStartPos = transform.position + new Vector3(randomX, 50f, randomZ);

        // 3. Bắn tia thẳng xuống mặt đất
        if (Physics.Raycast(rayStartPos, Vector3.down, out RaycastHit hit, 100f, groundLayer))
        {
            // Điểm sinh củi = Điểm tia chạm đất + Nâng lên một chút để tránh xuyên đất
            Vector3 spawnPosition = hit.point + Vector3.up * spawnHeightOffset;

            // Xoay củi nằm ngang (Cylinder cần xoay 90 độ trục X hoặc Z để nằm ra sàn)
            // Trục Y xoay ngẫu nhiên từ 0 đến 360 để củi nằm lộn xộn tự nhiên
            Quaternion randomRotation = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
            
            // Tạo củi và đưa vào danh sách quản lý
            GameObject newWood = Instantiate(woodPrefab, spawnPosition, randomRotation);
            activeWoodList.Add(newWood);
        }
    }

    // Vẽ vùng sinh củi trong cửa sổ Scene (Màu xanh lá mờ)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        // Vẽ khối hộp chữ nhật dẹt đại diện cho khu vực
        Gizmos.DrawCube(transform.position, new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
    }
}