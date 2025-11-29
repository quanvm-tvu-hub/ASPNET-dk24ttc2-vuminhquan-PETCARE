using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteThuCungBento.Models
{
    /// <summary>
    /// Model cho Vai trò
    /// </summary>
    public class VaiTro
    {
        [Key]
        public int MAVAITRO { get; set; }

        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên vai trò")]
        public string TENVAITRO { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string MOTA { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NGAYTAO { get; set; }

        // Navigation property
        public virtual ICollection<AdminVaiTro> AdminVaiTros { get; set; }
    }

    /// <summary>
    /// Model cho bảng liên kết Admin - Vai trò
    /// </summary>
    public class AdminVaiTro
    {
        [Required]
        public int MAADMIN { get; set; }

        [Required]
        public int MAVAITRO { get; set; }

        [Display(Name = "Ngày gán quyền")]
        public DateTime NGAYGANGHAN { get; set; }

        // Navigation properties
        public virtual ADMIN ADMIN { get; set; }
        public virtual VaiTro VaiTro { get; set; }
    }
}
