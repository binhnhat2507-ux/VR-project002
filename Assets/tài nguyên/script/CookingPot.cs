using UnityEngine;
using UnityEngine.UI;

public class CookingPot : MonoBehaviour
{
    [Header("Yêu cầu nguyên liệu")]
    public int requiredMushrooms = 3;
    private int currentMushrooms = 0;
    public bool hasWater = false; // Xô nước (bạn khác làm) sẽ đổi biến này thành true
    public string mushroomTag = "Mushroom"; // Tag của cây nấm

    [Header("Cài đặt Nấu ăn")]
    public float cookTime = 5f; // Thời gian nấu (giây)
    private float currentCookTime = 0f;

    [Header("Giao diện Pie Chart (Canvas -> Image)")]
    public Image pieChartUI; // Kéo UI Image (Image Type = Filled, Fill Method = Radial 360) vào đây

    // Quản lý trạng thái nồi
    private enum PotState { WaitingForIngredients, Cooking, Done }
    private PotState state = PotState.WaitingForIngredients;

    private void Start()
    {
        if (pieChartUI != null)
        {
            pieChartUI.fillAmount = 0f; // Ẩn pie chart ban đầu
            pieChartUI.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Trạng thái đang nấu
        if (state == PotState.Cooking)
        {
            currentCookTime += Time.deltaTime;
            
            // Cập nhật Pie Chart UI
            if (pieChartUI != null)
            {
                pieChartUI.fillAmount = currentCookTime / cookTime;
            }

            // Nấu xong
            if (currentCookTime >= cookTime)
            {
                state = PotState.Done;
                Debug.Log("🍲 Đã nấu xong Súp Nấm! Bấm 'E' để ăn.");
            }
        }
        // Trạng thái đã nấu xong -> Đợi người chơi bấm E
        else if (state == PotState.Done)
        {
            // Tạm dùng nút E cho PC/Simulator. Sau này làm VR thật có thể dùng XR Grab Interactable để đưa bát súp lên miệng.
            if (Input.GetKeyDown(KeyCode.E))
            {
                EatSoup();
            }
        }
    }

    // Xử lý khi ném nấm vào nồi
    private void OnTriggerEnter(Collider other)
    {
        if (state == PotState.WaitingForIngredients)
        {
            if (other.CompareTag(mushroomTag))
            {
                currentMushrooms++;
                Debug.Log($"🍄 Đã thêm nấm! ({currentMushrooms}/{requiredMushrooms})");
                Destroy(other.gameObject); // Xóa cục nấm đi

                CheckCanCook();
            }
        }
    }

    // Hàm này dành cho bạn code Xô Nước gọi vào (ví dụ: pot.AddWater(); )
    public void AddWater()
    {
        if (!hasWater && state == PotState.WaitingForIngredients)
        {
            hasWater = true;
            Debug.Log("💧 Đã đổ nước vào nồi!");
            CheckCanCook();
        }
    }

    // Kiểm tra đủ nguyên liệu thì bắt đầu nấu
    private void CheckCanCook()
    {
        if (currentMushrooms >= requiredMushrooms && hasWater)
        {
            state = PotState.Cooking;
            Debug.Log("🔥 Đủ nguyên liệu, bắt đầu nấu!");
            
            if (pieChartUI != null)
            {
                pieChartUI.gameObject.SetActive(true);
                pieChartUI.fillAmount = 0f;
            }
        }
    }

    // Hàm xử lý ăn súp
    private void EatSoup()
    {
        Debug.Log("😋 Bạn đã ăn súp nấm thơm ngon! Hồi máu / Tăng thể lực...");
        
        // Reset lại nồi nếu muốn nấu tiếp
        ResetPot();
    }

    private void ResetPot()
    {
        currentMushrooms = 0;
        hasWater = false;
        currentCookTime = 0f;
        state = PotState.WaitingForIngredients;

        if (pieChartUI != null)
        {
            pieChartUI.fillAmount = 0f;
            pieChartUI.gameObject.SetActive(false);
        }
    }
}
