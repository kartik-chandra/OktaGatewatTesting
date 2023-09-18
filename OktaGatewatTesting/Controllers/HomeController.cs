using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OktaGatewatTesting.Models;
using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using Newtonsoft.Json;
using OktaGatewatTesting.Helper;

namespace OktaGatewatTesting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private OktaGatewayHelper _OktaGatewayHelper;
        private readonly IConfiguration _Configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _OktaGatewayHelper = new OktaGatewayHelper();
            _Configuration = configuration;
        }

        public IActionResult Index(UserAuthTokenModel authViewModel = null)
        {
            if (authViewModel == null)
            {
                authViewModel = new UserAuthTokenModel() { 
                    Email = "", 
                    IsAuthenticated = false 
                };
            }

            return View(authViewModel);
        }

        public IActionResult RedirectToOkta(AuthViewModel authViewModel)
        {
            string email = authViewModel.Email;
            string OktaGatewayBaseUrl = _Configuration["OktaGatewayUrl"];
            string BaseUrl = $"{Request.Scheme}://{Request.Host}";

            string returnUrl = "";
            //returnUrl = string.Format("{0}/home/loginwithoktagateway?Email={1}", BaseUrl, email.Trim());
            //returnUrl = string.Format("{0}/api/auth/login?ReturnUrl={1}&userName={2}", OktaGatewayBaseUrl, returnUrl, email);

            //returnUrl = string.Format("{0}/api/colpal/login/callback", BaseUrl);
            //returnUrl = string.Format("{0}/api/auth/login?ReturnUrl={1}", OktaGatewayBaseUrl, returnUrl);

            returnUrl = string.Format("{0}/api/colpal/login/callback?Email={1}", BaseUrl, email.Trim());
            returnUrl = string.Format("{0}/api/auth/login?ReturnUrl={1}&userName={2}", OktaGatewayBaseUrl, returnUrl, email);

            return Redirect(returnUrl);
        }

        //[HttpGet]
        //[Route("loginwithoktagateway")]
        [AllowAnonymous]
        public IActionResult LoginWithOktaGateway()
        {
            string siteName = $"{Request.Host}";

            string bToken = _Configuration["OktaGatewayToken"];
            string OktaGatewayBaseUrl = _Configuration["OktaGatewayUrl"];

            try
            {
                var rqReturnUrl = $"{Request.Scheme}://{Request.Host}/Home/index";
                var autheResultToken = HttpContext.Request.Query["AutheToken"].ToString();
                var email = HttpContext.Request.Query["Email"].ToString();

                Console.WriteLine($"autheResultToken: {autheResultToken}, email: {email}");

                AuthViewModel userAuth = new AuthViewModel();
                if (!string.IsNullOrEmpty(autheResultToken))
                {

                    Console.WriteLine($"bToken: {bToken}, SiteName: {siteName}");

                    Console.WriteLine($"Going to validate token");
                    UserAuthTokenModel userAuthModel = new UserAuthTokenModel();
                    userAuthModel = _OktaGatewayHelper.ValidateAuthenticationToken(OktaGatewayBaseUrl, "/api/auth/validatetoken", bToken, autheResultToken, siteName); //new UserAuthTokenModel() { Email = "kartik.chandra@adpeople.com", IsAuthenticated = true };
                    Console.WriteLine($"Isvalidate:{(userAuthModel == null ? false : true)}");

                    if (userAuthModel.IsAuthenticated)
                    {
                        Console.WriteLine($"userAuthModel.Email: {userAuthModel.Email}");

                        if (string.IsNullOrEmpty(email) || string.Compare(email, userAuthModel.Email, true) == 0)
                        {
                            //userAuth = new AuthViewModel()
                            //{
                            //    Email = userAuthModel.Email,
                            //    IsAutnentication = userAuth.IsAutnentication,
                            //    BaseUrl = $"{Request.Scheme}://{Request.Host}"
                            //};

                            // here .....

                            return RedirectToAction("Index", userAuthModel);
                        }
                        else
                        {
                            return RedirectToAction("Error", new ErrorViewModel() { Exception = new Exception("Invalid email address") });
                        }

                    }
                }

                return RedirectToAction("Index", new UserAuthTokenModel());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return RedirectToAction("Error", "Home", new ErrorViewModel() { Exception = ex });
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Exception = null });
        }

        private string GetSitenamefromReturnUrl(string returnUrl)
        {
            string siteName = returnUrl.Replace("http://", "").Replace("https://", "").TrimEnd('\\').TrimEnd('/');
            string[] parts = siteName.Replace("\\", "###").Replace("/", "###").Split("###", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                siteName = parts[0];
            }

            return siteName;
        }
    }
}
