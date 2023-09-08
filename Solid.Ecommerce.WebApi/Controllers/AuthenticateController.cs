using Microsoft.AspNetCore.Identity;
using Solid.Ecommerce.IdentityJWT.Authentication;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Solid.Ecommerce.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    /*Fields*/
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration; //read file appsettings.json 

    public AuthenticateController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    /*Register user has roles (admin)*/
    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] UserRegisterModel model)
    {
        //1. check exist user
        var userExist = await this._userManager.FindByNameAsync(model.UserName);
        if (userExist != null)
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ResponseHeader { Status = "Error", Message = "User already exists..." });
        //2. create new IdentiyUser
        IdentityUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName
        };
        //3. apply into db via UserManager
        var result = await this._userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseHeader { Status = "Error", Message = "User created fail, please check user information input and try again..." });
        //4. if register ok, then add roles (Admin & User and Client)
        if (!await this._roleManager.RoleExistsAsync(UserRoles.AdminRole))
        {
            await this._roleManager.CreateAsync(new IdentityRole(UserRoles.AdminRole));
        }
        if (!await this._roleManager.RoleExistsAsync(UserRoles.UserRole))
        {
            await this._roleManager.CreateAsync(new IdentityRole(UserRoles.UserRole));
        }
        if (!await this._roleManager.RoleExistsAsync(UserRoles.ClientRole))
        {
            await this._roleManager.CreateAsync(new IdentityRole(UserRoles.ClientRole));
        }
        //5. add user to each role
        if (await this._roleManager.RoleExistsAsync(UserRoles.AdminRole))
        {
            await this._userManager.AddToRoleAsync(user, UserRoles.AdminRole);
        }
        if (await this._roleManager.RoleExistsAsync(UserRoles.UserRole))
        {
            await this._userManager.AddToRoleAsync(user, UserRoles.UserRole);
        }
        if (await this._roleManager.RoleExistsAsync(UserRoles.ClientRole))
        {
            await this._userManager.AddToRoleAsync(user, UserRoles.ClientRole);
        }

        return Ok(new ResponseHeader { Status = "Success", Message = "User created successfully..." });

    }

    /*Register user*/
    [HttpPost]
    [Route("register-user")]
 
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterModel model)
    {
        //1. check exist user
        var userExist = await this._userManager.FindByNameAsync(model.UserName);
        if (userExist != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseHeader { Status = "Error", Message = "User already exists..." });
        //2. create new IdentiyUser
        IdentityUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName
        };
        //3. apply into db via UserManager
        var result = await this._userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseHeader { Status = "Error", Message = "User created fail, please check user information input and try again..." });

        return Ok(new ResponseHeader { Status = "Success", Message = "User created ok..." });

    }
    /**/
    /*Login & generated token value*/
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> UserLogin([FromBody] UserLoginModel model)
    {
        //1. check exits user
        var user = await this._userManager.FindByNameAsync(model.UserName);
        if (user != null && await this._userManager.CheckPasswordAsync(user, model.Password))
        {
            //sinh ra token key
            var userRoles = await this._userManager.GetRolesAsync(user); //get all roles of the user
            var authClaims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name,user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

                    };

            //2. If user has roles (optional) then add to claims
            foreach (var userRole in userRoles)
            {

                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            //3. render token
            var tokenValue = GeneratedToken(authClaims);
            //4. return Ok to browser
            return Ok(
                new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(tokenValue),
                    expiration = tokenValue.ValidTo
                });

        }
        return Unauthorized();
    }

    //Method for render auto Token
    private JwtSecurityToken GeneratedToken(List<Claim> authClaims)
    {
        //1. convert private key to byte array
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration["JWT:Secret"]));
        //2. render token value
        var tokenValue = new JwtSecurityToken(
            issuer: this._configuration["JWT:ValidIssuer"],
            audience: this._configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(2),
            claims: authClaims, /*Information sign*/
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        return tokenValue;
    }





}

