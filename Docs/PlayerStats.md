# Giai đoạn 1 — Player Survival Stats

Script: `Assets/Scripts/PlayerStats.cs`. Tự gắn `SurvivalGameOver` để hiện màn hình kết thúc và chơi lại.

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
- Một thanh Hunger/Thirst bằng 0: giảm Health 2 điểm/giây. Cả hai bằng 0: giảm 4 điểm/giây (gấp đôi Health Drain Per Second).
- Khi cả hai được phục hồi trên 0, ngừng mất Health do đói/khát.
- `Time.timeScale = 0` sẽ tạm dừng hao hụt. Tắt/bật component không reset chỉ số.
- Health = 0: khóa trạng thái chết (`IsDead`), dừng thời gian và hiện Game Over.
  Ăn/uống/Heal không hồi sinh. Bấm CHOI LAI hoặc R tải lại scene từ đầu,
  cả ba chỉ số về 100 và thời gian trở về 1. Tiến trình dựng bếp/nấu cũng reset.

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
3. Đặt cả Hunger và Thirst = 0: Health giảm khoảng 4/giây.
4. Phục hồi cả Hunger và Thirst trên 0: Health ngừng giảm.
5. Thử lượng cộng/sát thương lớn: chỉ số không vượt 100 hoặc thấp hơn 0.
6. Dừng rồi Play lại: cả ba trở về 100. Kiểm tra grab, gỗ và lều như trước.

7. Khi Play, đặt Health = 1, Hunger = Thirst = 0 để thử Game Over nhanh.
8. Kiểm tra chuột bấm CHOI LAI và phím R riêng từng lần: scene tải lại và game chạy bình thường.
9. Trong headset, panel ở trước camera và hỗ trợ XR UI ray; cần kiểm thử trực tiếp trên thiết bị.
