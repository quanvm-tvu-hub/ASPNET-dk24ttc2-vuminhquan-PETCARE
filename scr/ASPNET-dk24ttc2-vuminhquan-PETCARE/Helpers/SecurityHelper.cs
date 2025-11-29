using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WebsiteThuCungBento.Helpers
{
    /// <summary>
    /// Helper class cho các chức năng bảo mật
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Hash mật khẩu sử dụng SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                
                return builder.ToString();
            }
        }

        /// <summary>
        /// Kiểm tra mật khẩu có khớp với hash không
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hashedPassword) == 0;
        }

        /// <summary>
        /// Kiểm tra độ mạnh của mật khẩu
        /// </summary>
        /// <returns>
        /// 0 = Yếu
        /// 1 = Trung bình
        /// 2 = Mạnh
        /// 3 = Rất mạnh
        /// </returns>
        public static int CheckPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return 0;

            int score = 0;

            // Độ dài
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;

            // Có chữ hoa
            if (Regex.IsMatch(password, @"[A-Z]")) score++;

            // Có chữ thường
            if (Regex.IsMatch(password, @"[a-z]")) score++;

            // Có số
            if (Regex.IsMatch(password, @"[0-9]")) score++;

            // Có ký tự đặc biệt
            if (Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]")) score++;

            // Tính điểm cuối cùng
            if (score <= 2) return 0; // Yếu
            if (score <= 3) return 1; // Trung bình
            if (score <= 4) return 2; // Mạnh
            return 3; // Rất mạnh
        }

        /// <summary>
        /// Lấy mô tả độ mạnh mật khẩu
        /// </summary>
        public static string GetPasswordStrengthText(int strength)
        {
            switch (strength)
            {
                case 0: return "Yếu";
                case 1: return "Trung bình";
                case 2: return "Mạnh";
                case 3: return "Rất mạnh";
                default: return "Không xác định";
            }
        }

        /// <summary>
        /// Tạo mật khẩu ngẫu nhiên
        /// </summary>
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            return password.ToString();
        }

        /// <summary>
        /// Validate email
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Sử dụng regex để validate email
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate số điện thoại Việt Nam
        /// </summary>
        public static bool IsValidVietnamesePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Loại bỏ khoảng trắng và dấu gạch ngang
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            // Kiểm tra định dạng số điện thoại Việt Nam
            // Bắt đầu bằng 0 hoặc +84, theo sau là 9-10 chữ số
            var regex = new Regex(@"^(0|\+84)[0-9]{9,10}$");
            return regex.IsMatch(phoneNumber);
        }
    }
}
