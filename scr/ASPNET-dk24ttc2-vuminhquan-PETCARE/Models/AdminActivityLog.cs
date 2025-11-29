using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteThuCungBento.Models
{
    /// <summary>
    /// Model cho lịch sử hoạt động của Admin
    /// </summary>
    public class AdminActivityLog
    {
        [Key]
        public int MALOG { get; set; }

        [Required]
        public int MAADMIN { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Loại hoạt động")]
        public string LOAIHOATDONG { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string MOTA { get; set; }

        [Display(Name = "Ngày thực hiện")]
        public DateTime NGAYTHUCHIEN { get; set; }

        [StringLength(50)]
        [Display(Name = "Địa chỉ IP")]
        public string DIACHI_IP { get; set; }

        [StringLength(500)]
        [Display(Name = "User Agent")]
        public string USERAGENT { get; set; }

        // Navigation property
        public virtual ADMIN ADMIN { get; set; }
    }

    /// <summary>
    /// Enum cho các loại hoạt động
    /// </summary>
    public enum LoaiHoatDong
    {
        [Display(Name = "Đăng nhập")]
        DangNhap,

        [Display(Name = "Đăng xuất")]
        DangXuat,

        [Display(Name = "Tạo admin")]
        TaoAdmin,

        [Display(Name = "Sửa admin")]
        SuaAdmin,

        [Display(Name = "Xóa admin")]
        XoaAdmin,

        [Display(Name = "Đổi mật khẩu")]
        DoiMatKhau,

        [Display(Name = "Kích hoạt tài khoản")]
        KichHoat,

        [Display(Name = "Vô hiệu hóa tài khoản")]
        VoHieuHoa,

        [Display(Name = "Gán quyền")]
        GanQuyen,

        [Display(Name = "Thu hồi quyền")]
        ThuHoiQuyen
    }
}
