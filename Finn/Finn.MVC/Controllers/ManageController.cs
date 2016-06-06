using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Finn.MVC.Models;
using OtpSharp;
using Base32;

namespace Finn.MVC.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
		// for cross-site requiest forgery protection
		private const string XsrfKey = "XsrfId";
		public ManageController()
        {
        }
        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

	    private ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
	        set 
            { 
                _signInManager = value; 
            }
        }

	    private ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
	        set
            {
                _userManager = value;
            }
        }

        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Пароль задан."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Настроен поставщик двухфакторной проверки подлинности."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";

            var userId = User.Identity.GetUserId();
			var user = UserManager.FindById(User.Identity.GetUserId());
			var model = new IndexViewModel
            {
                HasPassword = user.PasswordHash.Length > 0,
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
				//we use PhoneNumberConfirmed property for IsGoogleAuthenticatorEnabled as we can't extend identity model
				IsGoogleAuthenticatorEnabled = user.PhoneNumberConfirmed
			};
	        ViewBag.Login = user.UserName;
			return View(model);
        }

        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
				await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
			return View();
        }

        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if ((!ModelState.IsValid) || (user == null)) return View(model);
	        if (user.PasswordHash.Length > 0) return RedirectToAction("ChangePassword");
			var resetToken = await UserManager.GeneratePasswordResetTokenAsync(User.Identity.GetUserId());
			var result = await UserManager.ResetPasswordAsync(User.Identity.GetUserId(), resetToken, model.NewPassword);
			if (result.Succeeded)
	        {
			    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
		        return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
	        }
	        AddErrors(result);
			//error - redisplay form
			return View(model);
        }

        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "Внешнее имя входа удалено."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel { CurrentLogins = userLogins, OtherLogins = otherLogins });
        }

		// POST: /Manage/RemoveLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
		{
			ManageMessageId? message;
			var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				}
				message = ManageMessageId.RemoveLoginSuccess;
			}
			else
			{
				message = ManageMessageId.Error;
			}
			return RedirectToAction("ManageLogins", new { Message = message });
		}

		// POST: /Manage/LinkLogin
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

		// POST: /Manage/DisableGoogleAuthenticator
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DisableGoogleAuthenticator()
		{
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user == null) return RedirectToAction("Index", "Manage");
			user.PhoneNumberConfirmed = false;
			await UserManager.UpdateAsync(user);
			await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
			return RedirectToAction("Index", "Manage");
		}

		// GET: /Manage/EnableGoogleAuthenticator
		[HttpGet]
		public ActionResult EnableGoogleAuthenticator()
		{
			var secretKey = KeyGeneration.GenerateRandomKey(20);
			var userName = User.Identity.GetUserName();
			var barcodeUrl = KeyUrl.GetTotpUrl(secretKey, userName) + "&issuer=ServerMonitor";
			var model = new GoogleAuthenticatorViewModel
			{
				SecretKey = Base32Encoder.Encode(secretKey),
				BarcodeUrl = HttpUtility.UrlEncode(barcodeUrl)
			};
			return View(model);
		}

		// POST: /Manage/EnableGoogleAuthenticator
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EnableGoogleAuthenticator(GoogleAuthenticatorViewModel model)
		{
			if (!ModelState.IsValid) return View(model);
			var secretKey = Base32Encoder.Decode(model.SecretKey);
			long timeStepMatched;
			var otp = new Totp(secretKey);
			if (otp.VerifyTotp(model.Code, out timeStepMatched, new VerificationWindow(2, 2)))
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				user.PhoneNumberConfirmed = true;
				user.PhoneNumber = model.SecretKey;
				await UserManager.UpdateAsync(user);
				return RedirectToAction("Index", "Manage");
			}
			ModelState.AddModelError("Код", "Неправильный код");
			return View(model);
		}

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
	    private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

		public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }
		protected override void Dispose(bool disposing)
		{
			if (disposing && _userManager != null)
			{
				_userManager.Dispose();
				_userManager = null;
			}
			base.Dispose(disposing);
		}
	}
}