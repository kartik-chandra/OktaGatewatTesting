using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OktaGatewatTesting.Helper;
using OktaGatewatTesting.Models;

namespace OktaGatewatTesting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColpalController : ControllerBase
    {
        private readonly ILogger<ColpalController> _logger;
        private OktaGatewayHelper _OktaGatewayHelper;
        private readonly IConfiguration _Configuration;

        public ColpalController(ILogger<ColpalController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _OktaGatewayHelper = new OktaGatewayHelper();
            _Configuration = configuration;
        }

        //[HttpGet]
        [Route("login/callback")]
        [AllowAnonymous]
        public IActionResult LoginCallback()
        {
            string siteName = $"{Request.Host}";

            string bToken = _Configuration["OktaGatewayToken"];
            string OktaGatewayBaseUrl = _Configuration["OktaGatewayUrl"];

            try
            {
                var autheResultToken = HttpContext.Request.Query["AutheToken"].ToString();

                Console.WriteLine($"autheResultToken: {autheResultToken}");

                AuthViewModel userAuth = new AuthViewModel();
                if (!string.IsNullOrEmpty(autheResultToken))
                {

                    Console.WriteLine($"bToken: {bToken}, SiteName: {siteName}");

                    Console.WriteLine($"Going to validate token");
                    UserAuthTokenModel userAuthModel = new UserAuthTokenModel();
                    userAuthModel = _OktaGatewayHelper.ValidateAuthenticationToken(OktaGatewayBaseUrl, "/api/auth/validatetoken", bToken, autheResultToken, siteName); 
                    Console.WriteLine($"Isvalidate:{(userAuthModel == null ? false : true)}");

                    if (userAuthModel.IsAuthenticated)
                    {
                        Console.WriteLine($"userAuthModel.Email: {userAuthModel.Email}");
                        return RedirectToAction("Index", "Home", userAuthModel);
                    }
                }

                return RedirectToAction("Index", "Home", new UserAuthTokenModel());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return RedirectToAction("Error", "Home", new ErrorViewModel() { Exception = ex });
            }
        }

    }
}
