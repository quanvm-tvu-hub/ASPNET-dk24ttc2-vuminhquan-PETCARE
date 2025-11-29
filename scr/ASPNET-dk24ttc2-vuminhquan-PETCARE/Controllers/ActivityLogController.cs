using WebsiteThuCungBento.Models;
using WebsiteThuCungBento.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;

namespace WebsiteThuCungBento.Controllers
{
    /// <summary>
    /// Controller quản lý lịch sử hoạt động của admin
    /// </summary>
    [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
    public class ActivityLogController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();

        /// <summary>
        /// Danh sách lịch sử hoạt động với tìm kiếm và lọc
        /// </summary>
        public ActionResult Index(ActivityLogListViewModel model)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            try
            {
                // Query cơ bản
                string sql = @"
                    SELECT l.*, a.HOTEN as TenAdmin 
                    FROM ADMIN_ACTIVITY_LOG l
                    INNER JOIN ADMIN a ON l.MAADMIN = a.MAADMIN
                    WHERE 1=1";

                var parameters = new List<object>();

                // Lọc theo admin
                if (model.AdminId.HasValue)
                {
                    sql += " AND l.MAADMIN = {" + parameters.Count + "}";
                    parameters.Add(model.AdminId.Value);
                }

                // Lọc theo loại hoạt động
                if (!string.IsNullOrEmpty(model.LoaiHoatDong))
                {
                    sql += " AND l.LOAIHOATDONG = {" + parameters.Count + "}";
                    parameters.Add(model.LoaiHoatDong);
                }

                // Lọc theo khoảng thời gian
                if (model.TuNgay.HasValue)
                {
                    sql += " AND l.NGAYTHUCHIEN >= {" + parameters.Count + "}";
                    parameters.Add(model.TuNgay.Value);
                }

                if (model.DenNgay.HasValue)
                {
                    sql += " AND l.NGAYTHUCHIEN <= {" + parameters.Count + "}";
                    parameters.Add(model.DenNgay.Value.AddDays(1)); // Bao gồm cả ngày cuối
                }

                // Đếm tổng số bản ghi
                string countSql = "SELECT COUNT(*) FROM (" + sql + ") AS CountQuery";
                model.TotalRecords = data.ExecuteQuery<int>(countSql, parameters.ToArray()).FirstOrDefault();
                model.TotalPages = (int)Math.Ceiling((double)model.TotalRecords / model.PageSize);

                // Sắp xếp và phân trang
                sql += " ORDER BY l.NGAYTHUCHIEN DESC";
                sql += " OFFSET {" + parameters.Count + "} ROWS";
                parameters.Add((model.CurrentPage - 1) * model.PageSize);
                sql += " FETCH NEXT {" + parameters.Count + "} ROWS ONLY";
                parameters.Add(model.PageSize);

                // Lấy dữ liệu
                var logs = data.ExecuteQuery<dynamic>(sql, parameters.ToArray()).ToList();

                model.Logs = logs.Select(l => new AdminActivityLogViewModel
                {
                    MALOG = l.MALOG,
                    MAADMIN = l.MAADMIN,
                    TenAdmin = l.TenAdmin,
                    LOAIHOATDONG = l.LOAIHOATDONG,
                    MOTA = l.MOTA,
                    NGAYTHUCHIEN = l.NGAYTHUCHIEN,
                    DIACHI_IP = l.DIACHI_IP,
                    USERAGENT = l.USERAGENT
                }).ToList();

                // Danh sách cho dropdown
                PrepareDropdownLists(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi khi tải lịch sử: " + ex.Message;
                model.Logs = new List<AdminActivityLogViewModel>();
            }

            return View(model);
        }

        /// <summary>
        /// Xem chi tiết lịch sử hoạt động của một admin
        /// </summary>
        public ActionResult AdminHistory(int id, int page = 1)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            var admin = data.ADMINs.SingleOrDefault(a => a.MAADMIN == id);
            if (admin == null)
            {
                TempData["Error"] = "Không tìm thấy admin";
                return RedirectToAction("Index");
            }

            ViewBag.AdminName = admin.HOTEN;
            ViewBag.AdminId = id;

            var model = new ActivityLogListViewModel
            {
                AdminId = id,
                CurrentPage = page
            };

            return Index(model);
        }

        /// <summary>
        /// Xuất báo cáo lịch sử hoạt động
        /// </summary>
        public ActionResult ExportReport(ActivityLogListViewModel model)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            try
            {
                // TODO: Implement export to Excel/PDF
                TempData["Info"] = "Tính năng xuất báo cáo đang được phát triển";
                return RedirectToAction("Index", model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi khi xuất báo cáo: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Xóa log cũ (chỉ Super Admin)
        /// </summary>
        [HttpPost]
        public JsonResult DeleteOldLogs(int days = 90)
        {
            try
            {
                var currentAdmin = Session["Taikhoanadmin"] as ADMIN;
                if (currentAdmin == null || currentAdmin.TENLOAI != "Super Admin")
                {
                    return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này" });
                }

                var cutoffDate = DateTime.Now.AddDays(-days);
                data.ExecuteCommand(
                    "DELETE FROM ADMIN_ACTIVITY_LOG WHERE NGAYTHUCHIEN < {0}",
                    cutoffDate
                );

                return Json(new { success = true, message = $"Đã xóa log cũ hơn {days} ngày" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Thống kê hoạt động theo thời gian
        /// </summary>
        public ActionResult Statistics()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");

            try
            {
                // Thống kê theo loại hoạt động
                var statsByType = data.ExecuteQuery<dynamic>(@"
                    SELECT LOAIHOATDONG, COUNT(*) as SoLuong
                    FROM ADMIN_ACTIVITY_LOG
                    WHERE NGAYTHUCHIEN >= DATEADD(MONTH, -1, GETDATE())
                    GROUP BY LOAIHOATDONG
                    ORDER BY SoLuong DESC
                ").ToList();

                ViewBag.StatsByType = statsByType;

                // Thống kê theo admin
                var statsByAdmin = data.ExecuteQuery<dynamic>(@"
                    SELECT TOP 10 a.HOTEN, COUNT(*) as SoLuong
                    FROM ADMIN_ACTIVITY_LOG l
                    INNER JOIN ADMIN a ON l.MAADMIN = a.MAADMIN
                    WHERE l.NGAYTHUCHIEN >= DATEADD(MONTH, -1, GETDATE())
                    GROUP BY a.HOTEN
                    ORDER BY SoLuong DESC
                ").ToList();

                ViewBag.StatsByAdmin = statsByAdmin;

                // Thống kê theo ngày (7 ngày gần nhất)
                var statsByDay = data.ExecuteQuery<dynamic>(@"
                    SELECT CAST(NGAYTHUCHIEN AS DATE) as Ngay, COUNT(*) as SoLuong
                    FROM ADMIN_ACTIVITY_LOG
                    WHERE NGAYTHUCHIEN >= DATEADD(DAY, -7, GETDATE())
                    GROUP BY CAST(NGAYTHUCHIEN AS DATE)
                    ORDER BY Ngay
                ").ToList();

                ViewBag.StatsByDay = statsByDay;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi khi tải thống kê: " + ex.Message;
            }

            return View();
        }

        #region Helper Methods

        /// <summary>
        /// Chuẩn bị dữ liệu cho dropdown
        /// </summary>
        private void PrepareDropdownLists(ActivityLogListViewModel model)
        {
            // Danh sách admin
            model.AdminList = data.ADMINs
                .OrderBy(a => a.HOTEN)
                .Select(a => new SelectListItem
                {
                    Value = a.MAADMIN.ToString(),
                    Text = a.HOTEN
                })
                .ToList();

            // Danh sách loại hoạt động
            try
            {
                var loaiHoatDong = data.ExecuteQuery<string>(
                    "SELECT DISTINCT LOAIHOATDONG FROM ADMIN_ACTIVITY_LOG ORDER BY LOAIHOATDONG"
                ).ToList();

                model.LoaiHoatDongList = loaiHoatDong.Select(l => new SelectListItem
                {
                    Value = l,
                    Text = l
                }).ToList();
            }
            catch
            {
                model.LoaiHoatDongList = new List<SelectListItem>();
            }
        }

        #endregion
    }
}
