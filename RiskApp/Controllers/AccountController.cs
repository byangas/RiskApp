using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RiskApp.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using RiskApp.Extensions;
using System.Dynamic;
using RiskApp.Models.System;
using RiskApp.Models.User;

namespace RiskApp.Controllers
{
    public class AccountController : Controller
    {
        private const string cookieAuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        private readonly UserAccountService accountService;
        private readonly RegistrationService registrationService;

        public AccountController(UserAccountService accountService, RegistrationService registrationService)
        {
            this.accountService = accountService;
            this.registrationService = registrationService;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View(new List<Models.System.ResponseMessage>());
        }

        // this path in the controller is for when users click on the link in the email that is sent out
        [HttpGet]
        public IActionResult SendRegistrationCode()
        {
            dynamic model = new ExpandoObject();
            model.email = "";
            model.registrationCode = "";
            model.messages = new List<ResponseMessage>();
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string email, string code)
        {
            dynamic model = new ExpandoObject();
            model.email = email;
            model.code = code;
            model.firstName = "";
            model.lastName = "";
            model.phone = "";
            model.messages = new List<ResponseMessage>();
            return View(model);
        }


        [HttpPost]
        public IActionResult Register(string code, string email, string firstName, string lastName, string phone, string password)
        {
            dynamic model = new ExpandoObject();
            model.email = email;
            model.code = code;
            model.firstName = firstName;
            model.lastName = lastName;
            model.phone = phone;

            var messages = new List<ResponseMessage>();
            model.messages = messages;

            if (string.IsNullOrWhiteSpace(code))
            {
                messages.Add(ResponseMessage.Create("Registration Code is required."));
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                messages.Add(ResponseMessage.Create("Registration Code is required."));
            }
            else if (string.IsNullOrWhiteSpace(firstName))
            {
                messages.Add(ResponseMessage.Create("First Name is required."));
            }
            else if (string.IsNullOrWhiteSpace(lastName))
            {
                messages.Add(ResponseMessage.Create("Last Name is required."));
            }
            else if (string.IsNullOrWhiteSpace(phone))
            {
                messages.Add(ResponseMessage.Create("Phone is required."));
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                messages.Add(ResponseMessage.Create("Password is required."));
            }
            else
            {
                //validate registration code
                Registration registration = registrationService.GetValidRegistration(email, code);
                if (registration == null)
                {
                    messages.Add(ResponseMessage.Create("Valid Registration Code not found. Please request new registration code."));
                }
                else
                {
                    // todo: validate password strength
                    if (password.Length < 8)
                    {
                        messages.Add(ResponseMessage.Create("Password must be greater than 8 characters."));
                    }
                    else
                    {
                        accountService.CreateAccountFromRegistration(registration, password, email, firstName, lastName, phone);
                        Response.Redirect("/");
                    }

                }
            }


            return View(model);
        }



        // this path is used when the user starts the "send registration code" from login page, and also when
        // clicking "continue registration" from this view.
        [HttpPost]
        public IActionResult SendRegistrationCode(IFormCollection collection)
        {

            string registrationEmail = collection["email"];


            List<ResponseMessage> messages = new List<ResponseMessage>();

            dynamic model = new ExpandoObject();
            model.email = registrationEmail;
            model.messages = messages;

            //if only email is provided, then send new registration code
            if (string.IsNullOrEmpty(registrationEmail))
            {
                messages.Add(ResponseMessage.Create("Email is required"));
            }
            else
            {
                messages.AddRange(registrationService.SendRegistrationCode(registrationEmail, Request.BaseUrl()));
            }

            return View(model);
        }




        [HttpPost]
        public IActionResult SignIn(IFormCollection collection)
        {


            string userName = collection["userName"];
            string password = collection["password"];


            List<Models.System.ResponseMessage> messages = new List<Models.System.ResponseMessage>();


            var principal = accountService.GetPrincipal(userName, password, cookieAuthenticationScheme);

            if (principal == null)
            {
                messages.Add(new Models.System.ResponseMessage() { Display = "Invalid Email or Password", Debug = "Not able to find matching user or password was not valid" });
                return View(messages);
            }


            var authProperties = new AuthenticationProperties()
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMonths(3),
                IssuedUtc = DateTime.UtcNow,
            };

            // creates the Authentication cookie for user
            HttpContext.SignInAsync(cookieAuthenticationScheme, principal, authProperties);

            // user is valid, so redirect back to home page (for now)
            return Redirect("/");

        }

        [HttpGet]
        public async Task SignOut()
        {
            await HttpContext.SignOutAsync();
            Response.Redirect("/");
        }
    }
}
