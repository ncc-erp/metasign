using System.ComponentModel.DataAnnotations;

namespace EC.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}