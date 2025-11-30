# Use Case Diagrams - Hệ Thống Minh Quân Pet Service

## 📋 Danh Sách Use Case Diagrams


### 1. UseCase_Overall.puml
**Sơ đồ tổng quát** - Hiển thị tất cả actors và use cases chính của hệ thống

**Actors:**
- Khách (Guest)
- Người Dùng (User)
- Nhân Viên (Sitter)
- Quản Trị Viên (Admin)

**Chức năng chính:**
- Xem và đặt dịch vụ
- Quản lý tài khoản và thú cưng
- Quản lý lịch làm việc
- Quản trị hệ thống

---

### 2. UseCase_User_Guest.puml
**Sơ đồ chi tiết cho Khách và Người Dùng**

**Khách (Guest):**
- Xem danh sách dịch vụ
- Đặt dịch vụ không cần đăng nhập
- Nhập thông tin liên hệ và thú cưng
- Xem xác nhận đặt lịch

**Người Dùng (User):**
- Đăng ký/Đăng nhập tài khoản
- Quản lý thú cưng (Thêm/Sửa/Xóa)
- Đặt dịch vụ với thú cưng có sẵn
- Xem lịch sử đặt lịch
- Hủy đặt lịch

---

### 3. UseCase_Admin.puml
**Sơ đồ chi tiết cho Quản Trị Viên**

**Quản Lý Người Dùng:**
- Xem/Thêm/Sửa/Xóa người dùng
- Phân quyền

**Quản Lý Dịch Vụ:**
- Xem/Thêm/Sửa/Xóa dịch vụ
- Cập nhật giá

**Quản Lý Đặt Lịch:**
- Xem tất cả đặt lịch
- Phân công nhân viên
- Xác nhận/Hủy đặt lịch

**Báo Cáo & Thống Kê:**
- Xem báo cáo doanh thu
- Thống kê đặt lịch
- Hiệu suất nhân viên
- Xuất báo cáo

---

### 4. UseCase_Sitter.puml
**Sơ đồ chi tiết cho Nhân Viên**

**Quản Lý Lịch Làm Việc:**
- Xem lịch làm việc
- Xem đơn được phân công
- Xem thông tin khách hàng và thú cưng

**Cập Nhật Trạng Thái:**
- Xác nhận nhận việc
- Đánh dấu đang thực hiện
- Hoàn thành công việc
- Báo cáo vấn đề

**Thống Kê Cá Nhân:**
- Xem số đơn đã làm
- Lịch sử công việc
- Đánh giá

---

## 🎨 Cách Xem Diagrams

### Online (Khuyến nghị)
1. Truy cập [PlantUML Online Editor](https://www.plantuml.com/plantuml/uml/)
2. Copy nội dung file `.puml`
3. Paste vào editor
4. Xem kết quả ngay lập tức

### VS Code
1. Cài extension: **PlantUML**
2. Mở file `.puml`
3. Nhấn `Alt + D` để xem preview

### Command Line
```bash
# Cài PlantUML
npm install -g node-plantuml

# Generate PNG
puml generate UseCase_Overall.puml -o output.png
```

---

## 📊 Ký Hiệu Sử Dụng

- `-->` : Association (Actor sử dụng Use Case)
- `--|>` : Generalization (Kế thừa)
- `..>` : Dependency
- `<<include>>` : Use case bắt buộc phải thực hiện
- `<<extend>>` : Use case tùy chọn
- `<<require>>` : Điều kiện tiên quyết

---

## 🎯 Mục Đích

Các use case diagrams này được tạo để:
- Hiểu rõ chức năng của hệ thống
- Phân tích yêu cầu
- Thiết kế hệ thống
- Tài liệu hóa dự án
- Trình bày demo

---

## 📝 Ghi Chú

- Tất cả use cases đều dựa trên implementation thực tế của hệ thống
- Diagrams được cập nhật theo version mới nhất của code
- Sử dụng tiếng Việt để dễ hiểu

---

**Ngày tạo:** 30/11/2025  
**Phiên bản:** 1.0  
**Tác giả:** Minh Quân Pet Service Team
