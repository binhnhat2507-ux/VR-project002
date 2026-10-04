using UnityEngine;

[DisallowMultipleComponent]
public class MushroomItem : MonoBehaviour
{
    [SerializeField]
    private bool isCooked = false;

    public bool IsCooked => isCooked;
    public bool CanEat => isCooked;

    // Nồi gọi hàm này khi nấu xong.
    public void Cook()
    {
        if (isCooked)
            return;

        isCooked = true;
        Debug.Log($"{gameObject.name}: Nấm đã chín, có thể ăn.", this);
    }

    // Test tạm khi chưa có hệ nấu ăn.
    [ContextMenu("Test/Nấu chín nấm")]
    private void TestCook()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("Hãy bật Play trước khi test.", this);
            return;
        }

        Cook();
    }
}