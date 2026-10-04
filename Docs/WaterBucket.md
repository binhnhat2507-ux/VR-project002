# Xô nước mẫu và đổ vào nồi

Đây là phần xô mới, tách khỏi script xô cũ đã xóa. Prefab `WaterBucket`
hiện dùng mesh miệng mở, lòng rỗng và quai cầm. Thay model sau vẫn giữ
component WaterBucket.

## Xô có sẵn trong scene

1. Mở scene `MushroomGrab_Test`: `WaterBucket` đã được lưu sẵn trong Hierarchy.
   Bấm Play là có xô, không cần dùng menu Tools hoặc tạo xô mỗi lần.
2. Prefab, mesh và material vẫn nằm trong `Assets/Gameplay`. Scene dùng một
   instance của prefab `WaterBucket`; WaterDrinkPoint tham chiếu trực tiếp tới nó.
   Muốn đổi vị trí, di chuyển instance trong Edit Mode rồi lưu scene.
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

## Nấu và ăn súp nấm

- Nộp 3 củi vào blueprint để dựng bếp. CookingPot trên prefab bếp nhận
  2 nấm + 1 lượt nước, nấu 5 giây, sau đó bấm E để ăn (+30 Hunger, tối đa 100).
- CookingPot tự tìm PlayerStats, MainCamera và xô lúc bếp xuất hiện.
  Không cần tạo CookingPotWaterPoint riêng hay gán scene reference vào prefab.
- Cầm xô đầy, đứng cách tâm vùng nhận nguyên liệu tối đa 2.5 m, bấm E để đổ.
  Xô rỗng ngay khi nồi nhận nước. Có thể bỏ nấm hoặc đổ nước trước.
- Mỗi MushroomItem chỉ tính một lần kể cả có nhiều collider hoặc bị override
  tag Untagged. Nồi đủ 2 nấm không tiêu thụ thêm nấm.
- Đủ nguyên liệu: hiện vòng tiến độ khi đứng gần. Nấu xong: vòng ẩn và hiện
  [E] An sup nam. Ăn xong reset nấm/nước để nấu lượt mới; bếp và lửa vẫn giữ.
- Prompt tự tạo trên bếp và quay theo camera. Đi xa hoặc pause thì ẩn;
  không thể đổ/ăn từ xa hay trong lúc pause. Không sinh vật phẩm thức ăn.
- CookingPotWater là điểm đổ tùy chọn cho setup cũ: gán Cooking Pot hoặc đặt
  dưới object có CookingPot, cùng Player Head và Bucket. Nước lấy trạng thái
  từ CookingPot, không giữ trạng thái riêng tách khỏi công thức.

### Kiểm tra trong Play Mode

1. Dừng Play, đợi compile rồi Play lại; dựng bếp mới bằng 3 củi.
2. Bỏ 2 nấm chưa có nước: không nấu. Nấm thứ 3 không bị tiêu thụ.
3. Cầm xô đầy, tới gần bấm E: xô rỗng, nấu 5 giây, vòng ẩn, hiện lời nhắc ăn.
4. Đi xa bấm E: không ăn. Đến gần bấm E: Hunger tăng 30, nồi về 0/2 và 0/1.
5. Lượt sau thử đổ nước trước, nấm sau. Kết quả tương tự.
6. Pause khi nấu: tiến độ dừng, không thể đổ nước hoặc ăn.

Prototype hiện hỗ trợ một xô. Cần kiểm tra vật lý và bố trí chữ trực tiếp
trong Play Mode sau thay đổi này.
