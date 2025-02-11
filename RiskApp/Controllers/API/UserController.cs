using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiskApp.Extensions;
using RiskApp.Models.User;
using RiskApp.Repositories;
using RiskApp.Services;

namespace RiskApp.Controllers.API
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ProfileService profileService;

        public UserController(ProfileService profileService)
        {
            this.profileService = profileService;
        }

        [HttpGet("info")]
        public ActionResult<UserInfo> GetUserInfo()
        {
            //perf hack: storing user Id in the name property on Identity.
            Guid userProfileId = User.CurrentUserId();
            UserInfo userInfo = profileService.GetUserInfo(userProfileId);
            if(userInfo != null)
            {
                return Ok(userInfo);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Gets the current logged in users Roles
        /// </summary>

        [HttpGet("roles")]
        public string[] Roles()
        {
            IEnumerable<Claim> claims = User.Claims;

            string[] roles = RolesFromClaims(claims);
            return roles;
        }

        [HttpGet("profile")]
        public Profile GetProfile()
        {
            Guid currentUser = User.CurrentUserId();
            return profileService.GetProfile(currentUser);
        }

        [HttpPut("profile")]
        public void UpdateProfile([FromBody] Profile profile)
        {
            // so that the update will only affect the current user.
            // Prevents user from setting values on another user
            profile.Id = User.CurrentUserId();
            profileService.UpdateProfile(profile);
        }

        private string[]  RolesFromClaims(IEnumerable<Claim> claims)
        {
             return claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value).ToArray();
        }
    }
}
