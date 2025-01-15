using System.ComponentModel.DataAnnotations;

namespace ModelsLibrary
{

    //---User Access Model-----------
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }

    public class LoginResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }


    public class RoleModel
    {
        public string? userid { get; set; }
        public string? roleid { get; set; }
    }

}
