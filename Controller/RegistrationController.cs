#region IMPORT NAMESPACE
using Censored;
using SampleCode.Helpers.Manager;
using SampleCode.Models;
using SampleCode.Repositories;
using SampleCode.Web.Repositories.Interfaces;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
#endregion

namespace SampleCode.Web.Controllers
{
    public class RegistrationController : Controller
    {
         #region VARIABLE DECLARATION
        private ApplicationUserManager userManager;
        private readonly ILog _logger = LogManager.GetLogger(typeof(RegistrationController));
        private readonly INotificationManager notificationManager;
        private readonly IStudentRepository studentRepository;
        #endregion

         #region METHODS
        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 17th November, 2016
        /// Purpose : Initialization
        /// </summary>
        /// <param name="_iNotificationManager"></param>
        /// <param name="_studentRepository"></param>
        public RegistrationController(INotificationManager _iNotificationManager, IStudentRepository _studentRepository)
        {
            notificationManager = _iNotificationManager;
            studentRepository = _studentRepository;
        }

         /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 17th November, 2016
        /// Purpose : Initialization
        /// </summary>
        /// <param name="_iNotificationManager"></param>
        /// <param name="user"></param>
        public RegistrationController(INotificationManager _iNotificationManager, ApplicationUserManager user)
        {
            notificationManager = _iNotificationManager;
            userManager = user;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 17th November, 2016
        /// Purpose : Get student details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Student()
        {
            Students model = new Students();

            var ci = ResolveCulture();
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            model.OptionalBirthDay = null;
            model.Country = getCountry(ci);

            return View("~/Views/Landings/Student.cshtml", model);
        }

          /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 17th November, 2016
        /// Purpose : Save student details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Student(Students model)
        {
            IdentityResult result = null;

            if (model != null && model.Termsofuse && !string.IsNullOrWhiteSpace(model.FirstName) && !string.IsNullOrWhiteSpace(model.LastName) && !string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Bio)) 
            {
                string password = Membership.GeneratePassword(12, 1);
                string userName = getUserName(model.Email);

                var user = new ApplicationUser() { UserName = userName, Email = model.Email };
                result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    try
                    {
                        CultureInfo ci = ResolveCulture();
                        model.Language = ci.DisplayName;
                        model.Country = getCountry(ci);

                        model.Birthday = model.OptionalBirthDay.HasValue ? model.OptionalBirthDay.Value : DateTime.MinValue;

                        var auth = HttpContext.GetOwinContext().Authentication;
                        auth.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                        auth.SignIn(new AuthenticationProperties() { IsPersistent = true }, await user.GenerateUserIdentityAsync(UserManager));
                        var userCreated = await UserManager.FindByEmailAsync(model.Email);
                        userCreated.Roles.Add(new IdentityUserRole { RoleId = 4.ToString() });
                        await UserManager.UpdateAsync(userCreated);
                        model.Street = model.BusinessName = model.City = model.Phone = model.CellProvider = model.HomeOrganizationID = model.Zip = "-";
                        model.IsProfilePublic = false;

                        await studentRepository.UpsetProspectData(userCreated.Id, model);

                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        await notificationManager.SendUserRegisteredNotification(user.Email, callbackUrl, password, userName);

                        string registrationMessage = string.Format(Resources.Resources.RegistrationMessage, userName+"--"+model.Email, "StudentNewForm");
                        await notificationManager.SendSytemNotification(registrationMessage, Resources.Resources.NewUser, this.Request.IsLocal);
                    }
                    catch (Exception ex)
                    {
                        string registrationMessage = " === " + DateTime.Now.ToLongDateString() + " === " + Environment.NewLine + ex.Message + userName + model.Email+ "----->" +ex.Message+ "=== " + Environment.NewLine;
                        _logger.Error(registrationMessage);
                    }

                    return RedirectToAction("SetupProperty", "Home", new { guided = true });
                }                
            }

            _logger.Error(model == null ? "Model is null" : $"{model.Termsofuse.ToString()} - {model.FirstName} - {model.LastName} - {model.Email} - UserManager Errors:"+ getErrorText(result?.Errors));

            model.Error = true;
            return View("~/Views/Landings/Student.cshtml", model);
        }

          /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 17th November, 2016
        /// Purpose : Get error text.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        private string getErrorText(IEnumerable<string> errors)
        {
            string all = Environment.NewLine;
            if (errors != null)
            {
                foreach (var e in errors)
                {
                    all += e + Environment.NewLine;
                }
            }

            return all;
        }

          /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 17th November, 2016
        /// Purpose : Get username
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private string getUserName(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    var tokens = email.Split('@');
                    return tokens[0];
                }
                catch
                {
                    return email;
                }
            }

            return email;
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 17th November, 2016
        /// Purpose : Get country.
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        private string getCountry(CultureInfo ci)
        {
            RegionInfo ri = null;
            try
            {
                if (ci != null)
                {
                    ri = new RegionInfo(ci.LCID);
                    return ri.DisplayName;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "United States";
        }

         /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 17th November, 2016
        /// Purpose : Resolve culture.
        /// </summary>
        /// <returns></returns>
        private CultureInfo ResolveCulture()
        {
            string[] languages = HttpContext.Request.UserLanguages;
            if ( (languages == null) || (languages.Length == 0))
                return Thread.CurrentThread.CurrentUICulture; 
            try
            {
                string language = languages[0].ToLowerInvariant().Trim();
                return CultureInfo.CreateSpecificCulture(language);
            }
            catch
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
        }
        #endregion
    }
}