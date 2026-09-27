using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Az e-mail cím megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Nem érvényes e-mail formátum.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        [MinLength(6, ErrorMessage = "A jelszónak legalább 6 karakter hosszúnak kell lennie.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(100, ErrorMessage = "A név nem lehet hosszabb 100 karakternél.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A születési dátum megadása kötelező.")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "A város megadása kötelező.")]
        public string City { get; set; }
        [Required(ErrorMessage = "A nem megadása kötelező.")]
        public string Gender {  get; set; }
    }
}
