# Uống nước ở ao gần base

`WaterSource` độc lập với xô nước, terrain, gỗ và lều. Nó chỉ gọi
`PlayerStats.AddThirst(25)` khi người chơi đứng gần điểm uống và nhấn E.
UI hiện có tự cập nhật. Không tự tạo Canvas hoặc thay đổi scene.

Phím E trước đây trùng với Y Translate (đi lên) và chiều phải của Resting Hand
Axis 2D trong XR Simulator. Hai binding này đã chuyển sang Page Up trong các
Input Actions mẫu đang dùng; Q và các phím khác giữ nguyên. E dành cho uống nước.

## Thiết lập trong Unity (dừng Play trước)

1. Tạo Empty GameObject tên `WaterDrinkPoint` ở bờ ao, nơi đứng được trên đất.
   Đặt nó cao khoảng 1 mét so với mặt đất, Scale = (1, 1, 1).
   Không đặt ở tâm Plane nước lớn: khoảng cách được tính từ chính điểm này.
2. Add Component → Water Source.
3. Kéo XR Origin có PlayerStats vào ô Player Stats.
4. Kéo Main Camera dưới XR Origin vào ô Player Head.
5. Giữ Interaction Distance = 3, Thirst Per Drink = 25, Drink Cooldown = 1.
   Khoảng cách là hình cầu 3 mét tính từ camera; chọn điểm để xem gizmo trong Scene.
   Không cần Rigidbody, Collider, XR Grab hay XR Simple Interactable cho phím E.
   Nếu trước đó tạo Box Collider chỉ để làm vùng uống, có thể bỏ component đó.
6. Trong SurvivalHUD hiện có, tạo UI (Canvas) → Text - TextMeshPro,
   đặt tên WaterPrompt, làm con trực tiếp của SurvivalHUD.
   Anchor giữa, Pos = (0, -80, 0), Width = 480, Height = 30,
   Scale = (1, 1, 1), Font Size = 20, căn giữa, màu trắng.
7. Kéo WaterPrompt vào Interaction Text của WaterDrinkPoint.
   Giữ GameObject chữ active, script sẽ tự bật/tắt component chữ theo khoảng cách.
   Drink Message mặc định dùng không dấu để tương thích font:
   `[E] Uong nuoc (+25)`. Có thể đổi sang `[E] Uống nước (+25)` nếu font hỗ trợ.
   Nếu đổi Thirst Per Drink, chỉnh lại con số trong Drink Message cho phù hợp.
8. Lưu scene. Chỉ tạo một WaterSource ở ao này để mỗi lần nhấn E chỉ uống một lần.

## Kiểm tra trong Play

- Bấm vào tab Game để bàn phím điều khiển game.
- Đứng xa: không hiện lời nhắc, nhấn E không tăng Thirst.
- Đến gần bờ: hiện lời nhắc; đặt Thirst về 50 trong Inspector rồi nhấn E:
  tăng khoảng 25, thanh khát cập nhật (có hao hụt nhỏ theo thời gian).
- Giữ E không uống liên tục. Nhấn lại sau ít nhất 1 giây để uống lần tiếp theo.
- Thirst ở 90 rồi uống: tối đa 100. Hunger và Health không được cộng bởi nước.
- Rời vùng: lời nhắc biến mất. Time.timeScale = 0: không uống được.
- Thử trong kính cần gắn một nút XR gọi hàm public Drink(); phím E là bản thử trên máy.

Khoảng cách dùng Main Camera để hoạt động cả khi XR Simulator di chuyển đầu
giả lập mà XR Origin không đổi vị trí. Chưa kiểm tra đường nhìn hay mô phỏng bơi:
đặt vùng nhỏ ở bờ trống, không để vùng bao phủ xuyên qua vách/địa hình cao.
Chưa tích hợp Bucket; khi làm sau này có thể mở rộng WaterSource mà không đổi PlayerStats.
