using System;
using System.Configuration;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Finn.MVC.Models;
using MySql.AspNet.Identity;
using SendGrid;

namespace Finn.MVC
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
			await configSendGridasync(message);
		}
		//configure email sending via SendGrid
		private async Task configSendGridasync(IdentityMessage message)
		{
			var myMessage = new SendGridMessage();
			myMessage.AddTo(message.Destination);
			myMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["mailAddress"], "Server Monitor");
			myMessage.Subject = message.Subject;
			myMessage.Text = message.Body;
			myMessage.Html = message.Body;
			//get credentials from web.config
			var credentials = new NetworkCredential(
					   ConfigurationManager.AppSettings["mailAccount"],
					   ConfigurationManager.AppSettings["mailPassword"]
					   );
			// create a web transport for sending email
			var transportWeb = new Web(credentials);
			await transportWeb.DeliverAsync(myMessage);
		}
	}

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
			var manager = new ApplicationUserManager(new MySqlUserStore<ApplicationUser>());
			// login validation settings
			manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // password validation settings
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };
            // lockout settings
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
			//two factor providers
			manager.RegisterTwoFactorProvider("Google Authenticator", new GoogleAuthenticatorTokenProvider());
			manager.RegisterTwoFactorProvider("Код из письма", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Код безопасности",
                BodyFormat = "Ваш код безопасности: {0}"
            });
            manager.EmailService = new EmailService();
			var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
