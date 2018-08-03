#region IMPORT NAMESPACE
using SampleCode.Repositories.Interfaces;
using SampleCode.Web.Models;
using SampleCode.Web.Repositories.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
#endregion

namespace SampleCode.Controllers
{
    public class TaskController : Controller
    {
        #region VARIABLE DECLARATION
        private readonly Lazy<IImageRepository> imageRepository;
        private readonly Lazy<IOrganizationRepository> organizationRepository;
        private Lazy<ITasksRepository> tasksRepository;
        #endregion

        #region METHODS
        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Initialization
        /// </summary>
        /// <param name="_iImageRepository"></param>
        /// <param name="_iOrganizationRepository"></param>
        /// <param name="_iTasksRepository"></param>
        public TaskController(Lazy<IImageRepository> _iImageRepository, Lazy<IOrganizationRepository> _iOrganizationRepository, Lazy<ITasksRepository> _iTasksRepository)
        {
            imageRepository = _iImageRepository;
            organizationRepository = _iOrganizationRepository;
            tasksRepository = _iTasksRepository;
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Display dashboard
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="LessonId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Dashboard(int? selected, Guid? LessonId)
        {
            if ( (LessonId != null) || (LessonId.HasValue == true))
            {
                LessonModel model = new LessonModel();
                model.LessonId = LessonId.Value;
                model.activeTab = selected.Value;
                TempData["LessonModel"] = model;
            }
            return View(selected.HasValue ? selected.Value : (int)ActiveTab.AssignWork);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Get subject by category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> GetSubjectsByCategory(string category)
        {
            if (category != null)
            {
                var model = await tasksRepository.Value.GetCategotyData();
                var result = model.Where(a => a.Category.Trim().Equals(category.Trim(), StringComparison.InvariantCultureIgnoreCase)).Select(a => new { id = a.Id, name = a.Title });
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Create new task
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> CreateTask()
        {
            var Model = new LearningTask();
            Model.CategoryData = await tasksRepository.Value.GetCategotyData();
            return View("_CreateTask", Model);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Assigned tasks
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult AssignedTasks()
        {
            return View("_AssignedTasks");
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Search task detail
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult SearchTask()
        {
            return View("_SearchTask");
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Load all task.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult LoadTask()
        {
            var model = tasksRepository.Value.GetTasksByCreatedBy(User.Identity.GetUserId());
            return View("_LoadTask", model);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Create new task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateTask(LearningTask task)
        {
            task.CreatedOn = DateTime.Now;
            Guid userId = Guid.Parse(User.Identity.GetUserId());
            task.CreatedByUser = userId;
            await tasksRepository.Value.UpsertTask(task);

            return RedirectToAction("Dashboard");
        }
        #endregion
    }
}