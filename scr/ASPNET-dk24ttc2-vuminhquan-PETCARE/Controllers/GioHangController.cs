using WebsiteThuCungBento.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteThuCungBento.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        DataClassesDataContext data = new DataClassesDataContext();
        public List<Giohang> Laygiohang()
        {
            List<Giohang> dsGiohang = Session["Giohang"] as List<Giohang>;
            if (dsGiohang == null)
            {
                dsGiohang = new List<Giohang>();
                Session["Giohang"] = dsGiohang;
            }
            return dsGiohang;
        }
        //Them hang vao gio
        public ActionResult ThemGiohang(int iMASP, string strURL)
        {
            List<Giohang> dsGiohang = Laygiohang();
            Giohang sanpham = dsGiohang.Find(n => n.iMASP == iMASP);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMASP);
                dsGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSOLUONG++;
                return Redirect(strURL);
            }
        }


        //Xay dung trang Gio hang
        public ActionResult GioHang()
        {
            List<Giohang> dsGiohang = Laygiohang();
            if (dsGiohang.Count == 0)
            {
                return RedirectToAction("Index", "User");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(dsGiohang);
        }
        //Tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> dsGiohang = Session["GioHang"] as List<Giohang>;
            if (dsGiohang != null)
            {
                iTongSoLuong = dsGiohang.Sum(n => n.iSOLUONG);
            }
            return iTongSoLuong;
        }
        //Tinh tong tien
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> dsGiohang = Session["GioHang"] as List<Giohang>;
            if (dsGiohang != null)
            {
                iTongTien = dsGiohang.Sum(n => n.dTHANHTIEN);
            }
            return iTongTien;
        }

        //Tao Partial view de hien thi thong tin gio hang
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }

        //Cap nhat Giỏ hàng
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            List<Giohang> dsGiohang = Laygiohang();
            Giohang sanpham = dsGiohang.SingleOrDefault(n => n.iMASP == iMaSP);
            if (sanpham != null)
            {
                sanpham.iSOLUONG = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }
        //Xoa Giohang
        public ActionResult XoaGiohang(int iMaSP)
        {
            List<Giohang> dsGiohang = Laygiohang();
            Giohang sanpham = dsGiohang.SingleOrDefault(n => n.iMASP == iMaSP);
            if (sanpham != null)
            {
                dsGiohang.RemoveAll(n => n.iMASP == iMaSP);
                return RedirectToAction("GioHang");

            }
            if (dsGiohang.Count == 0)
            {
                return RedirectToAction("index", "User");
            }
            return RedirectToAction("GioHang");
        }
        //Xoa tat ca thong tin trong Gio hang
        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> dsGiohang = Laygiohang();
            dsGiohang.Clear();
            return RedirectToAction("index", "User");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "User");
            }

            //Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return View(lstGiohang);
        }

        #region Thêm đơn đặt hàng mới
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            //Them Don hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> gh = Laygiohang();
            ddh.MAKH = kh.MAKH;
            ddh.NGAYDAT = DateTime.Now;
            if (collection["Ngaygiao"].Equals(""))
            {
                DateTime aDateTime = DateTime.Now;
                DateTime newTime = aDateTime.AddDays(7);
                ddh.NGAYGIAO = newTime;
            }
            else
            {
                var ngaygiao = String.Format("{0:dd/MM/yyyy}", collection["Ngaygiao"]);
                ddh.NGAYGIAO = DateTime.Parse(ngaygiao);
            }

            ddh.TINHTRANGDH = false;

            int HTTH = int.Parse(collection["sl_ThanhToan"]);
            if (HTTH == 0)
                ddh.DATHANHTOAN = false;
            else
                ddh.DATHANHTOAN = true;

            ddh.TONGTIEN = (decimal)TongTien();
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach (var item in gh)
            {
                SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == item.iMASP);
                if (sanpham.SOLUONG >= item.iSOLUONG)
                {
                    CTDONDATHANG ctdh = new CTDONDATHANG();
                    ctdh.MADH = ddh.MADH;
                    ctdh.MASP = item.iMASP;
                    ctdh.SOLUONG = item.iSOLUONG;
                    ctdh.DONGIA = (int)item.dDONGIA;
                    ctdh.THANHTIEN = (decimal)item.dTHANHTIEN;
                    data.CTDONDATHANGs.InsertOnSubmit(ctdh);
                    sanpham.SOLUONG = sanpham.SOLUONG - item.iSOLUONG;
                    data.SubmitChanges();
                    Session["Giohang"] = null;
                }
                else
                {
                    return RedirectToAction("ThongBao", "Giohang");
                }

            }
            return RedirectToAction("Xacnhandonhang", "Giohang");

        }
        #endregion



        public ActionResult ThongBao()
        {
            return View();
        }
            
        public ActionResult Xacnhandonhang()
        {
            var dh = from d in data.DONDATHANGs select d.NGAYGIAO;
            return View(dh);
        }

        #region Lấy hình thương hiệu
        public ActionResult hinhthuonghieu()
        {
            var listthuonghieu = from THUONGHIEU in data.THUONGHIEUs select THUONGHIEU;
            return PartialView(listthuonghieu);
        }
        #endregion
    }
}