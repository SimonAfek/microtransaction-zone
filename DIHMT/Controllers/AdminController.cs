using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIHMT.Models;
using DIHMT.Static;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace DIHMT.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private AppUserManager _userManager;
        private RoleManager<AppRole> _roleManager;
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public AppUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            private set => _userManager = value;
        }

        public RoleManager<AppRole> RoleManager
        {
            get => _roleManager ?? HttpContext.GetOwinContext().GetUserManager<RoleManager<AppRole>>();
            private set => _roleManager = value;
        }

        public AdminController()
        {
        }

        public AdminController(AppUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public AdminController(AppUserManager userManager, ApplicationSignInManager signInManager, RoleManager<AppRole> roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        // GET: /Admin/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Admin/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    //return View("Lockout");
                case SignInStatus.RequiresVerification:
                    //return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        // GET: /Admin/Register
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Admin/Register
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return RedirectToAction("Index", "Home");
            }

            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateRole(string roleName)
        {
            if (!_roleManager.RoleExists(roleName))
            {
                _roleManager.Create(new AppRole(roleName));
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult AddRoleToUser(string username, string roleName)
        {
            try
            {
                UserManager.AddToRole(UserManager.FindByName(username).Id, roleName);
            }
            catch
            {
                return View("Error");
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: /Admin/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Pending(int? id)
        {
            var pending = id.HasValue ? GameHelpers.GetPendingSubmissionWithCurrentRating(id.Value) : null;

            if (pending != null)
            {
                return View(pending);
            }

            var pendingRatings = GameHelpers.GetPendingSubmissionsList();

            return View("Pending_List", pendingRatings);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Pending(PendingDisplayModel input)
        {
            if (input.RatingModel.Valid)
            {
                GameHelpers.HandlePostPending(input);
            }

            return RedirectToAction("Index", "Game", new { id = input.GameId });
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
