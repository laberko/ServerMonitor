using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MySql.AspNet.Identity;
using OtpSharp;
using Base32;

namespace Finn.MVC.Models
{
    public class ApplicationUser : IdentityUser
    {
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
	public class GoogleAuthenticatorTokenProvider : IUserTokenProvider<ApplicationUser, string>
	{
		//provider does not actually generate a code
		public Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser, string> manager, ApplicationUser user)
		{
			return Task.FromResult((string)null);
		}
		public Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser, string> manager, ApplicationUser user)
		{
			long timeStepMatched;
			var otp = new Totp(Base32Encoder.Decode(user.PhoneNumber));
			var valid = otp.VerifyTotp(token, out timeStepMatched, new VerificationWindow(2, 2));
			return Task.FromResult(valid);
		}
		//no need to send a notification
		public Task NotifyAsync(string token, UserManager<ApplicationUser, string> manager, ApplicationUser user)
		{
			return Task.FromResult(true);
		}
		//indicates whether this user can use Google Authenticator as a two factor authentication mechanism
		public Task<bool> IsValidProviderForUserAsync(UserManager<ApplicationUser, string> manager, ApplicationUser user)
		{
			return Task.FromResult(user.PhoneNumberConfirmed);
		}
	}
}