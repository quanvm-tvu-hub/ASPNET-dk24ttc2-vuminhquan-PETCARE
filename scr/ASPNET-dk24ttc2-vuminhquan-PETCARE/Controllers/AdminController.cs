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
    public class AdminController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                // Thống kê số liệu
                ViewBag.TongDoanhThu = data.CTDONDATHANGs.Sum(n => (decimal?)n.SOLUONG * (decimal?)n.DONGIA) ?? 0;
                ViewBag.TongDonHang = data.DONDATHANGs.Count();
                ViewBag.TongSanPham = data.SANPHAMs.Count();
                ViewBag.TongKhachHang = data.KHACHHANGs.Count();

                // Lấy 5 đơn hàng mới nhất
                var recentOrders = data.DONDATHANGs.OrderByDescending(n => n.NGAYDAT).Take(5).ToList();
                return View(recentOrders);
            }
        }

        [HttpGet]
        public ActionResult dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult dangnhap(DangNhapModel model)
        {
            if (ModelState.IsValid)
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(n => n.TENDN == model.tendn && n.MATKHAU == model.matkhau);
                if (ad != null)
                {
                    ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["Taikhoanadmin"] = ad;
                    
                    // Ghi log đăng nhập
                    try
                    {
                        ActivityLogger.Log(ad.MAADMIN, ActivityType.DangNhap, "Đăng nhập vào hệ thống");
                    }
                    catch { }
                    
                    // Redirect đến trang Thống kê (Dashboard)
                    return RedirectToAction("Index", "ThongKe");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View(model);
        }

        public ActionResult thongtinadmin()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            return View();
        }

        public ActionResult dangxuat()
        {
            // Ghi log đăng xuất
            try
            {
                var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                if (currentAdmin != null)
                {
                    ActivityLogger.Log(currentAdmin.MAADMIN, ActivityType.DangXuat, "Đăng xuất khỏi hệ thống");
                }
            }
            catch { }
            
            Session.Clear();
            return RedirectToAction("Index", "Admin");
        }

        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult listadmin()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var ad = from admin in data.ADMINs select admin;
                return View(ad);
            }
        }

        [HttpGet]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ADMIN admin, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img/"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    admin.AVATAR = fileName;

                    data.ADMINs.InsertOnSubmit(admin);
                    data.SubmitChanges();
                    
                    // Ghi log
                    try
                    {
                        var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                        if (currentAdmin != null)
                        {
                            ActivityLogger.Log(currentAdmin.MAADMIN, ActivityType.TaoAdmin, 
                                $"Tạo tài khoản admin mới: {admin.TENDN} - {admin.HOTEN}");
                        }
                    }
                    catch { }
                }

                return RedirectToAction("listadmin", "Admin");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var admin = data.ADMINs.SingleOrDefault(ad => ad.MAADMIN == id);
                if (admin == null)
                {
                    return RedirectToAction("listadmin", "Admin");
                }
                return View(admin);
            }
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateInput(false)]
        public ActionResult Capnhat(int id, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(n => n.MAADMIN == id);
                if (ad == null)
                {
                    return RedirectToAction("listadmin", "Admin");
                }

                if (fileUpload == null)
                {
                    // Nếu không upload ảnh mới, giữ nguyên ảnh cũ
                    // ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                    // return View(ad);
                }
                else
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img/"), fileName);
                    fileUpload.SaveAs(path);
                    ad.AVATAR = fileName;
                }

                UpdateModel(ad);
                data.SubmitChanges();
                
                // Ghi log
                try
                {
                    var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                    if (currentAdmin != null)
                    {
                        ActivityLogger.Log(currentAdmin.MAADMIN, ActivityType.SuaAdmin,
                            $"Cập nhật thông tin admin: {ad.TENDN} - {ad.HOTEN}");
                    }
                }
                catch { }
                
                return RedirectToAction("listadmin", "Admin");
            }
        }
        
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var ad = from adm in data.ADMINs where adm.MAADMIN == id select adm;
                return View(ad.Single());
            }
        }
        
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(n => n.MAADMIN == id);
                if (ad != null)
                {
                    string tenAdmin = ad.HOTEN;
                    string tenDN = ad.TENDN;

                    // Xóa các quyền liên quan trong bảng PHANQUYEN trước khi xóa Admin
                    var phanQuyens = data.PHANQUYENs.Where(pq => pq.MAADMIN == id).ToList();
                    data.PHANQUYENs.DeleteAllOnSubmit(phanQuyens);
                    data.SubmitChanges();
                    
                    data.ADMINs.DeleteOnSubmit(ad);
                    data.SubmitChanges();
                    
                    // Ghi log
                    try
                    {
                        var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                        if (currentAdmin != null)
                        {
                            ActivityLogger.Log(currentAdmin.MAADMIN, ActivityType.XoaAdmin,
                                $"Xóa tài khoản admin: {tenDN} - {tenAdmin}");
                        }
                    }
                    catch { }
                }
                
                return RedirectToAction("listadmin", "Admin");
            }
        }
        
        [HttpGet]
        public ActionResult DoiMK(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                DoiMKadmin model = new DoiMKadmin();
                return View(model);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMK(int id, DoiMKadmin reset)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(n => n.MAADMIN == id && n.MATKHAU == reset.CheckPass);
                if (ad != null)
                {
                    ad.MATKHAU = reset.NewPassword;
                    UpdateModel(ad);
                    Session["Taikhoanadmin"] = ad;
                    data.SubmitChanges();
                    message = "Cập nhật mật khẩu mới thành công ";
                    
                    // Ghi log
                    try
                    {
                        ActivityLogger.Log(ad.MAADMIN, ActivityType.DoiMatKhau, "Đổi mật khẩu thành công");
                    }
                    catch { }
                    
                    return RedirectToAction("thongtinadmin", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Mật khẩu cũ không đúng ";
                }
            }
            else
            {
                message = "Điều gì đó không hợp lệ";
            }
            ViewBag.Message = message;
            return View(reset);
        }

        #region Tính Năng Mới - Dashboard và Thống Kê

        /// <summary>
        /// Dashboard quản lý admin với thống kê
        /// </summary>
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult Dashboard()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            var viewModel = new AdminDashboardViewModel();

            try
            {
                // Thống kê cơ bản
                viewModel.TongSoAdmin = data.ADMINs.Count();
                viewModel.AdminHoatDong = data.ADMINs.Count();
                viewModel.AdminKhongHoatDong = 0;

                // Admin mới nhất
                viewModel.AdminGanDay = data.ADMINs
                    .OrderByDescending(a => a.MAADMIN)
                    .Take(5)
                    .Select(a => new AdminItemViewModel
                    {
                        MAADMIN = a.MAADMIN,
                        HOTEN = a.HOTEN,
                        EMAIL = a.EMAIL,
                        AVATAR = a.AVATAR,
                        TRANGTHAI = true
                    }).ToList();

                // Thống kê theo loại
                viewModel.ThongKeTheoLoai = data.ADMINs
                    .GroupBy(a => a.TENLOAI)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Hoạt động gần đây
                try
                {
                    var logs = data.ExecuteQuery<dynamic>(@"
                        SELECT TOP 10 l.*, a.HOTEN as TenAdmin 
                        FROM ADMIN_ACTIVITY_LOG l
                        INNER JOIN ADMIN a ON l.MAADMIN = a.MAADMIN
                        ORDER BY l.NGAYTHUCHIEN DESC
                    ").ToList();

                    viewModel.HoatDongGanDay = logs.Select(l => new AdminActivityLogViewModel
                    {
                        MALOG = l.MALOG,
                        MAADMIN = l.MAADMIN,
                        TenAdmin = l.TenAdmin,
                        LOAIHOATDONG = l.LOAIHOATDONG,
                        MOTA = l.MOTA,
                        NGAYTHUCHIEN = l.NGAYTHUCHIEN,
                        DIACHI_IP = l.DIACHI_IP
                    }).ToList();
                }
                catch
                {
                    viewModel.HoatDongGanDay = new List<AdminActivityLogViewModel>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi khi tải thống kê: " + ex.Message;
            }

            return View(viewModel);
        }

        /// <summary>
        /// Danh sách admin nâng cao với tìm kiếm và phân trang
        /// </summary>
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult ListAdminAdvanced(string searchKeyword, string loaiTaiKhoan, int page = 1, int pageSize = 10)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            var model = new AdminListViewModel
            {
                SearchKeyword = searchKeyword,
                LoaiTaiKhoan = loaiTaiKhoan,
                CurrentPage = page,
                PageSize = pageSize
            };

            try
            {
                var query = data.ADMINs.AsQueryable();

                // Tìm kiếm
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    var keyword = searchKeyword.ToLower();
                    query = query.Where(a =>
                        a.HOTEN.ToLower().Contains(keyword) ||
                        a.EMAIL.ToLower().Contains(keyword) ||
                        a.TENDN.ToLower().Contains(keyword) ||
                        a.DIENTHOAI.Contains(keyword)
                    );
                }

                // Lọc theo loại
                if (!string.IsNullOrEmpty(loaiTaiKhoan))
                {
                    query = query.Where(a => a.TENLOAI == loaiTaiKhoan);
                }

                // Tổng số bản ghi
                model.TotalRecords = query.Count();
                model.TotalPages = (int)Math.Ceiling((double)model.TotalRecords / pageSize);

                // Phân trang
                var admins = query
                    .OrderByDescending(a => a.MAADMIN)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

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
                    TRANGTHAI = true
                }).ToList();

                // Dropdown lists
                model.LoaiTaiKhoanList = data.ADMINs
                    .Select(a => a.TENLOAI)
                    .Distinct()
                    .Select(l => new SelectListItem { Value = l, Text = l })
                    .ToList();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi: " + ex.Message;
                model.Admins = new List<AdminItemViewModel>();
            }

            return View(model);
        }

        /// <summary>
        /// Thay đổi trạng thái admin (AJAX)
        /// </summary>
        [HttpPost]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                data.ExecuteCommand(
                    "UPDATE ADMIN SET TRANGTHAI = CASE WHEN TRANGTHAI = 1 THEN 0 ELSE 1 END WHERE MAADMIN = {0}",
                    id
                );

                return Json(new { success = true, message = "Đã thay đổi trạng thái" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Xem lịch sử hoạt động
        /// </summary>
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult ActivityLog(int? adminId, string loaiHoatDong, int page = 1)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            var model = new ActivityLogListViewModel
            {
                AdminId = adminId,
                LoaiHoatDong = loaiHoatDong,
                CurrentPage = page,
                PageSize = 20
            };

            try
            {
                string sql = @"
                    SELECT l.*, a.HOTEN as TenAdmin 
                    FROM ADMIN_ACTIVITY_LOG l
                    INNER JOIN ADMIN a ON l.MAADMIN = a.MAADMIN
                    WHERE 1=1";

                var parameters = new List<object>();

                if (adminId.HasValue)
                {
                    sql += " AND l.MAADMIN = {" + parameters.Count + "}";
                    parameters.Add(adminId.Value);
                }

                if (!string.IsNullOrEmpty(loaiHoatDong))
                {
                    sql += " AND l.LOAIHOATDONG = {" + parameters.Count + "}";
                    parameters.Add(loaiHoatDong);
                }

                sql += " ORDER BY l.NGAYTHUCHIEN DESC";
                sql += " OFFSET {" + parameters.Count + "} ROWS";
                parameters.Add((page - 1) * model.PageSize);
                sql += " FETCH NEXT {" + parameters.Count + "} ROWS ONLY";
                parameters.Add(model.PageSize);

                var logs = data.ExecuteQuery<dynamic>(sql, parameters.ToArray()).ToList();

                model.Logs = logs.Select(l => new AdminActivityLogViewModel
                {
                    MALOG = l.MALOG,
                    MAADMIN = l.MAADMIN,
                    TenAdmin = l.TenAdmin,
                    LOAIHOATDONG = l.LOAIHOATDONG,
                    MOTA = l.MOTA,
                    NGAYTHUCHIEN = l.NGAYTHUCHIEN,
                    DIACHI_IP = l.DIACHI_IP
                }).ToList();

                // Dropdown lists
                model.AdminList = data.ADMINs
                    .Select(a => new SelectListItem
                    {
                        Value = a.MAADMIN.ToString(),
                        Text = a.HOTEN
                    }).ToList();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi: " + ex.Message;
                model.Logs = new List<AdminActivityLogViewModel>();
            }

            return View(model);
        }

        #endregion
    }
}