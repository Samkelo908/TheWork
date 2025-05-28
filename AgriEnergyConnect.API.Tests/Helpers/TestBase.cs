using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriEnergyConnect.API.Tests.Helpers
{
    public abstract class TestBase
    {
        protected ControllerContext CreateControllerContextWithUser(string userId, string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            }, "TestAuthentication"));

            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
    }
}