using Quiz_WebPlatform.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Quiz_WebPlatform.Controllers
{
    public class AccountController : Controller
    {
        private QuizWebPlatformEntities _dbContext;

        public AccountController()
        {
            _dbContext = new QuizWebPlatformEntities();
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public ActionResult Index(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User user = _dbContext.Users.SingleOrDefault(item => item.UserName == viewModel.UserName);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This user doesn't exist!");
                }
                else
                {
                    if (user.UserPassword != viewModel.UserPassword)
                    {
                        ModelState.AddModelError(string.Empty, "Name/Password is invalid!");
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(viewModel.UserName, false);
                        var authTicket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddMinutes(20), false, "User");

                        string encryptTicket = FormsAuthentication.Encrypt(authTicket);

                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptTicket);
                        HttpContext.Response.Cookies.Add(authCookie);

                        Session["UserId"] = user.UserId;
                        Session["QuizId"] = user.QuizId;

                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "All fields must be filled correctly!");
            }

            return View();
        }

        public ActionResult Logout()
        {
            if (Session["UserAnswers"] != null)
                Session["UserAnswers"] = null;

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}