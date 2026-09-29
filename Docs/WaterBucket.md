# Xô nước mẫu và đổ vào nồi

Đây là phần xô mới, tách khỏi script xô cũ đã xóa. Prefab `WaterBucket`
hiện dùng mesh miệng mở, lòng rỗng và quai cầm. Thay model sau vẫn giữ
component WaterBucket.

## Tạo xô

1. Prefab có sẵn tại `Assets/Gameplay/WaterBucket.prefab`. Nếu cần tạo lại,
   dừng Play rồi chọn **Tools → Survival → Create or Update Water Bucket Prefab**.
2. Kéo prefab từ Project vào scene, đặt trên nền đất gần base. Scene
   `MushroomGrab_Test` đã có một instance xô và đã nối ô Bucket của
   WaterDrinkPoint với instance đó.
3. Prefab có sẵn Rigidbody, Box Collider đáy phẳng, XR Grab Interactable,
   WaterBucket và mặt nước bên trong. Play rồi thử nhặt xô bằng XR Grab.
4. Chọn WaterDrinkPoint bên ao, kiểm tra ô **Bucket** trỏ tới instance xô.
   Cầm xô rỗng bằng XR Grab, đến gần điểm nước: HUD hiện cả uống bằng E
   và `[Y] Muc nuoc vao xo`. Bấm Y: mặt nước xanh xuất hiện trên xô.
   E luôn uống trực tiếp, kể cả khi đang cầm xô. Y không tăng Thirst.
   Nếu ô Bucket trống, WaterSource tìm xô đầu tiên trong scene lúc Start.
5. Menu Input Selection của XR Simulator chuyển từ Y sang U để tránh trùng.

## Vật lý và kiểm tra

- Collider thân xô là hộp đơn giản (0.26, 0.34, 0.26), tâm (0, 0.17, 0).
  Đáy phẳng giúp đặt trên nền. Đây là va chạm đơn giản cho múc nước bằng phím;
  chưa mô phỏng vật nhỏ rơi vào lòng xô rỗng.
- Khi chưa cầm, WaterBucket giữ xô thẳng và khóa xoay, vẫn có trọng lực.
  Khi cầm, bỏ khóa xoay và dùng Velocity Tracking để theo tay bằng Rigidbody.
  Khi thả, đưa xô về đứng thẳng, dừng vận tốc và khóa xoay lại; không ném xô.
- Đặt đáy xô phía trên nền, không xuyên terrain. Dốc cao vẫn có thể làm xô trượt.
- Play: chờ xô rơi xuống nền phẳng; nhặt, nghiêng rồi thả vài lần và kiểm tra
  xô đứng thẳng, không tự lăn. Chưa kiểm chứng trực tiếp trong Play Mode.
- Ở ao: E chỉ tăng Thirst; Y khi cầm xô rỗng chỉ làm đầy xô. Y khi xô đầy,
  không cầm xô, ở xa ao hoặc game đang pause không làm đầy xô.

## Đổ vào nồi

1. Tạo Empty GameObject `CookingPotWaterPoint` trong scene, gần vị trí chiếc nồi
   của bếp, ở chỗ có thể đứng tới. Bếp được `StoveBlueprint` sinh tại vị trí
   bản vẽ sau khi nộp đủ gỗ, nên điểm nhận nước có thể đặt sẵn trong scene.
   Không gắn điểm này vào prefab bếp: prefab asset không thể giữ tham chiếu
   trực tiếp tới instance xô trong scene.
2. Add Component → **Cooking Pot Water**.
3. Gán Player Head = Main Camera của XR Origin, Bucket = instance xô ở scene.
4. Tạo một mặt nước nhỏ bên trong nồi, để inactive; kéo vào ô Water Inside Pot.
   Có thể dùng Cylinder màu xanh, scale khoảng (0.2, 0.002, 0.2), xóa Collider.
5. Tạo Text - TextMeshPro mới trong SurvivalHUD, ví dụ `PotPrompt`, kéo vào
   Interaction Text. Dùng chữ riêng, không dùng chung WaterPrompt của ao.
6. Lưu scene. Cầm xô đã đầy tới gần nồi, bấm E: nước trên xô tắt,
   nước trong nồi bật. Khi chưa cầm xô, xô rỗng hoặc quá xa, E không đổ.

`CookingPotWater.HasWater` là trạng thái để script nấu của nhóm đọc sau này.
Chưa có logic đun sôi hay nấu nấm. Bếp `StoveBlueprint` và cơ chế nộp gỗ
không bị thay đổi. `WaterBucket` chỉ giữ một lượt nước; nồi chỉ nhận một lượt.

Nếu dùng nhiều xô sau này, thay tham chiếu một xô trong WaterSource và
CookingPotWater bằng hệ thống tìm xô đang được cầm. Bản đầu tiên giữ một xô
để dễ kiểm tra và trình bày.
