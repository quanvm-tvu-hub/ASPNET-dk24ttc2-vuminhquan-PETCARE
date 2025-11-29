-- =============================================
-- Script: Cập nhật Database cho Quản Lý Admin
-- Mô tả: Thêm các bảng và cột mới cho tính năng quản lý admin nâng cao
-- Ngày tạo: 2025-11-26
-- =============================================

USE WebsiteThuCung;
GO

-- =============================================
-- 1. CẬP NHẬT BẢNG ADMIN
-- =============================================

-- Kiểm tra và thêm cột TRANGTHAI
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ADMIN]') AND name = 'TRANGTHAI')
BEGIN
    ALTER TABLE ADMIN ADD TRANGTHAI BIT DEFAULT 1;
    PRINT 'Đã thêm cột TRANGTHAI vào bảng ADMIN';
END
ELSE
BEGIN
    PRINT 'Cột TRANGTHAI đã tồn tại';
END
GO

-- Kiểm tra và thêm cột NGAYTAO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ADMIN]') AND name = 'NGAYTAO')
BEGIN
    ALTER TABLE ADMIN ADD NGAYTAO DATETIME DEFAULT GETDATE();
    PRINT 'Đã thêm cột NGAYTAO vào bảng ADMIN';
END
ELSE
BEGIN
    PRINT 'Cột NGAYTAO đã tồn tại';
END
GO

-- Kiểm tra và thêm cột NGAYCAPNHAT
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ADMIN]') AND name = 'NGAYCAPNHAT')
BEGIN
    ALTER TABLE ADMIN ADD NGAYCAPNHAT DATETIME NULL;
    PRINT 'Đã thêm cột NGAYCAPNHAT vào bảng ADMIN';
END
ELSE
BEGIN
    PRINT 'Cột NGAYCAPNHAT đã tồn tại';
END
GO

-- Kiểm tra và thêm cột LANDANGNHAPCUOI
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ADMIN]') AND name = 'LANDANGNHAPCUOI')
BEGIN
    ALTER TABLE ADMIN ADD LANDANGNHAPCUOI DATETIME NULL;
    PRINT 'Đã thêm cột LANDANGNHAPCUOI vào bảng ADMIN';
END
ELSE
BEGIN
    PRINT 'Cột LANDANGNHAPCUOI đã tồn tại';
END
GO

-- Cập nhật giá trị mặc định cho các bản ghi hiện có
UPDATE ADMIN SET TRANGTHAI = 1 WHERE TRANGTHAI IS NULL;
UPDATE ADMIN SET NGAYTAO = GETDATE() WHERE NGAYTAO IS NULL;
GO

-- =============================================
-- 2. TẠO BẢNG LỊCH SỬ HOẠT ĐỘNG ADMIN
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ADMIN_ACTIVITY_LOG]') AND type in (N'U'))
BEGIN
    CREATE TABLE ADMIN_ACTIVITY_LOG (
        MALOG INT IDENTITY(1,1) PRIMARY KEY,
        MAADMIN INT NOT NULL,
        LOAIHOATDONG NVARCHAR(50) NOT NULL,
        MOTA NVARCHAR(500),
        NGAYTHUCHIEN DATETIME DEFAULT GETDATE(),
        DIACHI_IP NVARCHAR(50),
        USERAGENT NVARCHAR(500),
        CONSTRAINT FK_ADMIN_ACTIVITY_LOG_ADMIN FOREIGN KEY (MAADMIN) REFERENCES ADMIN(MAADMIN)
    );
    
    -- Tạo index cho tìm kiếm nhanh
    CREATE INDEX IX_ADMIN_ACTIVITY_LOG_MAADMIN ON ADMIN_ACTIVITY_LOG(MAADMIN);
    CREATE INDEX IX_ADMIN_ACTIVITY_LOG_NGAYTHUCHIEN ON ADMIN_ACTIVITY_LOG(NGAYTHUCHIEN DESC);
    
    PRINT 'Đã tạo bảng ADMIN_ACTIVITY_LOG';
END
ELSE
BEGIN
    PRINT 'Bảng ADMIN_ACTIVITY_LOG đã tồn tại';
END
GO

-- =============================================
-- 3. TẠO BẢNG VAI TRÒ
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VAI_TRO]') AND type in (N'U'))
BEGIN
    CREATE TABLE VAI_TRO (
        MAVAITRO INT IDENTITY(1,1) PRIMARY KEY,
        TENVAITRO NVARCHAR(100) NOT NULL UNIQUE,
        MOTA NVARCHAR(500),
        NGAYTAO DATETIME DEFAULT GETDATE()
    );
    
    PRINT 'Đã tạo bảng VAI_TRO';
    
    -- Thêm dữ liệu mẫu
    INSERT INTO VAI_TRO (TENVAITRO, MOTA) VALUES 
        (N'Super Admin', N'Quản trị viên cấp cao nhất, có toàn quyền'),
        (N'Admin', N'Quản trị viên, có quyền quản lý hệ thống'),
        (N'Moderator', N'Người kiểm duyệt, quản lý nội dung'),
        (N'Support', N'Nhân viên hỗ trợ khách hàng');
    
    PRINT 'Đã thêm dữ liệu mẫu vào bảng VAI_TRO';
END
ELSE
BEGIN
    PRINT 'Bảng VAI_TRO đã tồn tại';
END
GO

-- =============================================
-- 4. TẠO BẢNG LIÊN KẾT ADMIN - VAI TRÒ
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ADMIN_VAI_TRO]') AND type in (N'U'))
BEGIN
    CREATE TABLE ADMIN_VAI_TRO (
        MAADMIN INT NOT NULL,
        MAVAITRO INT NOT NULL,
        NGAYGANGHAN DATETIME DEFAULT GETDATE(),
        PRIMARY KEY (MAADMIN, MAVAITRO),
        CONSTRAINT FK_ADMIN_VAI_TRO_ADMIN FOREIGN KEY (MAADMIN) REFERENCES ADMIN(MAADMIN) ON DELETE CASCADE,
        CONSTRAINT FK_ADMIN_VAI_TRO_VAI_TRO FOREIGN KEY (MAVAITRO) REFERENCES VAI_TRO(MAVAITRO) ON DELETE CASCADE
    );
    
    PRINT 'Đã tạo bảng ADMIN_VAI_TRO';
END
ELSE
BEGIN
    PRINT 'Bảng ADMIN_VAI_TRO đã tồn tại';
END
GO

-- =============================================
-- 5. TẠO STORED PROCEDURES
-- =============================================

-- Stored Procedure: Ghi log hoạt động
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GhiLogHoatDong]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GhiLogHoatDong];
GO

CREATE PROCEDURE sp_GhiLogHoatDong
    @MAADMIN INT,
    @LOAIHOATDONG NVARCHAR(50),
    @MOTA NVARCHAR(500),
    @DIACHI_IP NVARCHAR(50) = NULL,
    @USERAGENT NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO ADMIN_ACTIVITY_LOG (MAADMIN, LOAIHOATDONG, MOTA, DIACHI_IP, USERAGENT)
    VALUES (@MAADMIN, @LOAIHOATDONG, @MOTA, @DIACHI_IP, @USERAGENT);
END
GO

PRINT 'Đã tạo stored procedure sp_GhiLogHoatDong';
GO

-- Stored Procedure: Lấy thống kê admin
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ThongKeAdmin]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ThongKeAdmin];
GO

CREATE PROCEDURE sp_ThongKeAdmin
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        (SELECT COUNT(*) FROM ADMIN) AS TongSoAdmin,
        (SELECT COUNT(*) FROM ADMIN WHERE TRANGTHAI = 1) AS AdminHoatDong,
        (SELECT COUNT(*) FROM ADMIN WHERE TRANGTHAI = 0) AS AdminKhongHoatDong,
        (SELECT COUNT(*) FROM ADMIN WHERE LANDANGNHAPCUOI >= DATEADD(DAY, -7, GETDATE())) AS DangNhapTuan,
        (SELECT COUNT(*) FROM ADMIN WHERE NGAYTAO >= DATEADD(DAY, -30, GETDATE())) AS AdminMoiThang;
END
GO

PRINT 'Đã tạo stored procedure sp_ThongKeAdmin';
GO

-- =============================================
-- 6. TẠO VIEWS
-- =============================================

-- View: Thông tin admin đầy đủ
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_AdminDayDu]'))
    DROP VIEW [dbo].[vw_AdminDayDu];
GO

CREATE VIEW vw_AdminDayDu
AS
SELECT 
    a.MAADMIN,
    a.HOTEN,
    a.DIACHI,
    a.DIENTHOAI,
    a.TENLOAI,
    a.TENDN,
    a.AVATAR,
    a.EMAIL,
    a.TRANGTHAI,
    a.NGAYTAO,
    a.NGAYCAPNHAT,
    a.LANDANGNHAPCUOI,
    STUFF((
        SELECT ', ' + vt.TENVAITRO
        FROM ADMIN_VAI_TRO avt
        INNER JOIN VAI_TRO vt ON avt.MAVAITRO = vt.MAVAITRO
        WHERE avt.MAADMIN = a.MAADMIN
        FOR XML PATH('')
    ), 1, 2, '') AS DanhSachVaiTro,
    (SELECT COUNT(*) FROM ADMIN_ACTIVITY_LOG WHERE MAADMIN = a.MAADMIN) AS SoLanHoatDong
FROM ADMIN a;
GO

PRINT 'Đã tạo view vw_AdminDayDu';
GO

-- =============================================
-- 7. TẠO TRIGGERS
-- =============================================

-- Trigger: Tự động cập nhật NGAYCAPNHAT khi sửa admin
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trg_Admin_Update]'))
    DROP TRIGGER [dbo].[trg_Admin_Update];
GO

CREATE TRIGGER trg_Admin_Update
ON ADMIN
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE ADMIN
    SET NGAYCAPNHAT = GETDATE()
    WHERE MAADMIN IN (SELECT MAADMIN FROM inserted);
END
GO

PRINT 'Đã tạo trigger trg_Admin_Update';
GO

-- =============================================
-- HOÀN TẤT
-- =============================================

PRINT '';
PRINT '========================================';
PRINT 'CẬP NHẬT DATABASE HOÀN TẤT!';
PRINT '========================================';
PRINT 'Đã thêm:';
PRINT '- 4 cột mới vào bảng ADMIN';
PRINT '- 3 bảng mới: ADMIN_ACTIVITY_LOG, VAI_TRO, ADMIN_VAI_TRO';
PRINT '- 2 stored procedures';
PRINT '- 1 view';
PRINT '- 1 trigger';
PRINT '========================================';
GO
