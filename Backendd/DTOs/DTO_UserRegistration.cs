using System.ComponentModel.DataAnnotations;

namespace Backendd.DTOs
{
    public class DTO_UserRegistration
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = null!;
    }
}
