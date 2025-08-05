using System.ComponentModel.DataAnnotations;

namespace Backendd.DTOs
{
    public class DTO_UserLogin
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng hoặc email.")]
        public string UsernameOrEmail { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string Password { get; set; } = null!;
    }
}
 