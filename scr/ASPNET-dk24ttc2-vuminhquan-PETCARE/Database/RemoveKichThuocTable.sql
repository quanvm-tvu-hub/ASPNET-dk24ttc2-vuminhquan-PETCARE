-- Script để xóa bảng KICHTHUOC khỏi database
-- Chạy script này trong SQL Server Management Studio

USE WebsiteThuCung;
GO

-- Kiểm tra và xóa bảng KICHTHUOC nếu tồn tại
IF OBJECT_ID('dbo.KICHTHUOC', 'U') IS NOT NULL
BEGIN
    PRINT 'Đang xóa bảng KICHTHUOC...'
    DROP TABLE dbo.KICHTHUOC;
    PRINT 'Đã xóa bảng KICHTHUOC thành công!'
END
ELSE
BEGIN
    PRINT 'Bảng KICHTHUOC không tồn tại trong database.'
END
GO
