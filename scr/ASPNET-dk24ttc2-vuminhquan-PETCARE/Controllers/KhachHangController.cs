using WebsiteThuCungBento.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;

namespace WebsiteThuCungBento.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_KHACHHANG")]
    public class KhachHangController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        
        // GET: KhachHang
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var kh = from k in data.KHACHHANGs select k;
                return View(kh);
            }
        }

        public ActionResult Details(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var kh = from l in data.KHACHHANGs where l.MAKH == id select l;
                return View(kh.Single());
            }
        }

        [HttpGet]
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
        public ActionResult Create(KHACHHANG kh, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                if (fileUpload == null)
                {
                    ViewBag.Thongbao = "Vui lòng chọn ảnh đại diện";
                    return View(kh);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/img/"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            // Ảnh đã tồn tại, vẫn sử dụng tên file này
                        }
                        else
                        {
                            fileUpload.SaveAs(path);
                        }
                        kh.HINHANH = fileName;
                        data.KHACHHANGs.InsertOnSubmit(kh);
                        data.SubmitChanges();
                        return RedirectToAction("Index", "KhachHang");
                    }
                }
                // Nếu không hợp lệ, trả về view để hiển thị lỗi
                return View(kh);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var kh = data.KHACHHANGs.SingleOrDefault(l => l.MAKH == id);
                if (kh == null)
                {
                    return RedirectToAction("Index", "KhachHang");
                }
                return View(kh);
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
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);
                if (kh == null)
                {
                    return RedirectToAction("Index", "KhachHang");
                }

                if (fileUpload != null)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img/"), fileName);
                    fileUpload.SaveAs(path);
                    kh.HINHANH = fileName;
                }
                
                UpdateModel(kh);
                data.SubmitChanges();
                return RedirectToAction("Index", "KhachHang");
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var kh = from nxb in data.KHACHHANGs where nxb.MAKH == id select nxb;
                return View(kh.Single());
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                try
                {
                    KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);
                    
                    if (kh != null)
                    {
                        // Xóa tất cả đơn đăng ký dịch vụ của khách hàng
                        var dangKyDichVus = data.DangKyDichVus.Where(dv => dv.MaKH == id);
                        data.DangKyDichVus.DeleteAllOnSubmit(dangKyDichVus);
                        
                        // Lấy tất cả đơn hàng của khách hàng
                        var donHangs = data.DONDATHANGs.Where(dh => dh.MAKH == id);
                        
                        foreach (var donHang in donHangs)
                        {
                            // Xóa chi tiết đơn hàng trước
                            var chiTietDonHangs = data.CTDONDATHANGs.Where(ct => ct.MADH == donHang.MADH);
                            data.CTDONDATHANGs.DeleteAllOnSubmit(chiTietDonHangs);
                        }
                        
                        // Xóa các đơn hàng
                        data.DONDATHANGs.DeleteAllOnSubmit(donHangs);
                        
                        // Cuối cùng xóa khách hàng
                        data.KHACHHANGs.DeleteOnSubmit(kh);
                        
                        // Lưu tất cả thay đổi
                        data.SubmitChanges();
                        
                        TempData["SuccessMessage"] = "Đã xóa khách hàng và tất cả đơn hàng liên quan thành công!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi khi xóa khách hàng: " + ex.Message;
                }
                
                return RedirectToAction("Index", "KhachHang");
            }
        }
    }
}