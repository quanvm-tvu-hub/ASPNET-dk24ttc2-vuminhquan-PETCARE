using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebsiteThuCungBento.ViewModels
{
    /// <summary>
    /// ViewModel cho danh sách admin với tìm kiếm và phân trang
    /// </summary>
    public class AdminListViewModel
    {
        public List<AdminItemViewModel> Admins { get; set; }
        
        // Tìm kiếm
        [Display(Name = "Từ khóa tìm kiếm")]
        public string SearchKeyword { get; set; }

        [Display(Name = "Loại tài khoản")]
        public string LoaiTaiKhoan { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? TrangThai { get; set; }

        [Display(Name = "Vai trò")]
        public int? VaiTroId { get; set; }

        // Phân trang
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        // Sắp xếp
        public string SortBy { get; set; } = "MAADMIN";
        public string SortOrder { get; set; } = "DESC";

        // Danh sách cho dropdown
        public List<SelectListItem> LoaiTaiKhoanList { get; set; }
        public List<SelectListItem> VaiTroList { get; set; }
    }

    /// <summary>
    /// ViewModel cho từng item admin trong danh sách
    /// </summary>
    public class AdminItemViewModel
    {
        public int MAADMIN { get; set; }
        public string HOTEN { get; set; }
        public string DIACHI { get; set; }
        public string DIENTHOAI { get; set; }
        public string TENLOAI { get; set; }
        public string TENDN { get; set; }
        public string AVATAR { get; set; }
        public string EMAIL { get; set; }
        public bool TRANGTHAI { get; set; }
        public DateTime? NGAYTAO { get; set; }
        public DateTime? LANDANGNHAPCUOI { get; set; }
        public string DanhSachVaiTro { get; set; }
        public int SoLanHoatDong { get; set; }
    }

    /// <summary>
    /// ViewModel cho tạo/sửa admin
    /// </summary>
    public class AdminFormViewModel
    {
        public int MAADMIN { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HOTEN { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string DIACHI { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(11, MinimumLength = 9, ErrorMessage = "Số điện thoại không hợp lệ")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string DIENTHOAI { get; set; }

        [Required(ErrorMessage = "Loại tài khoản không được để trống")]
        [StringLength(100)]
        [Display(Name = "Loại tài khoản")]
        public string TENLOAI { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(30)]
        [Display(Name = "Tên đăng nhập")]
        public string TENDN { get; set; }

        [StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-20 ký tự")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string MATKHAU { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("MATKHAU", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        public string XacNhanMatKhau { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(50)]
        [Display(Name = "Email")]
        public string EMAIL { get; set; }

        [Display(Name = "Hình đại diện")]
        public string AVATAR { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TRANGTHAI { get; set; } = true;

        [Display(Name = "Vai trò")]
        public List<int> SelectedVaiTroIds { get; set; }

        // Danh sách cho dropdown
        public List<SelectListItem> LoaiTaiKhoanList { get; set; }
        public List<SelectListItem> VaiTroList { get; set; }

        public bool IsEdit { get; set; }
    }

    /// <summary>
    /// ViewModel cho dashboard thống kê admin
    /// </summary>
    public class AdminDashboardViewModel
    {
        public int TongSoAdmin { get; set; }
        public int AdminHoatDong { get; set; }
        public int AdminKhongHoatDong { get; set; }
        public int DangNhapTuan { get; set; }
        public int AdminMoiThang { get; set; }

        public List<AdminItemViewModel> AdminGanDay { get; set; }
        public List<AdminActivityLogViewModel> HoatDongGanDay { get; set; }

        // Dữ liệu cho biểu đồ
        public Dictionary<string, int> ThongKeTheoLoai { get; set; }
        public Dictionary<string, int> ThongKeTheoThang { get; set; }
    }

    /// <summary>
    /// ViewModel cho lịch sử hoạt động
    /// </summary>
    public class AdminActivityLogViewModel
    {
        public int MALOG { get; set; }
        public int MAADMIN { get; set; }
        public string TenAdmin { get; set; }
        public string LOAIHOATDONG { get; set; }
        public string MOTA { get; set; }
        public DateTime NGAYTHUCHIEN { get; set; }
        public string DIACHI_IP { get; set; }
        public string USERAGENT { get; set; }
    }

    /// <summary>
    /// ViewModel cho lịch sử hoạt động với phân trang
    /// </summary>
    public class ActivityLogListViewModel
    {
        public List<AdminActivityLogViewModel> Logs { get; set; }
        
        [Display(Name = "Admin")]
        public int? AdminId { get; set; }

        [Display(Name = "Loại hoạt động")]
        public string LoaiHoatDong { get; set; }

        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? DenNgay { get; set; }

        // Phân trang
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        // Danh sách cho dropdown
        public List<SelectListItem> AdminList { get; set; }
        public List<SelectListItem> LoaiHoatDongList { get; set; }
    }
}
