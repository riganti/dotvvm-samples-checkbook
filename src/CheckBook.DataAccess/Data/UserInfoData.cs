using System.ComponentModel.DataAnnotations;
using CheckBook.DataAccess.Enums;

namespace CheckBook.DataAccess.Data
{
    public class UserInfoData : IAvatarData
    {

        public int Id { get; set; }

        public UserRole UserRole { get; set; }


        [Required(ErrorMessage = "The First Name field is required.")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        [StringLength(100)]
        public string LastName { get; set; }

        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string PasswordAgain { get; set; }

        [Required(ErrorMessage = "The E-mail Address field is required.")]
        [EmailAddress(ErrorMessage = "The E-mail Address format is not valid.")]
        [StringLength(100)]
        public string Email { get; set; }

        public int UserId => Id;

        public string Name { get; set; }

        public string ImageUrl { get; set; }
    }
}
