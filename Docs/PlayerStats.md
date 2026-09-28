# Giai đoạn 1 — Player Survival Stats

Script: `Assets/Scripts/PlayerStats.cs`. Không phụ thuộc xô nước, XR Toolkit,
UI, nấm, gỗ hay lều. Chưa triển khai giai đoạn 2–5.

## Gắn vào player

1. Mở scene gameplay đang sử dụng trong Unity.
2. Chọn GameObject gốc **XR Origin** của người chơi.
3. Add Component → **Player Stats**, chỉ gắn một lần trên player.
4. Bật Play. Health, Hunger và Thirst bắt đầu từ 100.

Chưa tự gắn component vào scene/prefab; scene có thay đổi chưa commit nên bước
này thực hiện trong Editor để bảo toàn trạng thái đang làm việc.

## Logic và cấu hình

- Hunger Drain Per Second: mặc định 0.2 điểm/giây.
- Thirst Drain Per Second: mặc định 0.4 điểm/giây, luôn lớn hơn Hunger.
- Health Drain Per Second: mặc định 2 điểm/giây.
- Mỗi frame giảm Hunger/Thirst theo `Time.deltaTime`, giới hạn trong 0–100.
- Khi Hunger hoặc Thirst bằng 0, giảm Health một lần mỗi frame.
- Khi cả hai được phục hồi trên 0, ngừng mất Health do đói/khát.
- `Time.timeScale = 0` sẽ tạm dừng hao hụt. Tắt/bật component không reset chỉ số.
- Chưa có Game Over hoặc tự hồi máu. `Heal()` vẫn dùng được khi Health = 0.

Các script khác đọc `Health`, `Hunger`, `Thirst`, và gọi các hàm sau trên
component PlayerStats của đúng người chơi:

```csharp
playerStats.AddHunger(25f);
playerStats.AddThirst(30f);
playerStats.Heal(10f);
playerStats.TakeDamage(15f);
```

Mỗi hàm bỏ qua số âm, NaN và Infinity; kết quả luôn giới hạn 0–100.
Inspector hiển thị giá trị hiện tại để kiểm tra; chỉnh chỉ số ở Edit Mode
không thay đổi yêu cầu khởi đầu 100 khi vào Play.

## Kiểm tra trong Play Mode

1. Với mặc định, sau khoảng 10 giây: Health ≈ 100, Hunger ≈ 98, Thirst ≈ 96.
2. Đặt Hunger = 0 trong Inspector khi đang Play: Health giảm khoảng 2/giây.
3. Đặt cả Hunger và Thirst = 0: Health vẫn chỉ giảm khoảng 2/giây.
4. Phục hồi cả Hunger và Thirst trên 0: Health ngừng giảm.
5. Thử lượng cộng/sát thương lớn: chỉ số không vượt 100 hoặc thấp hơn 0.
6. Dừng rồi Play lại: cả ba trở về 100. Kiểm tra grab, gỗ và lều như trước.
