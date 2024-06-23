using Microservices.Identity.Extensions;
using Microservices.Identity.Models;
using Microsoft.AspNetCore.Identity;
using NetDevPack.Identity.Interfaces;

namespace Microservices.Identity.Routes;

public static class RoutesExtension
{
    public static IEndpointRouteBuilder MapRoutes(this IEndpointRouteBuilder endpoints,
        UserManager<IdentityUser> userManager,
        IJwtBuilder _jwtBuilder, SignInManager<IdentityUser> SignInManager)
    {
        var routes = endpoints.MapGroup("/v1");

        routes.MapPost("/register", async (UserRegister userRegister) =>
        {
            var validation = Validation.Validate(userRegister);
            if (validation.Any()) return Results.BadRequest(validation);

            var user = new IdentityUser
            {
                UserName = userRegister.Email,
                Email = userRegister.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, userRegister.Password);
            if (result.Succeeded)
            {

                var jwt = await _jwtBuilder
                    .WithEmail(userRegister.Email)
                    .WithJwtClaims()
                    .WithUserClaims()
                    .WithUserRoles()
                    .WithRefreshToken()
                    .BuildUserResponse();

                return Results.Created();
            }

            return Results.BadRequest(result.Errors);
        });

        routes.MapPost("/login", async (UserLogin userLogin) =>
        {
            var result = await SignInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password,
                false, true);

            if (result.Succeeded)
            {
                var jwt = await _jwtBuilder
                    .WithEmail(userLogin.Email)
                    .WithJwtClaims()
                    .WithUserClaims()
                    .WithUserRoles()
                    .WithRefreshToken()
                    .BuildUserResponse();
                return Results.Ok(jwt);
            }

            if (result.IsLockedOut)
            {
                Results.BadRequest("User temporarily blocked due to multiple login attempts. Try again later.");
            }

            return Results.BadRequest("Invalid user or password.");
        });

        return endpoints;
    }
}
