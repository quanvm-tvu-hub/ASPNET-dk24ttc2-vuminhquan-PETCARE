using WebsiteThuCungBento.Models;
using WebsiteThuCungBento.ViewModels;
using WebsiteThuCungBento.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;

namespace WebsiteThuCungBento.Controllers
{
    public class AdminManagementController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();

        #region Dashboard và Thống Kê

        /// <summary>
        /// Dashboard quản lý admin
        /// </summary>
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult Dashboard()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            var viewModel = new AdminDashboardViewModel();

            try
            {
                // Lấy thống kê tổng quan
                var stats = data.ExecuteQuery<dynamic>(
                    "EXEC sp_ThongKeAdmin"
                ).FirstOrDefault();

                if (stats != null)
                {
                    viewModel.TongSoAdmin = stats.TongSoAdmin ?? 0;
                    viewModel.AdminHoatDong = stats.AdminHoatDong ?? 0;
                    viewModel.AdminKhongHoatDong = stats.AdminKhongHoatDong ?? 0;
                    viewModel.DangNhapTuan = stats.DangNhapTuan ?? 0;
                    viewModel.AdminMoiThang = stats.AdminMoiThang ?? 0;
                }

                // Lấy danh sách admin gần đây
                var adminGanDay = data.ExecuteQuery<dynamic>(
                    @"SELECT TOP 5 * FROM vw_AdminDayDu 
                      ORDER BY NGAYTAO DESC"
                ).ToList();

                viewModel.AdminGanDay = adminGanDay.Select(a => new AdminItemViewModel
                {
                    MAADMIN = a.MAADMIN,
                    HOTEN = a.HOTEN,
                    EMAIL = a.EMAIL,
                    TRANGTHAI = a.TRANGTHAI ?? true,
                    NGAYTAO = a.NGAYTAO,
                    LANDANGNHAPCUOI = a.LANDANGNHAPCUOI
                }).ToList();

                // Lấy hoạt động gần đây
                var hoatDongGanDay = data.ExecuteQuery<dynamic>(
                    @"SELECT TOP 10 l.*, a.HOTEN as TenAdmin 
                      FROM ADMIN_ACTIVITY_LOG l
                      INNER JOIN ADMIN a ON l.MAADMIN = a.MAADMIN
                      ORDER BY l.NGAYTHUCHIEN DESC"
                ).ToList();

                viewModel.HoatDongGanDay = hoatDongGanDay.Select(h => new AdminActivityLogViewModel
                {
                    MALOG = h.MALOG,
                    MAADMIN = h.MAADMIN,
                    TenAdmin = h.TenAdmin,
                    LOAIHOATDONG = h.LOAIHOATDONG,
                    MOTA = h.MOTA,
                    NGAYTHUCHIEN = h.NGAYTHUCHIEN,
                    DIACHI_IP = h.DIACHI_IP
                }).ToList();

                // Thống kê theo loại
                var thongKeLoai = data.ADMINs
                    .GroupBy(a => a.TENLOAI)
                    .Select(g => new { Loai = g.Key, SoLuong = g.Count() })
                    .ToDictionary(x => x.Loai, x => x.SoLuong);

                viewModel.ThongKeTheoLoai = thongKeLoai;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi khi tải thống kê: " + ex.Message;
            }

            return View(viewModel);
        }

        #endregion

        #region Danh Sách Admin với Tìm Kiếm và Phân Trang

        /// <summary>
        /// Danh sách admin với tìm kiếm, lọc và phân trang
        /// </summary>
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult Index(AdminListViewModel model)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            try
            {
                // Query cơ bản
                var query = data.ADMINs.AsQueryable();

                // Tìm kiếm
                if (!string.IsNullOrEmpty(model.SearchKeyword))
                {
                    var keyword = model.SearchKeyword.ToLower();
                    query = query.Where(a =>
                        a.HOTEN.ToLower().Contains(keyword) ||
                        a.EMAIL.ToLower().Contains(keyword) ||
                        a.TENDN.ToLower().Contains(keyword) ||
                        a.DIENTHOAI.Contains(keyword)
                    );
                }

                // Lọc theo loại tài khoản
                if (!string.IsNullOrEmpty(model.LoaiTaiKhoan))
                {
                    query = query.Where(a => a.TENLOAI == model.LoaiTaiKhoan);
                }

                // Lọc theo trạng thái (nếu có cột TRANGTHAI)
                // if (model.TrangThai.HasValue)
                // {
                //     query = query.Where(a => a.TRANGTHAI == model.TrangThai.Value);
                // }

                // Đếm tổng số bản ghi
                model.TotalRecords = query.Count();
                model.TotalPages = (int)Math.Ceiling((double)model.TotalRecords / model.PageSize);

                // Sắp xếp
                switch (model.SortBy)
                {
                    case "HOTEN":
                        query = model.SortOrder == "ASC" ? query.OrderBy(a => a.HOTEN) : query.OrderByDescending(a => a.HOTEN);
                        break;
                    case "EMAIL":
                        query = model.SortOrder == "ASC" ? query.OrderBy(a => a.EMAIL) : query.OrderByDescending(a => a.EMAIL);
                        break;
                    case "NGAYTAO":
                        // query = model.SortOrder == "ASC" ? query.OrderBy(a => a.NGAYTAO) : query.OrderByDescending(a => a.NGAYTAO);
                        query = query.OrderByDescending(a => a.MAADMIN); // Tạm thời dùng MAADMIN
                        break;
                    default:
                        query = model.SortOrder == "ASC" ? query.OrderBy(a => a.MAADMIN) : query.OrderByDescending(a => a.MAADMIN);
                        break;
                }

                // Phân trang
                var admins = query
                    .Skip((model.CurrentPage - 1) * model.PageSize)
                    .Take(model.PageSize)
                    .ToList();

                // Chuyển đổi sang ViewModel
                model.Admins = admins.Select(a => new AdminItemViewModel
                {
                    MAADMIN = a.MAADMIN,
                    HOTEN = a.HOTEN,
                    DIACHI = a.DIACHI,
                    DIENTHOAI = a.DIENTHOAI,
                    TENLOAI = a.TENLOAI,
                    TENDN = a.TENDN,
                    AVATAR = a.AVATAR,
                    EMAIL = a.EMAIL,
                    TRANGTHAI = true, // Mặc định true nếu chưa có cột
                    // NGAYTAO = a.NGAYTAO,
                    // LANDANGNHAPCUOI = a.LANDANGNHAPCUOI
                }).ToList();

                // Danh sách cho dropdown
                model.LoaiTaiKhoanList = data.ADMINs
                    .Select(a => a.TENLOAI)
                    .Distinct()
                    .Select(l => new SelectListItem { Value = l, Text = l })
                    .ToList();

                // Ghi log
                var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                if (currentAdmin != null)
                {
                    ActivityLogger.Log(currentAdmin.MAADMIN, ActivityType.XemDanhSach, "Xem danh sách admin");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi khi tải danh sách: " + ex.Message;
                model.Admins = new List<AdminItemViewModel>();
            }

            return View(model);
        }

        #endregion

        #region Tạo và Sửa Admin

        /// <summary>
        /// Hiển thị form tạo admin mới
        /// </summary>
        [HttpGet]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            var model = new AdminFormViewModel
            {
                IsEdit = false,
                TRANGTHAI = true
            };

            PrepareFormData(model);
            return View("CreateEdit", model);
        }

        /// <summary>
        /// Xử lý tạo admin mới
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult Create(AdminFormViewModel model, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            try
            {
                if (ModelState.IsValid)
                {
                    // Kiểm tra tên đăng nhập đã tồn tại
                    if (data.ADMINs.Any(a => a.TENDN == model.TENDN))
                    {
                        ModelState.AddModelError("TENDN", "Tên đăng nhập đã tồn tại");
                        PrepareFormData(model);
                        return View("CreateEdit", model);
                    }

                    // Kiểm tra email đã tồn tại
                    if (data.ADMINs.Any(a => a.EMAIL == model.EMAIL))
                    {
                        ModelState.AddModelError("EMAIL", "Email đã được sử dụng");
                        PrepareFormData(model);
                        return View("CreateEdit", model);
                    }

                    // Xử lý upload avatar
                    string avatarFileName = "default-avatar.png";
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        avatarFileName = SaveUploadedFile(fileUpload);
                    }

                    // Tạo admin mới
                    var admin = new ADMIN
                    {
                        HOTEN = model.HOTEN,
                        DIACHI = model.DIACHI,
                        DIENTHOAI = model.DIENTHOAI,
                        TENLOAI = model.TENLOAI,
                        TENDN = model.TENDN,
                        MATKHAU = SecurityHelper.HashPassword(model.MATKHAU), // Hash mật khẩu
                        EMAIL = model.EMAIL,
                        AVATAR = avatarFileName
                        // TRANGTHAI = model.TRANGTHAI,
                        // NGAYTAO = DateTime.Now
                    };

                    data.ADMINs.InsertOnSubmit(admin);
                    data.SubmitChanges();

                    // Ghi log
                    var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                    if (currentAdmin != null)
                    {
                        ActivityLogger.Log(currentAdmin.MAADMIN, ActivityType.TaoAdmin,
                            $"Tạo tài khoản admin mới: {admin.TENDN} - {admin.HOTEN}");
                    }

                    TempData["Success"] = "Tạo tài khoản admin thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            PrepareFormData(model);
            return View("CreateEdit", model);
        }

        /// <summary>
        /// Hiển thị form sửa admin
        /// </summary>
        [HttpGet]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            var admin = data.ADMINs.SingleOrDefault(a => a.MAADMIN == id);
            if (admin == null)
            {
                TempData["Error"] = "Không tìm thấy admin";
                return RedirectToAction("Index");
            }

            var model = new AdminFormViewModel
            {
                MAADMIN = admin.MAADMIN,
                HOTEN = admin.HOTEN,
                DIACHI = admin.DIACHI,
                DIENTHOAI = admin.DIENTHOAI,
                TENLOAI = admin.TENLOAI,
                TENDN = admin.TENDN,
                EMAIL = admin.EMAIL,
                AVATAR = admin.AVATAR,
                TRANGTHAI = true, // admin.TRANGTHAI,
                IsEdit = true
            };

            PrepareFormData(model);
            return View("CreateEdit", model);
        }

        /// <summary>
        /// Xử lý sửa admin
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult Edit(AdminFormViewModel model, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            try
            {
                var admin = data.ADMINs.SingleOrDefault(a => a.MAADMIN == model.MAADMIN);
                if (admin == null)
                {
                    TempData["Error"] = "Không tìm thấy admin";
                    return RedirectToAction("Index");
                }

                // Kiểm tra email trùng (trừ chính nó)
                if (data.ADMINs.Any(a => a.EMAIL == model.EMAIL && a.MAADMIN != model.MAADMIN))
                {
                    ModelState.AddModelError("EMAIL", "Email đã được sử dụng");
                    PrepareFormData(model);
                    return View("CreateEdit", model);
                }

                // Xử lý upload avatar mới
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    admin.AVATAR = SaveUploadedFile(fileUpload);
                }

                // Cập nhật thông tin
                admin.HOTEN = model.HOTEN;
                admin.DIACHI = model.DIACHI;
                admin.DIENTHOAI = model.DIENTHOAI;
                admin.TENLOAI = model.TENLOAI;
                admin.EMAIL = model.EMAIL;
                // admin.TRANGTHAI = model.TRANGTHAI;

                // Chỉ cập nhật mật khẩu nếu có nhập mới
                if (!string.IsNullOrEmpty(model.MATKHAU))
                {
                    admin.MATKHAU = SecurityHelper.HashPassword(model.MATKHAU);
                }

                data.SubmitChanges();

                // Ghi log
                var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                if (currentAdmin != null)
                {
                    ActivityLogger.Log(currentAdmin.MAADMIN, ActivityType.SuaAdmin,
                        $"Cập nhật thông tin admin: {admin.TENDN} - {admin.HOTEN}");
                }

                TempData["Success"] = "Cập nhật thông tin admin thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            PrepareFormData(model);
            return View("CreateEdit", model);
        }

        #endregion

        #region Xóa và Thay Đổi Trạng Thái

        /// <summary>
        /// Hiển thị trang xác nhận xóa
        /// </summary>
        [HttpGet]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            var admin = data.ADMINs.SingleOrDefault(a => a.MAADMIN == id);
            if (admin == null)
            {
                TempData["Error"] = "Không tìm thấy admin";
                return RedirectToAction("Index");
            }

            return View(admin);
        }

        /// <summary>
        /// Xử lý xóa admin
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            try
            {
                var admin = data.ADMINs.SingleOrDefault(a => a.MAADMIN == id);
                if (admin == null)
                {
                    TempData["Error"] = "Không tìm thấy admin";
                    return RedirectToAction("Index");
                }

                // Không cho phép xóa chính mình
                var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                if (currentAdmin != null && currentAdmin.MAADMIN == id)
                {
                    TempData["Error"] = "Không thể xóa tài khoản của chính bạn!";
                    return RedirectToAction("Index");
                }

                string tenAdmin = admin.HOTEN;
                string tenDN = admin.TENDN;

                data.ADMINs.DeleteOnSubmit(admin);
                data.SubmitChanges();

                // Ghi log
                if (currentAdmin != null)
                {
                    ActivityLogger.Log(currentAdmin.MAADMIN, ActivityType.XoaAdmin,
                        $"Xóa tài khoản admin: {tenDN} - {tenAdmin}");
                }

                TempData["Success"] = "Xóa admin thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi khi xóa: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Thay đổi trạng thái admin (kích hoạt/vô hiệu hóa)
        /// </summary>
        [HttpPost]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                // Cập nhật trạng thái trong database
                data.ExecuteCommand(
                    "UPDATE ADMIN SET TRANGTHAI = CASE WHEN TRANGTHAI = 1 THEN 0 ELSE 1 END WHERE MAADMIN = {0}",
                    id
                );

                // Lấy trạng thái mới
                var newStatus = data.ExecuteQuery<bool>(
                    "SELECT TRANGTHAI FROM ADMIN WHERE MAADMIN = {0}",
                    id
                ).FirstOrDefault();

                // Ghi log
                var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                if (currentAdmin != null)
                {
                    var admin = data.ADMINs.SingleOrDefault(a => a.MAADMIN == id);
                    string action = newStatus ? ActivityType.KichHoat : ActivityType.VoHieuHoa;
                    ActivityLogger.Log(currentAdmin.MAADMIN, action,
                        $"{action} tài khoản: {admin?.TENDN}");
                }

                return Json(new { success = true, newStatus = newStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Chuẩn bị dữ liệu cho form
        /// </summary>
        private void PrepareFormData(AdminFormViewModel model)
        {
            // Danh sách loại tài khoản
            model.LoaiTaiKhoanList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Super Admin", Text = "Super Admin" },
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Moderator", Text = "Moderator" },
                new SelectListItem { Value = "Support", Text = "Support" }
            };

            // Danh sách vai trò (nếu có)
            try
            {
                var vaiTros = data.ExecuteQuery<dynamic>("SELECT MAVAITRO, TENVAITRO FROM VAI_TRO").ToList();
                model.VaiTroList = vaiTros.Select(v => new SelectListItem
                {
                    Value = v.MAVAITRO.ToString(),
                    Text = v.TENVAITRO
                }).ToList();
            }
            catch
            {
                model.VaiTroList = new List<SelectListItem>();
            }
        }

        /// <summary>
        /// Lưu file upload
        /// </summary>
        private string SaveUploadedFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return "default-avatar.png";

            var fileName = Path.GetFileName(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var path = Path.Combine(Server.MapPath("~/img/"), uniqueFileName);

            file.SaveAs(path);
            return uniqueFileName;
        }

        #endregion
    }
}
