# Giai đoạn 2 — Ba thanh trạng thái trong tầm nhìn VR

`PlayerStatsUI` đọc Health/Hunger/Thirst từ `PlayerStats` và cập nhật ba UI Slider.
Script không tạo Canvas, không thay đổi chỉ số player, không phụ thuộc xô nước.
Scene/prefab hiện có không được sửa tự động. Thực hiện các bước sau khi đã dừng Play.

## 1. Tạo Canvas trong camera XR

Trong Hierarchy, mở **XR Origin → Camera Offset → Main Camera**
(tên Camera Offset có thể khác trong rig đang sử dụng).
Chọn đúng camera được gán trong component XR Origin.

Tạo **UI → Canvas**, đặt tên `SurvivalHUD`, rồi kéo làm con của Main Camera.
Nếu đã có Canvas dành riêng cho HUD, tái sử dụng nó.

Trên Canvas:

- Render Mode: **World Space**.
- Event Camera: kéo **Main Camera** của XR Origin vào.
- Rect Transform: Width **500**, Height **180**.
- Local Position: **X = -0.55, Y = 0.35, Z = 1** (phía trên bên trái).
- Local Rotation: **0, 0, 0**.
- Local Scale: **0.0012, 0.0012, 0.0012** (tăng 20% so với ban đầu).
- Canvas Scaler: Dynamic Pixels Per Unit **10** để bắt đầu.

Kích thước thực tương đương 0.6 × 0.216 mét, cách mặt phẳng camera 1 mét.
Vị trí này là điểm bắt đầu; điều chỉnh khi thử trong kính để đọc dễ dàng.
Canvas đi theo đầu vì nằm dưới camera; vật thể gần hơn vẫn có thể che UI.
Không dùng Screen Space Overlay cho HUD này.

## 2. Tạo ba Slider

Trong SurvivalHUD, tạo **UI → Slider**, đổi tên thành `HealthBar`.
Rect Transform: anchors ở giữa, Width **300**, Height **24**,
Pos X **60**, Pos Y **50**, Pos Z **0**, Scale **1, 1, 1**.

Trong component Slider:

- Min Value **0**, Max Value **100**, Value **100**.
- Direction: **Left To Right**.
- Bỏ chọn Interactable, Transition **None**.
- Xóa đối tượng con **Handle Slide Area** để bỏ nút kéo.
- Giữ **Background** và **Fill Area/Fill**; kiểm tra Fill Rect vẫn trỏ tới Fill.
- Đặt Fill Area stretch ngang, Left **0**, Right **0** để dùng hết chiều rộng.
- Background màu tối; Image của Fill màu đỏ.

Nhân đôi HealthBar hai lần:

| GameObject | Pos Y | Màu Image của Fill |
| --- | --- | --- |
| HealthBar | 50 | Đỏ |
| HungerBar | 0 | Cam |
| ThirstBar | -50 | Xanh lam |

Tạo ba **UI → Text - TextMeshPro** trong Canvas để ghi **Máu**, **Đói**, **Khát**.
Đặt anchors giữa, Pos X **-175**, Pos Y lần lượt **50, 0, -50**,
Width **100**, Height **36**, Font Size **24**, màu trắng, căn giữa theo chiều dọc.
Tắt Raycast Target trên các nhãn. Nếu font không hỗ trợ dấu tiếng Việt,
có thể dùng Health/Hunger/Thirst cho bản thử nghiệm.

HUD chỉ hiển thị: không cần thêm XR UI Input Module hay EventSystem mới.

## 3. Gắn script và kéo tham chiếu

Chọn SurvivalHUD → Add Component → **Player Stats UI**.

| Ô trong Inspector | Đối tượng kéo vào |
| --- | --- |
| Player Stats | XR Origin có component PlayerStats |
| Health Slider | HealthBar |
| Hunger Slider | HungerBar |
| Thirst Slider | ThirstBar |

Lưu scene trước khi Play. Nếu thiếu tham chiếu, script cảnh báo và tự tắt;
dừng Play, gán đủ tham chiếu rồi chạy lại.

## 4. Kiểm tra

1. Bật Play và chuyển sang tab **Game**: cả ba thanh ban đầu đầy.
2. Quay đầu/camera XR: HUD vẫn nằm phía trên bên trái tầm nhìn.
3. Sau khoảng 10 giây: Hunger ≈ 98, Thirst ≈ 96, Health = 100.
4. Trong Inspector của PlayerStats khi Play, kéo Hunger về 0:
   thanh đói hết, thanh máu bắt đầu giảm. Đưa Hunger lên trên 0 khi Thirst còn:
   thanh máu ngừng giảm.
5. Thử XR Grab để xác nhận HUD không chặn tương tác.

Nếu không thấy UI, kiểm tra Canvas active, nằm ở local Z dương trước đúng camera,
scale 0.0012, camera Culling Mask có layer của Canvas, và Console không báo thiếu
tham chiếu. Tab Scene là góc nhìn biên tập, không phải hình người chơi đang thấy.

## 5. Hiển thị số trên mỗi thanh

Dừng Play. Chuột phải HealthBar → UI (Canvas) → Text - TextMeshPro,
đổi tên thành HealthValue. Nếu Unity hỏi, chọn Import TMP Essentials.
Đặt HealthValue làm con trực tiếp của HealthBar, ở cuối danh sách con để chữ
vẽ trên thanh. Không đặt nó dưới Fill vì Fill thay đổi kích thước khi chỉ số giảm.

- Rect Transform: anchors ở giữa, Pos X/Y/Z = 0, Width = 300, Height = 24,
  Scale X/Y/Z = 1.
- Text: `100 / 100`, Font Size = 18, Auto Size tắt, màu trắng.
- Alignment: giữa theo cả chiều ngang và chiều dọc; Raycast Target tắt.

Tạo tương tự HungerValue dưới HungerBar và ThirstValue dưới ThirstBar
(hoặc nhân đôi cả HealthBar đã có HealthValue rồi đổi tên).
Trong Player Stats UI trên SurvivalHUD, kéo ba Text vào Health Text,
Hunger Text, Thirst Text tương ứng. Vẫn phải gán đủ Player Stats và ba Slider.
Các ô Text là tùy chọn: bỏ trống thì các thanh vẫn hoạt động như trước.

Lưu scene và Play: số cùng thanh cập nhật mỗi frame, ví dụ `98 / 100`.
Sau khoảng 10 giây với tốc độ mặc định: Health `100 / 100`, Hunger khoảng
`98 / 100`, Thirst khoảng `96 / 100`. Chữ dùng Mathf.RoundToInt để hiển thị
số nguyên gần nhất; chỉ số và thanh vẫn dùng giá trị float đầy đủ.
