using System.ComponentModel.DataAnnotations;
namespace Solid.Ecommerce.IdentityJWT.Authentication;
public class UserLoginModel
{
    [Required(ErrorMessage = "User Name is required...")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "Password is required...")]
    public string? Password { get; set; }
}
