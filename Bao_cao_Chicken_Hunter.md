# MỤC LỤC

1. Giới thiệu chung

2. Cốt truyện (Story)

3. Cơ chế game (Core Game Mechanics)

4. Phong cách nghệ thuật (Art Style)

5. Thế giới game (Game World)

6. Thể loại game (Genre)

7. Màn hình và chuyển cảnh (Screens & Transitions)

8. Lựa chọn công nghệ và kỹ thuật phát triển

9. Kết luận

# 1. GIỚI THIỆU CHUNG

Báo cáo này trình bày quá trình phân tích và thiết kế trò chơi di động 2D mang tên “Chicken Hunter” — một game thuộc thể loại Arcade & Shooting kết hợp yếu tố Casual, được xây dựng trên nền tảng Android. Nội dung báo cáo bao gồm: xác định cơ chế lõi (core game mechanics), xây dựng cốt truyện và phong cách nghệ thuật, thiết kế các màn hình và luồng chuyển cảnh (screens & transitions), sau đó mới đến lựa chọn công cụ, ngôn ngữ và API kỹ thuật để hiện thực hoá trò chơi.

## 1.1. Mục tiêu báo cáo

Xác định rõ ý tưởng, cốt truyện và mục đích chơi (ultimate purpose) của game.

Mô tả chi tiết cơ chế game (game mechanics) — luật chơi, đối tượng, hành vi, tương tác.

Thiết kế phong cách nghệ thuật (art style) và thế giới game (game world).

Xác định thể loại (genre) và các tựa game tham chiếu cùng thể loại.

Thiết kế toàn bộ màn hình (screens) và sơ đồ chuyển cảnh (transitions).

Lựa chọn công cụ phát triển: game engine, framework, ngôn ngữ lập trình, thư viện, tài nguyên (asset).

Liệt kê các thành phần và API của Unity Engine sẽ được sử dụng để hiện thực từng chức năng của game.

## 1.2. Lý do lựa chọn đề tài

Ngành công nghiệp game di động là một trong những ngành tăng trưởng nhanh nhất thế giới, với doanh thu 77,2 tỷ USD năm 2020 và hơn 2,5 tỷ người chơi toàn cầu (SocialPeta, 2020). Trong đó, thể loại Casual và Arcade chiếm tỷ trọng lớn nhất trên Google Play nhờ đặc tính dễ tiếp cận, dễ chơi, phù hợp với mọi đối tượng người dùng — hoàn toàn phù hợp để sinh viên thực hành xây dựng một game hoàn chỉnh bằng Unity Engine trong khuôn khổ môn học.

# 2. CỐT TRUYỆN (STORY)

Chicken Hunter được xây dựng với một cốt truyện đơn giản, hài hước, chỉ đóng vai trò dẫn nhập và tạo động lực chơi — không cần phức tạp về mặt tường thuật.

## 2.1. Bối cảnh

“Một trang trại nhỏ yên bình bỗng một ngày bị bầy gà nổi loạn xâm chiếm. Đàn gà không rõ nguyên nhân bỗng trở nên hung dữ, chạy nhảy khắp nơi và phá hoại mùa màng. Người nông dân — nhân vật chính của chúng ta — phải cầm súng bắn gà (chicken gun) đứng canh giữ trang trại, tiêu diệt hoặc bắt lại từng con gà trước khi chúng phá sạch ruộng vườn. Càng bắn trúng nhiều gà, phần thưởng và điểm số càng cao, giúp người nông dân mua thêm vũ khí và trang bị mới để đối phó với những đàn gà ngày càng đông và nguy hiểm hơn.”

## 2.2. Nhân vật

## 2.3. Mục đích (Purpose)

Mục tiêu chính của người chơi là bảo vệ trang trại bằng cách tiêu diệt toàn bộ các đợt gà xuất hiện, vượt qua các level và đánh bại Boss cuối màn. Người chơi phải quản lý số mạng, thời gian và tài nguyên để đạt điểm số cao nhất có thể

Chicken Hunter hướng tới ba yếu tố tạo động lực chơi cho người dùng:

Thúc đẩy người chơi (Motivating players): leo hạng trên bảng xếp hạng điểm số cao (high score), vượt qua các level ngày càng khó.

Phần thưởng (Being rewarded): kiếm xu/điểm để mua vũ khí mới (súng máy, súng phóng lựu, đạn xuyên giáp), mở khoá skin trang trại.

Khơi gợi sự tò mò (Promote curiosity): các loại gà mới, boss mới, trang trại mới xuất hiện dần theo tiến trình chơi.

# 3. CƠ CHẾ GAME (CORE GAME MECHANICS)

“Game mechanics là cấu trúc của luật chơi trong game — mọi thứ diễn ra trong game, và ở phía game là những gì game có thể làm và làm như thế nào.” Dưới đây là bốn câu hỏi cốt lõi được áp dụng cho Chicken Hunter.

### Chi tiết các bước trong Game Loop:

START (Bắt đầu): Khởi tạo màn chơi, cài đặt các thông số ban đầu (Máu, Điểm số, Wave 1).

SPAWN CHICKENS (Sinh gà): Xuất hiện đàn gà theo từng nhóm/khu vực định sẵn.

AIM / TOUCH (Ngắm / Chạm): Người chơi vuốt hoặc chạm màn hình để xác định hướng ngắm.

SHOOT (Bắn): Nhấn nút Fire hoặc chạm để phóng đạn về phía mục tiêu.

HIT? (Kiểm tra trúng mục tiêu):

❌ No (Trượt): Bỏ qua trạng thái trừ máu, tiếp tục vòng lặp ngắm/bắn.

Yes (Trúng): Trừ lượng máu tương ứng của gà (Chicken HP ↓).

Dead? (Gà bị tiêu diệt?): Nếu máu về 0, kích hoạt hiệu ứng biến mất và cộng điểm thưởng (SCORE +).

All Chickens Dead? (Hạ hết gà?): Kiểm tra xem đã dọn sạch gà trong đợt hiện tại hay chưa.

Next Wave / Boss Wave: Chuyển tiếp qua các Wave khó hơn cho tới khi gặp Boss Wave cuối cùng.

VICTORY (Chiến thắng): Tiêu diệt Boss thành công, hoàn thành màn chơi và lưu kỷ lục điểm cao.

## 3.1. Đối tượng trong thế giới game

## 3.2. Luật chơi chi tiết (Rules)

Tương tự cách trình bày cơ chế của game Snake trong slide (“Snake's Core Game Mechanics”), luật chơi của Chicken Hunter được liệt kê rõ ràng như sau:

Thế giới game được chia thành các vùng xuất hiện (spawn zone) cố định — gà xuất hiện ngẫu nhiên tại các vùng này theo chu kỳ thời gian (spawn timer).

Mỗi con gà di chuyển theo một hướng/pattern riêng (đường thẳng, zig-zag, hoặc bay theo hình sin) tuỳ loại.

Người chơi chạm vào màn hình để ngắm/nhắm, chạm hoặc giữ để bắn đạn về phía vị trí chạm.

Khi đạn va chạm với gà: trừ HP của gà; nếu HP về 0, gà biến mất, cộng điểm và có tỉ lệ % rơi power-up.

Nếu gà chạm tới hàng rào/ranh giới dưới màn hình (nghĩa là “thoát” khỏi vùng bắn), người chơi bị trừ 1 mạng (life).

Hết đạn (nếu có giới hạn đạn) → cần chờ nạp đạn (reload) hoặc dùng xu mua thêm đạn.

Hết mạng (life = 0) hoặc hết thời gian màn chơi (tuỳ chế độ) → chuyển sang trạng thái Game Over.

Mỗi level tăng dần độ khó: tăng tốc độ gà, tăng tần suất spawn, xuất hiện thêm loại gà mới.

Cuối mỗi level xuất hiện 1 Boss gà — cần nhiều phát bắn hơn và có thể tự bắn/tấn công lại người chơi.

## 3.3. Touch Control

## 3.4. Tham số cân bằng (Balancing Parameters)

Theo nguyên tắc “Parameters indicate the diversity and balance of game mechanics” (Module 2), các đối tượng gà tuy cùng chia sẻ một cơ chế (di chuyển – bị bắn – biến mất) nhưng được phân biệt qua tham số:

## 3.5. Trạng thái trò chơi (Game States)

Màn chơi (Game Screen) có 4 trạng thái chính, tương tự mô hình trạng thái của game Snake/Mr. Nom trong Module 2:

Ready — hiển thị “Chạm màn hình để bắt đầu”.

Running — vòng lặp game đang chạy, gà xuất hiện, người chơi bắn.

Paused — người chơi nhấn nút Pause, game tạm dừng, hiện menu Resume/Quit.

Game Over — hiển thị điểm số cuối cùng, so sánh với điểm cao nhất, nút chơi lại.

# 4. PHONG CÁCH NGHỆ THUẬT (ART STYLE)

Phong cách hình ảnh của Chicken Hunter được xây dựng dựa trên cốt truyện vui nhộn, nhẹ nhàng — hướng tới phong cách hoạt hình (cartoon) tươi sáng, dễ chịu cho mọi lứa tuổi.

## 4.1. Phong cách đồ hoạ

Phong cách 2D cartoon, màu sắc tươi sáng, đường nét bo tròn, biểu cảm hài hước.

Gà được thiết kế với các biểu cảm khác nhau (ngơ ngác, tức giận, hoảng loạn khi bị bắn trúng) để tăng tính giải trí.

Bối cảnh trang trại: đồng cỏ xanh, hàng rào gỗ, bầu trời có mây hoạt hình, có thể đổi theo thời gian trong ngày (sáng/chiều/tối).

Hiệu ứng (particle effect) lông gà bay ra khi bắn trúng, hiệu ứng loé sáng khi bắn súng.

## 4.2. Âm thanh

Âm thanh bắn súng vui nhộn, không quá bạo lực (phù hợp phong cách cartoon).

Tiếng gà kêu hài hước khi bị bắn trúng hoặc khi trốn thoát.

Nhạc nền vui tươi, nhịp độ nhanh, tạo cảm giác hồi hộp nhẹ nhàng khi chơi.

Âm thanh thông báo khi lên level mới, khi đạt high score, khi thua cuộc.

# 5. THẾ GIỚI GAME (GAME WORLD)

## 5.1. Cấu trúc thế giới

Thế giới 2D, chia theo các lớp (layer) tạo chiều sâu: lớp nền trời/mây (xa), lớp cây cối/hàng rào (giữa), lớp đất/sân trang trại nơi gà xuất hiện (gần).

Camera cố định (không cuộn) — toàn bộ hành động diễn ra trong khung hình duy nhất, phù hợp thao tác chạm trên màn hình di động.

Vùng xuất hiện gà (spawn zone) được chia thành 3–5 khu vực cố định để kiểm soát độ khó.

# 6. THỂ LOẠI GAME (GENRE)

Dựa trên phân loại các thể loại phổ biến trên Google Play được trình bày trong Module 1 (Casual Games, Puzzle Games, Arcade & Action Games, Tower-Defense Games), Chicken Hunter được xác định thuộc nhóm:

Thể loại chính — Arcade & Action / Shooting: gameplay dựa trên phản xạ, bắn súng tĩnh (on-rail shooter), tương tự tinh thần của các game Arcade cổ điển.

Yếu tố phụ — Casual Game: dễ tiếp cận, dễ chơi (easy-to-access, easy-to-play), phiên chơi ngắn (vài phút/level), phù hợp mọi loại người chơi — tương tự đặc điểm của Temple Run.

Bảng so sánh với các game tham chiếu cùng thể loại (đã đề cập trong Module 1):

# 7. MÀN HÌNH VÀ CHUYỂN CẢNH (SCREENS & TRANSITIONS)

Áp dụng nguyên tắc trong Module 2: “A screen is an atomic unit that fills the entire display, and it is responsible for exactly one part of the game.” Chicken Hunter gồm các màn hình sau:

## 7.1. Danh sách màn hình

## 7.2. Sơ đồ chuyển cảnh (Transition Flow)

Sơ đồ luồng chuyển cảnh giữa các màn hình (mô phỏng theo cách trình bày trong Module 2 đối với game Mr. Nom):

Splash Screen

└─> Main Menu

Nút “Play” → Level Select → (chọn level) → Game Screen (state = Ready)

Nút “Highscores” → Highscores Screen → nút Back → Main Menu

Nút “Shop” → Shop Screen → nút Back → Main Menu

Nút “Settings” → Settings Screen → nút Back → Main Menu

Trong Game Screen: chạm màn hình (Ready) → chuyển sang Running

Running → nhấn nút Pause → hiện Pause Overlay (state = Paused)

Pause Overlay → “Resume” → quay lại Running; “Restart” → reset về Ready; “Quit” → Main Menu

Running → hết mạng/thắng level → chuyển sang Game Over Screen

Game Over Screen → “Retry” → Game Screen (Ready); “Menu” → Main Menu

Hình 1. Sơ đồ trực quan luồng màn hình và chuyển cảnh của Chicken Hunter

## 7.3. Nguyên tắc thiết kế màn hình

Mỗi màn hình có ít nhất một thành phần tương tác (nút bấm) để kích hoạt chuyển cảnh, đúng nguyên tắc trong Module 2 (“Each screen has at least one interactive component to trigger a transition to another screen”).

Các thành phần hình ảnh của từng màn hình được tách thành các asset riêng biệt (button, background, icon...) để dễ dàng thay đổi/tái sử dụng.

Có thể tồn tại “màn hình trong màn hình” (screens within screens) — ví dụ Pause Overlay hiển thị đè lên Game Screen mà không phá huỷ trạng thái game hiện tại.

# 8. LỰA CHỌN CÔNG NGHỆ VÀ KỸ THUẬT PHÁT TRIỂN

## 8.1. Game Engine / Framework

Cân nhắc giữa các lựa chọn được liệt kê trong Module 1 (“Engines & Frameworks”):

→ Lựa chọn cuối cùng: phát triển Chicken Hunter bằng Unity3D (chế độ 2D) — tận dụng hệ thống Scene, GameObject/Prefab, Rigidbody2D/Collider2D có sẵn để xử lý va chạm, cùng Animator và Particle System để tạo hiệu ứng hình ảnh sinh động cho gà và đạn, giúp rút ngắn đáng kể thời gian hiện thực hoá so với việc tự viết toàn bộ vòng lặp vẽ bằng Canvas.

## 8.2. Ngôn ngữ lập trình

C# — ngôn ngữ lập trình chính thức và duy nhất cho scripting logic trong Unity (kế thừa từ MonoBehaviour).

Có thể sử dụng thêm ShaderLab/HLSL nếu cần tuỳ biến shader cho hiệu ứng ánh sáng ban đêm (Module “Thế giới game”)

## 8.3. Thư viện / Asset

## 8.4. Các thành phần & API của Unity sử dụng theo từng chức năng

Bảng dưới đây ánh xạ các thành phần (Component) và API tiêu biểu của Unity vào từng chức năng cụ thể của Chicken Hunter, thay thế cho các Android API gốc đã đề cập trong Module “Android APIs for 2D Game Programming” — do phần này hiện được Unity Engine đảm nhiệm bên dưới:

## 8.5. Kiến trúc project trong Unity

Cấu trúc thư mục Assets được tổ chức khoa học, thay thế cho cấu trúc AndroidManifest/res của Android Studio thuần, nhưng vẫn tuân theo nguyên tắc phân tách tài nguyên đã nêu ở Module 1 (“Setting Up the Development Environment”):

Assets/Scenes/ — chứa các Scene tương ứng với từng nhóm màn hình: SplashScene, MainMenuScene, GameScene, ShopScene...

Assets/Scripts/ — chứa toàn bộ mã nguồn C#: PlayerShooter.cs, Chicken.cs, Bullet.cs, GameManager.cs, SpawnManager.cs, UIManager.cs, ScoreManager.cs...

Assets/Sprites/ — chứa hình ảnh nhân vật, gà, vũ khí, nền, nút bấm (được cắt bằng Sprite Editor).

Assets/Animations/ — chứa Animator Controller và Animation Clip cho các đối tượng có chuyển động (gà đi, gà trúng đạn, súng bắn).

Assets/Audio/ — chứa AudioClip cho SFX và nhạc nền.

Assets/Prefabs/ — chứa các Prefab tái sử dụng: Chicken_Normal, Chicken_Fast, Chicken_Armored, Chicken_Boss, Bullet, PowerUp...

Assets/UI/ — chứa Canvas, TextMeshPro font asset, sprite dành riêng cho giao diện.

ProjectSettings/ — cấu hình Player Settings (icon, tên gói, độ phân giải), Input System, Physics 2D.

# 9. KẾT LUẬN

Báo cáo đã trình bày đầy đủ quy trình thiết kế game Chicken xuất phát từ ý tưởng cốt truyện và cơ chế lõi ở mức cao (không đi vào chi tiết kỹ thuật ngay), sau đó mới xây dựng phong cách nghệ thuật, thế giới game, hệ thống màn hình/chuyển cảnh, và cuối cùng là lựa chọn công cụ - công nghệ - API cụ thể để hiện thực hoá. Việc tuân thủ trình tự “Define high-level aspects → Design graphics & basic building blocks → Design screens and transitions” giúp đảm bảo trò chơi có định hướng rõ ràng trước khi bắt tay vào lập trình, tránh tình trạng “game phát triển không có định hướng hành vi rõ ràng” như đã cảnh báo trong phần Problem Statement.

Với nền tảng thiết kế này, nhóm phát triển có thể tiến hành hiện thực hoá Chicken Hunter trên nền tảng Unity3D bằng ngôn ngữ C#, tận dụng các thành phần có sẵn của engine như Rigidbody2D/Collider2D cho va chạm, Animator cho hoạt hình, AudioSource cho âm thanh, PlayerPrefs để lưu trữ điểm cao nhất và cấu hình người chơi, cùng hệ thống UI Canvas/TextMeshPro để xây dựng toàn bộ menu và HUD — giúp rút ngắn thời gian phát triển so với việc tự viết game engine thuần Android, đồng thời vẫn bám sát các nguyên lý thiết kế cơ chế, màn hình và chuyển cảnh đã học trong môn Phát triển Game Di động.

| Nhân vật | Mô tả | Vai trò trong gameplay |

| --- | --- | --- |

| Người nông dân (Player) | Đứng cố định ở cạnh dưới màn hình, cầm súng/súng cao su. | Nhân vật do người chơi điều khiển — nhắm và bắn. |

| Gà thường (Normal Chicken) | Di chuyển ngẫu nhiên, tốc độ trung bình, 1 máu. | Mục tiêu cơ bản, cho điểm thấp. |

| Gà nhanh (Speedy Chicken) | Di chuyển nhanh, hay đổi hướng đột ngột. | Tăng độ khó, cho điểm cao hơn. |

| Gà giáp (Armored Chicken) | Có "giáp" (mào sắt), cần bắn 2–3 phát mới hạ. | Mục tiêu khó, xuất hiện ở level cao. |

| Gà Boss (Big Boss Chicken) | Xuất hiện cuối mỗi màn, máu lớn, di chuyển và tấn công phức tạp. | Boss cuối màn, mở khoá phần thưởng lớn. |

| Đối tượng | Kích thước/Vị trí | Thuộc tính chính |

| --- | --- | --- |

| Người chơi (tầm ngắm/súng) | Cố định hoặc trượt ngang cạnh dưới màn hình | Vị trí ngắm (x, y), loại vũ khí, số đạn |

| Gà (nhiều loại) | Sinh ngẫu nhiên trong vùng chơi (grid/vùng toạ độ) | HP, tốc độ, hướng di chuyển, điểm thưởng |

| Đạn (Bullet) | Bắn ra từ vị trí ngắm | Vận tốc, sát thương, hình ảnh |

| Vật cản/hàng rào | Cố định trong trang trại | Chặn gà hoặc chặn đạn (tuỳ thiết kế) |

| HUD (điểm số, thời gian, mạng) | Cố định trên/dưới màn hình | Hiển thị dữ liệu thời gian thực |

| Thao tác | Chức năng |

| --- | --- |

| Tap | Bắn |

| Drag | Di chuyển hướng ngắm |

| Hold Fire | Bắn liên tục |

| Pause | Tạm dừng |

| Swipe trái/phải | Di chuyển Player nếu áp dụng |

| Loại gà | HP | Tốc độ (px/s) | Điểm thưởng | Tỉ lệ xuất hiện |

| --- | --- | --- | --- | --- |

| Gà thường | 1 | 80–120 | 10 | 60% |

| Gà nhanh | 1 | 180–220 | 20 | 20% |

| Gà giáp | 3 | 60–100 | 50 | 15% |

| Gà Boss | 30 | 40–60 | 500 | Cuối level (100%) |

| Game tham chiếu | Thể loại | Điểm tương đồng với Chicken Hunter |

| --- | --- | --- |

| Temple Run (Imangi Studios) | Casual / Arcade | Phiên chơi ngắn, dễ chơi, tăng dần độ khó |

| Plants vs. Zombies (PopCap) | Tower-Defense / Casual | Nhiều loại "quái" với thông số riêng, cơ chế phòng thủ |

| Fruit Ninja (Halfbrick) | Arcade / Reflex-based | Gameplay dựa trên phản xạ chạm màn hình liên tục |

| Angry Birds (Rovio) | Puzzle / Casual | Phong cách đồ hoạ cartoon vui nhộn, nhân vật hoạt hình |

| Màn hình | Thành phần chính | Chức năng |

| --- | --- | --- |

| Splash Screen | Logo game, tên game, hiệu ứng fade-in | Hiển thị 2–3 giây rồi tự động chuyển sang Main Menu |

| Main Menu | Logo, nút Play, Highscores, Shop, Settings, nút bật/tắt âm thanh | Trung tâm điều hướng chính của game |

| Level Select | Danh sách level, trạng thái khoá/mở | Cho phép chọn level để chơi |

| Game Screen | Vùng chơi, HUD (điểm, mạng, đạn), nút Pause | Nơi diễn ra toàn bộ gameplay chính, có 4 trạng thái con: Ready, Running, Paused, Game Over |

| Pause Overlay | Nút Resume, Restart, Quit, Settings | Hiện đè lên Game Screen khi tạm dừng |

| Game Over Screen | Điểm số đạt được, điểm cao nhất, nút Retry/Menu | Hiển thị khi người chơi hết mạng hoặc thất bại điều kiện của màn chơi. |

| Highscores Screen | Bảng xếp hạng điểm số (local/online) | Hiển thị Top điểm số cao nhất |

| Settings Screen | Bật/tắt nhạc, âm thanh, độ nhạy điều khiển | Cấu hình được lưu lại cho phiên chơi sau |

| Lựa chọn | Ưu điểm | Nhược điểm | Đánh giá cho Chicken Hunter |

| --- | --- | --- | --- |

| Unity3D | Công cụ kéo-thả (Editor) mạnh, hỗ trợ đa nền tảng (Android/iOS/PC), có sẵn Physics 2D, hệ thống Animation, Particle System, Asset Store phong phú | Ngôn ngữ chính là C# (không phải Java/Kotlin), dung lượng build lớn hơn app thuần Android | ★ Lựa chọn chính — rút ngắn thời gian phát triển, dễ mở rộng hiệu ứng 2D/3D, phù hợp làm game hoàn chỉnh nhanh |

| Android Studio (native, Canvas/SurfaceView) | Toàn quyền kiểm soát vòng lặp game và API Android, đúng trọng tâm lý thuyết môn học | Tốn nhiều công sức code thủ công (vẽ, va chạm, animation, âm thanh) | Phương án tham khảo/đối chiếu để hiểu bản chất kỹ thuật bên dưới engine |

| libgdx | Java, mã nguồn mở, test trực tiếp trên desktop | Cần thiết lập project phức tạp hơn Unity | Không ưu tiên cho đồ án này |

| Unreal Development Kit | Đồ hoạ 3D rất mạnh, chất lượng AAA | Nặng, đường cong học tập cao, không cần thiết cho game 2D casual | Không phù hợp với quy mô đồ án |

| Loại tài nguyên | Định dạng | Nguồn/Ghi chú |

| --- | --- | --- |

| Hình ảnh nhân vật, gà, vũ khí | PNG (có alpha), Sprite Atlas (Sprite Sheet) cho animation | Import vào Unity dưới dạng Sprite (2D and UI); dùng Sprite Editor để cắt frame |

| Hình nền (background) | PNG/JPG, nhiều lớp Sorting Layer cho hiệu ứng parallax | Thiết kế theo phong cách cartoon đã chọn, sắp lớp bằng Sorting Layer/Order in Layer |

| Âm thanh hiệu ứng (SFX) | WAV/OGG ngắn | Import làm AudioClip, phát qua AudioSource.PlayOneShot() |

| Nhạc nền (BGM) | MP3/OGG dài, loop | AudioClip gắn vào AudioSource, bật thuộc tính Loop |

| Font chữ | TTF/OTF tuỳ chỉnh (phong cách hoạt hình) | Import vào Unity, dùng cho TextMeshPro (UI điểm số, menu) |

| Animation nhân vật/gà | Animator Controller (.controller), Animation Clip | Tạo trong Unity Editor từ sprite sheet đã cắt |

| Hiệu ứng particle (lông gà bay, loé súng) | Particle System (native Unity) | Cấu hình trực tiếp trong Editor, không cần asset ngoài |

| Chức năng | Thành phần Unity | API / Phương thức tiêu biểu |

| --- | --- | --- |

| Vòng lặp game & vòng đời đối tượng | MonoBehaviour | Awake(), Start(), Update(), FixedUpdate(), OnEnable()/OnDisable() |

| Hiển thị nhân vật, gà, đạn lên màn hình | SpriteRenderer | sprite, color, sortingLayerName, sortingOrder |

| Va chạm giữa đạn và gà | Collider2D / Rigidbody2D | OnTriggerEnter2D(), OnCollisionEnter2D(), Physics2D.OverlapCircle() |

| Di chuyển gà theo pattern | Transform / Rigidbody2D | Transform.Translate(), Rigidbody2D.velocity, Vector2.Lerp() |

| Nhận thao tác chạm/bắn của người chơi | Input System (mới) / Input (cũ) | Touch, Camera.ScreenToWorldPoint() |

| Animation cho gà (đi, trúng đạn, chết) | Animator | SetTrigger(), SetBool(), Animator.Play() |

| Hiệu ứng particle (lông gà bay, loé súng) | ParticleSystem | Play(), Stop(), Emit() |

| Phát hiệu ứng âm thanh khi bắn trúng gà | AudioSource | PlayOneShot(), volume, pitch |

| Phát nhạc nền | AudioSource | clip, loop = true, Play(), Pause() |

| Lưu điểm cao nhất / cấu hình âm thanh | PlayerPrefs | SetInt(), GetInt(), SetFloat(), Save() |

| Giao diện Menu, HUD, Shop | UI Canvas (UGUI) / TextMeshPro | Button.onClick, Text/TMP_Text.text, Image.sprite |

| Chuyển cảnh giữa các Scene (Menu ↔ Game) | SceneManagement | SceneManager.LoadScene(), LoadSceneAsync() |

| Sinh gà theo chu kỳ thời gian (spawn) | Coroutine | StartCoroutine(), IEnumerator, WaitForSeconds() |

| Rung nhẹ khi bắn trúng Boss (tuỳ chọn) | Handheld (Mobile) | Handheld.Vibrate() |