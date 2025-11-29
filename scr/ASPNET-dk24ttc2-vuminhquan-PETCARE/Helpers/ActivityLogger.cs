using System;
using System.Linq;
using System.Web;
using WebsiteThuCungBento.Models;

namespace WebsiteThuCungBento.Helpers
{
    /// <summary>
    /// Helper class để ghi log hoạt động của admin
    /// </summary>
    public static class ActivityLogger
    {
        /// <summary>
        /// Ghi log hoạt động
        /// </summary>
        public static void Log(int maAdmin, string loaiHoatDong, string moTa)
        {
            try
            {
                using (var db = new DataClassesDataContext())
                {
                    var log = new AdminActivityLog
                    {
                        MAADMIN = maAdmin,
                        LOAIHOATDONG = loaiHoatDong,
                        MOTA = moTa,
                        NGAYTHUCHIEN = DateTime.Now,
                        DIACHI_IP = GetClientIP(),
                        USERAGENT = GetUserAgent()
                    };

                    // Vì chưa có trong DBML, ta sẽ dùng ExecuteCommand
                    db.ExecuteCommand(
                        "INSERT INTO ADMIN_ACTIVITY_LOG (MAADMIN, LOAIHOATDONG, MOTA, NGAYTHUCHIEN, DIACHI_IP, USERAGENT) " +
                        "VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                        maAdmin, loaiHoatDong, moTa, DateTime.Now, GetClientIP(), GetUserAgent()
                    );
                }
            }
            catch (Exception ex)
            {
                // Log error (có thể ghi vào file hoặc event log)
                System.Diagnostics.Debug.WriteLine($"Error logging activity: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy địa chỉ IP của client
        /// </summary>
        private static string GetClientIP()
        {
            try
            {
                var context = HttpContext.Current;
                if (context == null) return "Unknown";

                string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                
                if (string.IsNullOrEmpty(ip))
                {
                    ip = context.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    // Lấy IP đầu tiên nếu có nhiều IP
                    ip = ip.Split(',')[0].Trim();
                }

                return ip ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Lấy User Agent
        /// </summary>
        private static string GetUserAgent()
        {
            try
            {
                var context = HttpContext.Current;
                if (context == null) return "Unknown";

                return context.Request.UserAgent ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }

    /// <summary>
    /// Các hằng số cho loại hoạt động
    /// </summary>
    public static class ActivityType
    {
        public const string DangNhap = "Đăng nhập";
        public const string DangXuat = "Đăng xuất";
        public const string TaoAdmin = "Tạo admin";
        public const string SuaAdmin = "Sửa admin";
        public const string XoaAdmin = "Xóa admin";
        public const string DoiMatKhau = "Đổi mật khẩu";
        public const string KichHoat = "Kích hoạt tài khoản";
        public const string VoHieuHoa = "Vô hiệu hóa tài khoản";
        public const string GanQuyen = "Gán quyền";
        public const string ThuHoiQuyen = "Thu hồi quyền";
        public const string XemDanhSach = "Xem danh sách";
        public const string XemChiTiet = "Xem chi tiết";
    }
}
