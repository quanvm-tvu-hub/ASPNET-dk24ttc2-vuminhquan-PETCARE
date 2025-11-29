using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.Models;

namespace WebsiteThuCungBento.Controllers
{
    public class ThongKeController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        // GET: ThongKe
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.TongDoanhThu = ThongKeTongDoanhThu();
                ViewBag.TongDonHang = ThongKeDonHang();
                ViewBag.TongSanPham = ThongKeSanPham();
                ViewBag.TongKhachHang = ThongKeKhachHang();
                ViewBag.TongAdmin = ThongKeNhanVien();
                return View();
            }
        }
        
        public double ThongKeDonHang()
        {
            double slDonHang = data.DONDATHANGs.Count();
            return slDonHang;
        }

        public double ThongKeSanPham()
        {
            double slSanPham = data.SANPHAMs.Count();
            return slSanPham;
        }

        public double ThongKeKhachHang()
        {
            double slKhachHang = data.KHACHHANGs.Count();
            return slKhachHang;
        }

        public double ThongKeNhanVien()
        {
            double slNhanVien = data.ADMINs.Count();
            return slNhanVien;
        }

        public decimal ThongKeTongDoanhThu()
        {
            // Sử dụng nullable int để xử lý trường hợp không có dữ liệu
            var tongDoanhThu = data.CTDONDATHANGs.Sum(n => (int?)n.SOLUONG * (int?)n.DONGIA);
            
            // Nếu null (không có đơn hàng), trả về 0
            return tongDoanhThu.HasValue ? (decimal)tongDoanhThu.Value : 0m;
        }

        public decimal ThongKeDoanhThuThang(int Thang, int Nam)
        {
            var listDH = data.DONDATHANGs.Where(n => n.NGAYDAT.Month == Thang && n.NGAYDAT.Year == Nam);
            decimal TongTien = 0;
            
            foreach(var item in listDH)
            {
                // Sử dụng nullable int để xử lý trường hợp không có chi tiết đơn hàng
                var tongDonHang = item.CTDONDATHANGs.Sum(n => (int?)n.SOLUONG * (int?)n.DONGIA);
                
                if (tongDonHang.HasValue)
                {
                    TongTien += (decimal)tongDonHang.Value;
                }
            }
            
            return TongTien;
        }
    }
}