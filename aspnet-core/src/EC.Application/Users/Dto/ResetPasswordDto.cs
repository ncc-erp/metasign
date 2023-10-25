using System.ComponentModel.DataAnnotations;

namespace EC.Users.Dto
{
    public class ResetPasswordDto
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
