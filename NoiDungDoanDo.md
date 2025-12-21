# HỆ THỐNG QUẢN LÝ VÀ ĐẶT CHỖ DU LỊCH (ACC TOUR)

## 1. LÝ DO CHỌN ĐỀ TÀI

Ngành du lịch là một ngành kinh tế quan trọng, đóng góp đáng kể vào tăng trưởng kinh tế và tạo việc làm cho các quốc gia trên thế giới. Tuy nhiên, các doanh nghiệp du lịch truyền thống thường gặp nhiều thách thức:

- **Quản lý tour phức tạp**: Quản lý thông tin các tour du lịch, lịch trình, giá cả, số lượng chỗ ngồi còn lại một cách thủ công là rất khó khăn và dễ dẫn đến sai sót.

- **Khó khăn trong đặt chỗ**: Khách hàng phải gọi điện hoặc ghé thăm văn phòng để đặt tour, mất thời gian và không thuận tiện.

- **Thiếu tính minh bạch**: Thông tin về giá cả, lịch trình, tình trạng đặt chỗ không được cập nhật kịp thời và không công khai.

- **Quản lý khách hàng không hiệu quả**: Khó theo dõi các đơn đặt chỗ, lịch sử giao dịch, và phản hồi của khách hàng.

- **Thiếu hỗ trợ bán hàng**: Không có cách hiệu quả để quản lý bán hàng, thanh toán và theo dõi doanh thu.

Do đó, chúng tôi lựa chọn phát triển hệ thống **Quản lý và Đặt chỗ Du lịch (ACC Tour)** để giải quyết các vấn đề trên. Hệ thống này sẽ cung cấp một giải pháp công nghệ thông tin hiện đại, giúp doanh nghiệp du lịch:
- Quản lý tour một cách hiệu quả
- Cho phép khách hàng đặt chỗ trực tuyến dễ dàng
- Cung cấp thông tin minh bạch và kịp thời
- Cải thiện trải nghiệm khách hàng
- Nâng cao năng suất và hiệu quả kinh doanh

---

## 2. MỤC TIÊU ĐỒ ÁN

### 2.1 Mục tiêu chung
Phát triển một nền tảng web toàn diện cho phép doanh nghiệp du lịch quản lý các tour du lịch và cho phép khách hàng dễ dàng tìm kiếm, xem chi tiết, và đặt chỗ cho các tour.

### 2.2 Mục tiêu cụ thể

#### A. Chức năng quản lý tour
- Xây dựng module cho phép quản lý viên (admin) thêm, sửa, xóa thông tin các tour du lịch
- Quản lý thông tin chi tiết tour như: tên tour, giá cả, ngày khởi hành, ngày kết thúc, số người tối đa/tối thiểu, mô tả, hình ảnh
- Thống kê số chỗ còn lại theo thời gian thực
- Cập nhật trạng thái hoạt động của tour (kích hoạt/vô hiệu hóa)

#### B. Chức năng quản lý đặt chỗ
- Cho phép khách hàng đăng ký tài khoản và đăng nhập vào hệ thống
- Cho phép khách hàng tìm kiếm tour theo tên
- Cho phép khách hàng xem chi tiết các tour (giá, lịch trình, mô tả, hình ảnh)
- Cho phép khách hàng đặt chỗ cho các tour (chọn số người, thời gian)
- Hệ thống tự động tính toán tổng giá tiền
- Quản lý lịch sử đặt chỗ của khách hàng

#### C. Chức năng quản lý tài khoản người dùng
- Xây dựng hệ thống xác thực và phân quyền người dùng
- Phân biệt giữa quản lý viên (Admin) và khách hàng (User)
- Quản lý thông tin cá nhân của người dùng (họ tên, email, điện thoại)
- Bảo mật tài khoản bằng xác thực password mạnh

#### D. Chức năng nội dung và thông tin
- Xây dựng module quản lý blog để đăng bài viết thông tin du lịch
- Cho phép khách hàng xem các bài viết về các điểm đến du lịch
- Quản lý thư viện hình ảnh mô tả tour
- Quản lý các tệp đính kèm trong bài blog

#### E. Chức năng quản lý hướng dẫn viên
- Lưu trữ thông tin hướng dẫn viên
- Gán hướng dẫn viên cho các tour
- Quản lý khả năng và điểm mạnh của từng hướng dẫn viên

#### F. Chức năng đánh giá và nhận xét
- Cho phép khách hàng đánh giá tour sau khi tham gia
- Xem các đánh giá và nhận xét từ những khách hàng khác

### 2.3 Mục tiêu phi chức năng
- **Hiệu năng**: Hệ thống phải tải nhanh và xử lý được lượng yêu cầu từ nhiều người dùng đồng thời
- **Bảo mật**: Bảo vệ thông tin cá nhân và dữ liệu giao dịch của người dùng
- **Độ tin cậy**: Hệ thống phải ổn định và hoạt động liên tục
- **Khả năng mở rộng**: Dễ dàng thêm các tính năng mới trong tương lai
- **Tính thân thiện**: Giao diện dễ sử dụng cho cả người dùng kỹ thuật và không kỹ thuật

---

## 3. Ý NGHĨA THỰC TIỄN

### 3.1 Ý nghĩa đối với doanh nghiệp du lịch

**Cải thiện hiệu quả hoạt động:**
- Giảm thời gian quản lý tour từ hàng giờ xuống chỉ vài phút
- Tự động hóa các quy trình cơ bản giúp giảm lỗi sai
- Có được cái nhìn tổng quan về tình trạng đặt chỗ và doanh thu thực tế
- Quản lý nhiều tour cùng lúc một cách dễ dàng

**Tăng cơ hội bán hàng:**
- Tiếp cận khách hàng mới thông qua nền tảng trực tuyến
- Cho phép khách hàng đặt chỗ mọi lúc, mọi nơi (24/7)
- Không bị hạn chế bởi giờ làm việc của văn phòng
- Hỗ trợ đa nền tảng (web, responsive mobile)

**Cung cấp trải nghiệm khách hàng tốt hơn:**
- Khách hàng có thể xem thông tin chi tiết, minh bạch về tour trước khi đặt chỗ
- Quy trình đặt chỗ đơn giản, nhanh chóng
- Có thể theo dõi tình trạng đơn hàng của mình
- Nhận được thông báo và cập nhật thông tin tour một cách kịp thời

**Xây dựng thương hiệu:**
- Một hệ thống chuyên nghiệp tạo ấn tượng tốt với khách hàng
- Giúp xây dựng hình ảnh doanh nghiệp hiện đại
- Tạo cơ sở để phát triển các tính năng nâng cao trong tương lai

### 3.2 Ý nghĩa đối với khách hàng

**Tiện lợi:**
- Không cần phải gọi điện hay đến văn phòng để đặt chỗ
- Có thể đặt chỗ bất cứ lúc nào từ máy tính hoặc điện thoại
- Thông tin du lịch dễ tìm kiếm và truy cập

**Đảm bảo quyền lợi:**
- Thông tin minh bạch về giá cả, lịch trình, điều kiện tour
- Có thông báo xác nhận đơn đặt chỗ
- Có thể xem lịch sử các đơn đặt chỗ của mình
- Hỗ trợ khách hàng bằng cách cung cấp thông tin chi tiết

**Cộng đồng du lịch:**
- Có thể xem đánh giá, nhận xét từ những du khách khác
- Nhận được thông tin, mẹo về các điểm đến từ blog
- Tham gia cộng đồng những người yêu thích du lịch

### 3.3 Ý nghĩa đối với nhân viên du lịch

**Cải thiện công việc:**
- Giảm bớt công việc thủ công, lặp lại
- Dễ dàng tìm kiếm và quản lý thông tin khách hàng
- Theo dõi các đơn hàng một cách tổ chức
- Dễ dàng lên kế hoạch và điều phối tour

**Nâng cao năng suất:**
- Có thể tập trung vào dịch vụ khách hàng thay vì quản lý hành chính
- Công cụ hỗ trợ giúp quyết định nhanh hơn

### 3.4 Ý nghĩa công nghệ

**Ứng dụng công nghệ hiện đại:**
- Sử dụng ASP.NET Core - một framework web mạnh mẽ và hiện đại
- Cơ sở dữ liệu SQL Server với Entity Framework ORM
- Xây dựng hệ thống xác thực và phân quyền theo chuẩn ngành
- Áp dụng mô hình MVC (Model-View-Controller)

**Mở rộng cho tương lai:**
- Dễ dàng thêm tính năng thanh toán trực tuyến
- Có thể tích hợp với hệ thống đặt vé máy bay, khách sạn khác
- Hỗ trợ phát triển ứng dụng di động
- Có thể tích hợp AI để gợi ý tour phù hợp

---

## 4. CƠ SỞ LÝ THUYẾT

### 4.1 Kiến trúc phần mềm - Mô hình MVC (Model-View-Controller)

**Định nghĩa:**
Mô hình MVC là một mô hình thiết kế kiến trúc phổ biến trong phát triển ứng dụng web. Nó chia ứng dụng thành ba thành phần chính:

1. **Model (Mô hình dữ liệu):**
   - Đại diện cho dữ liệu và logic kinh doanh của ứng dụng
   - Trong dự án: Tour, Booking, ApplicationUser, Blog, Review, TourGuide, TourAssignment, DescriptionImage
   - Quản lý truy cập cơ sở dữ liệu thông qua Entity Framework Core

2. **View (Giao diện người dùng):**
   - Hiển thị dữ liệu cho người dùng
   - Là các trang HTML được render động từ dữ liệu Model
   - Cho phép người dùng tương tác với ứng dụng

3. **Controller (Bộ điều khiển):**
   - Xử lý yêu cầu từ người dùng
   - Gọi các phương thức của Model để truy cập/cập nhật dữ liệu
   - Chọn View phù hợp để hiển thị kết quả
   - Trong dự án: TourController, BookingController, HomeController, AccountController

**Lợi ích:**
- Phân tách rõ ràng giữa dữ liệu, logic và giao diện
- Dễ bảo trì và mở rộng mã nguồn
- Hỗ trợ test đơn vị (unit testing)
- Cho phép nhiều developer làm việc trên các phần khác nhau cùng một lúc

### 4.2 Cơ sở dữ liệu quan hệ - Relational Database

**Khái niệm:**
Cơ sở dữ liệu quan hệ sử dụng bảng (table) để lưu trữ dữ liệu, với các hàng (row) đại diện cho bản ghi và cột (column) đại diện cho các trường (field). Các bảng được liên kết với nhau thông qua các khóa ngoài (foreign key).

**Các bảng chính trong dự án:**
- **Tours**: Lưu thông tin các tour du lịch
- **Bookings**: Lưu các đơn đặt chỗ của khách hàng
- **Users**: Lưu thông tin tài khoản người dùng (kế thừa từ AspNetUsers)
- **Reviews**: Lưu đánh giá của khách hàng
- **Blogs**: Lưu các bài viết blog
- **TourGuides**: Lưu thông tin hướng dẫn viên
- **TourAssignments**: Lưu sự gán hướng dẫn viên cho tour
- **DescriptionImages**: Lưu hình ảnh mô tả của tour

**Ưu điểm:**
- An toàn dữ liệu, đảm bảo tính toàn vẹn
- Dễ query dữ liệu phức tạp
- Hỗ trợ giao dịch (transaction) để đảm bảo tính nhất quán
- Hiệu quả lưu trữ, tiết kiệm không gian

### 4.3 Xác thực và phân quyền (Authentication & Authorization)

**Xác thực (Authentication):**
Xác thực là quá trình xác định danh tính của người dùng. Trong dự án:
- Sử dụng ASP.NET Identity để quản lý tài khoản người dùng
- Mật khẩu phải đáp ứng các yêu cầu bảo mật (tối thiểu 8 ký tự, chứa chữ hoa, chữ thường, số, ký tự đặc biệt)
- Sử dụng cookie để duy trì session đăng nhập

**Phân quyền (Authorization):**
Phân quyền là quá trình quyết định xem người dùng có quyền truy cập tài nguyên hay không:
- Hai vai trò chính: Admin và User
- Admin có quyền quản lý tour, xem thống kê, quản lý người dùng
- User chỉ có quyền đặt chỗ, xem tour, đánh giá
- Sử dụng đặc tính `[Authorize]` để bảo vệ các action

**Ưu điểm:**
- Bảo vệ thông tin nhạy cảm
- Ngăn chặn truy cập trái phép
- Tạo môi trường tin cậy cho người dùng

### 4.4 Object-Relational Mapping (ORM) - Entity Framework Core

**Khái niệm:**
ORM là một kỹ thuật lập trình cho phép ánh xạ dữ liệu từ cơ sở dữ liệu quan hệ thành các đối tượng trong mã nguồn. Entity Framework Core là một ORM mạnh mẽ cho .NET.

**Lợi ích:**
- Không cần viết SQL trực tiếp (LINQ thay thế)
- Giảm lỗi liên quan đến SQL injection
- Dễ thay đổi cơ sở dữ liệu mà không thay đổi mã logic
- Tự động quản lý việc ánh xạ giữa bảng và đối tượng
- Hỗ trợ lazy loading và eager loading để tối ưu truy vấn

**Cách sử dụng trong dự án:**
```csharp
var tours = from t in _context.Tours
           where t.IsActive == true
           select t;
```

### 4.5 Mẫu thiết kế Repository Pattern

**Khái niệm:**
Repository Pattern là một mẫu thiết kế giúp tách biệt logic truy cập dữ liệu khỏi logic kinh doanh. Repository đóng vai trò là một lớp trung gian giữa Controller và cơ sở dữ liệu.

**Lợi ích:**
- Dễ test bằng cách mock Repository
- Tập trung truy cập dữ liệu ở một nơi
- Dễ bảo trì và thay đổi cách truy cập dữ liệu

### 4.6 Validations (Kiểm tra dữ liệu)

**Định nghĩa:**
Validation là quá trình kiểm tra xem dữ liệu đầu vào có đáp ứng các yêu cầu hay không.

**Các loại validation trong dự án:**
- **Required**: Trường bắt buộc phải nhập (Tên Tour, Giá, Ngày bắt đầu)
- **Range**: Giá trị phải nằm trong một khoảng (Giá > 0, Số người > 0)
- **StringLength**: Độ dài chuỗi tối đa (Tiêu đề blog ≤ 200 ký tự)
- **DataType**: Kiểm tra kiểu dữ liệu (Currency, Date)
- **Custom Validation**: Xác thực tùy chỉnh (Ngày kết thúc > Ngày bắt đầu)

**Ưu điểm:**
- Đảm bảo dữ liệu hợp lệ trước khi lưu vào cơ sở dữ liệu
- Cung cấp thông báo lỗi rõ ràng cho người dùng
- Tăng độ tin cậy của ứng dụng

### 4.7 Xử lý giao dịch (Transaction Management)

**Định nghĩa:**
Giao dịch là một chuỗi các hoạt động cơ sở dữ liệu được xử lý như một đơn vị. Nó đảm bảo tính ACID:
- **Atomicity** (Tính nguyên tử): Tất cả hoặc không có gì
- **Consistency** (Tính nhất quán): Dữ liệu luôn ở trạng thái hợp lệ
- **Isolation** (Tính cô lập): Các giao dịch không can nhiễm nhau
- **Durability** (Tính bền): Dữ liệu được lưu vĩnh viễn khi giao dịch hoàn thành

**Áp dụng trong dự án:**
- Khi khách hàng đặt chỗ, hệ thống cần cập nhật cả bảng Bookings và giảm RemainingSlots của Tour
- Nếu bất kỳ lỗi nào xảy ra, toàn bộ giao dịch được rollback

### 4.8 Design Patterns - Công cụ cho phát triển bền vững

**Dependency Injection (DI):**
- Kỹ thuật giúp giảm sự phụ thuộc giữa các thành phần
- Trong dự án: ApplicationDbContext được inject vào các Controller thông qua constructor
- Làm cho mã dễ test hơn

**Builder Pattern:**
- Sử dụng trong Program.cs để xây dựng ứng dụng
- Cho phép cấu hình dần dần các dịch vụ

---

## KẾT LUẬN

Dự án ACC Tour áp dụng nhiều nguyên tắc và công nghệ hiện đại:
- Kiến trúc MVC rõ ràng giúp dễ bảo trì
- Sử dụng Entity Framework Core giảm lỗi lập trình
- Xác thực/phân quyền bảo vệ dữ liệu
- Validation đảm bảo chất lượng dữ liệu
- Giao dịch cơ sở dữ liệu đảm bảo tính nhất quán

Những nền tảng lý thuyết này giúp xây dựng một hệ thống:
- **Hiệu quả**: Quản lý tour một cách chuyên nghiệp
- **An toàn**: Bảo vệ thông tin người dùng
- **Mở rộng được**: Dễ thêm tính năng mới
- **Dễ bảo trì**: Cấu trúc rõ ràng, dễ hiểu
- **Tin cậy**: Xử lý dữ liệu một cách an toàn
