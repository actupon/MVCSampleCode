#region IMPORT NAMESPACE
using SampleCode.Helpers.Manager;
using SampleCode.Repositories;
using SampleCode.Repositories.Interfaces;
using SampleCode.Web.Repositories.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
#endregion

namespace SampleCode.Controllers
{
    public class StudentController : Controller
    {
        #region VARIABLE DECLARATION
        private readonly Lazy<IAdminRepository> adminRepository;
        private readonly Lazy<IVolunteerRepository> setupRepository;
        private readonly Lazy<IStudentRepository> studentRepository;
        private readonly Lazy<IImageRepository> imageRepository;
        private readonly Lazy<INotificationManager> notificationManager;
        #endregion

        #region METHODS
        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 18th November, 2016
        /// Purpose : Initialization
        /// </summary>
        /// <param name="_iAdminRepository"></param>
        /// <param name="_iSetupRepository"></param>
        /// <param name="_iStudentRepository"></param>
        /// <param name="_iImageRepository"></param>
        /// <param name="_iNotificationManager"></param>
        public StudentController(
            Lazy<IAdminRepository> _iAdminRepository,
            Lazy<IVolunteerRepository> _iSetupRepository,
            Lazy<IStudentRepository> _iStudentRepository,
            Lazy<IImageRepository> _iImageRepository,
            Lazy<INotificationManager> _iNotificationManager
            )
        {
            adminRepository = _iAdminRepository;
            setupRepository = _iSetupRepository;
            imageRepository = _iImageRepository;
            studentRepository = _iStudentRepository;
            notificationManager = _iNotificationManager;
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 18th November, 2016
        /// Purpose : Get student profile details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> StudentProfile(string id)
        {
            var userId = User.Identity.GetUserId();
            var student = await studentRepository.Value.LoadProspect(id);
            if (student.IsProfilePublic == false)
            {
                return HttpNotFound("No results found.");
            }
            student.Avatar = await imageRepository.Value.getFilePathForUser(id, "/images/noavatar.png");
            return View(student);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 18th November, 2016
        /// Purpose : Get student profile card
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> StudentProfileCard(string id)
        {
            var userId = User.Identity.GetUserId();
            var student = await studentRepository.Value.LoadProspect(id);
            return View(student);
        }
        #endregion
    }
}