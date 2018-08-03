#region IMPORT NAMESPACE
using SampleCode.ExtensionMethods;
using SampleCode.Web.Helpers;
using SampleCode.Web.Models;
using SampleCode.Web.Repositories.Interfaces;
using log4net;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
#endregion

namespace SampleCode.Web.Controllers
{
    public class LessonController : Controller
    {
        #region VARIABLE DECLARATION
        readonly ILog logger = LogManager.GetLogger(typeof(LessonController));
        private Lazy<ILessonRepository> lessonRepository;
        private Lazy<INotificationManager> notificaitonManager;
        private Lazy<IAdminRepository> adminRepository;
        private Lazy<IUtility> utility;
        #endregion

        #region METHODS

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Initialization
        /// </summary>
        /// <returns></returns>
        /// <param name="iLessonRepository"></param>
        /// <param name="inotificaitonManager"></param>
        /// <param name="iadminRepository"></param>
        /// <param name="iUtility"></param>
        public LessonController(Lazy<ILessonRepository> iLessonRepository, Lazy<INotificationManager> inotificaitonManager, Lazy<IAdminRepository> iadminRepository, Lazy<IUtility> iUtility)
        {
            lessonRepository = iLessonRepository;
            notificaitonManager = inotificaitonManager;
            adminRepository = iadminRepository;
            utility = iUtility;
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : GET: Lesson.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> CreateLesson()
        {
            LessonModel lessonModel = new LessonModel();
            List<CategoryData> categoryDatas = await lessonRepository.Value.GetCategoryData();
            if (TempData["LessonModel"] == null)
            {
                lessonModel.LessonId = Guid.NewGuid();
                lessonModel.isAdded = true;
                lessonModel.activeTab = (int)ActiveTab.CreateLesson;
                lessonModel.CategoryData = categoryDatas;
            }
            else
            {
                var obj = TempData["LessonModel"] as LessonModel;
                lessonModel.isAdded = false;
                lessonModel = lessonRepository.Value.LoadLesson(new Guid(obj.LessonId.ToString()));

                List<CategoryData> catselected = categoryDatas.Where(r => r.Category == lessonModel.Category).Distinct().ToList();
                List<CategoryData> catremainselected = categoryDatas.Where(r => r.Category != lessonModel.Category).Distinct().ToList();
                List<CategoryData> emptyCate = new List<CategoryData>();
                if (catselected != null && catremainselected != null)
                {
                    emptyCate.AddRange(catselected);
                    emptyCate.AddRange(catremainselected);
                }
                lessonModel.CategoryData = emptyCate;
            }

            return View("_CreateLesson", lessonModel);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Create Lesson.
        /// </summary>
        /// <param name="lesson"></param>
        /// <param name="ButtonType"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateDraftLesson(LessonModel lesson, string buttonType)
        {
            lesson.CreatedBy = Guid.Parse(User.Identity.GetUserId());
            lesson.CreatedDate = DateTime.Now;
            lesson = await lessonRepository.Value.LessonCreation(LessonStatus.Draft, lesson);
            lesson.buttonType = CustomMessage.Success;
            return View();
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 01st December, 2016
        /// Purpose : Create Lesson.
        /// </summary>
        /// <param name="lesson"></param>
        /// <param name="ButtonType"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SaveLesson(LessonModel lesson, string buttonType)
        {
            lesson.CreatedBy = Guid.Parse(User.Identity.GetUserId());
            lesson.CreatedDate = DateTime.Now;
            if (User.IsVolunteer())
            {
                lesson = await lessonRepository.Value.LessonCreation(LessonStatus.SubmitedByAuthor, lesson);
            }
            if (User.IsAdmin() && lesson.Status != LessonStatus.Draft.ToDescription())
            {
                lesson = await lessonRepository.Value.LessonCreation(LessonStatus.Published, lesson);
            }
            lesson.buttonType = CustomMessage.Publish;

            string lessonReady = string.Empty;
            if (!String.Equals(lesson.Source.ToString(), LessonSource.SampleCode.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                lessonReady = lesson.Lesson.Keys.FirstOrDefault();
            }
            else 
            { 
                lessonReady = lesson.LessonTitle; 
            }

            string emailBody = string.Empty;

            emailBody = string.Format(utility.Value.RenderPartialViewToString(this, "~/Views/EmailTemplates/_LessonEmail.cshtml", null), lessonReady);
            await notificaitonManager.Value.SendSytemNotification(emailBody);
            return View();
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Retrieve subject by Category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> GetSubjectsByCategory(string category)
        {
            if (category != null)
            {
                var model = await lessonRepository.Value.GetCategoryData();
                var result = model.Where(a => a.Category.Trim().Equals(category.Trim(), StringComparison.InvariantCultureIgnoreCase))
                            .Select(a => new { id = a.Id, name = a.Title });

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Add section.
        /// </summary>
        /// <param name="lessonId"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<JsonResult> AddSection(Guid lessonId, string title, string isAdded)
        {
            LessonModel lesson = new LessonModel();
            lesson = lessonRepository.Value.LoadLesson(lessonId);

            if (lesson == null)
            {
                lesson = new LessonModel { LessonId = lessonId, Status = LessonStatus.InActive.ToDescription(), Sections = new List<Section>() };
            }

            lesson.Sections.Add(new Section { SectionId = Guid.NewGuid(), Title = title, Tasks = null });

            await lessonRepository.Value.UpdateLesson(lesson);
            return Json(new { SectionId = lesson.Sections.Last().SectionId.ToString() });
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 20th November, 2016
        /// Purpose : Delete section.
        /// </summary>
        /// <param name="lessonId"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<JsonResult> DeleteSection(string lessonId, string sectionId)
        {
            var retVal = await lessonRepository.Value.DeleteSection(lessonId, sectionId);
            return Json(new { Message = retVal });
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 29th November, 2016
        /// Purpose : Delete Task.
        /// </summary>
        /// <param name="lessonId"></param>
        /// <param name="sectionId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<JsonResult> DeleteTask(string lessonId, string sectionId, string taskId)
        {
            var retVal = await lessonRepository.Value.DeleteTask(lessonId, sectionId, taskId);
            return Json(new { Message = retVal });
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Get task type partial view as per parameter.
        /// </summary>
        /// <param name="taskTypeId"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> GetTaskTypeView(string lessonId, string sectionId, string taskId)
        {
            LessonTaskModel task = new LessonTaskModel();
            Guid? taskID = String.IsNullOrWhiteSpace(taskId) ? Guid.Empty : new Guid(taskId.ToSafeStringWithTrim());

            if (taskID != Guid.Empty)
            {
                task.LessonId = String.IsNullOrWhiteSpace(lessonId) ? new Guid() : new Guid(lessonId.ToSafeStringWithTrim());
                task.SectionId = String.IsNullOrWhiteSpace(sectionId) ? new Guid() : task.SectionId = new Guid(sectionId.ToSafeStringWithTrim());

                LessonModel lesson = lessonRepository.Value.LoadLesson(task.LessonId);
                Section section = lesson.Sections.FirstOrDefault(a => a.SectionId == task.SectionId);
                if (section != null)
                {
                    task = section.Tasks.FirstOrDefault(t => t.TaskId == taskID);
                }
            }
            else
            {
                task.Type = TypeOfExpectedAnswer.Text.ToString();
            }
            return PartialView("_CreateTask", task);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 19th November, 2016
        /// Purpose : Add/Update test task in lesson.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<JsonResult> CreateTask(LessonTaskModel task)
        {
            LessonModel lesson = lessonRepository.Value.LoadLesson(task.LessonId);
            Boolean isSuccess = false;
            bool isAdd = false;
            try
            {
                if (lesson != null)
                {
                    Section section = lesson.Sections.Where(i => i != null).FirstOrDefault(a => a.SectionId == task.SectionId);
                    if ((task.TaskId == Guid.Empty) && (section != null))
                    {
                        task.TaskId = Guid.NewGuid();
                        isAdd = true;
                    }

                    if (task.TaskType.ToSafeStringWithTrim().ToLower() != TaskType.Assignment.ToSafeStringWithTrim().ToLower())
                    {
                        task.XPPoints = 0;
                        task.Language = null;
                        task.Instructions = null;
                        task.Type = null;
                        task.YouTubeLink = task.YouTubeLink.ToSafeStringWithTrim();
                    }
                    else if (task.TaskType.ToSafeStringWithTrim().ToLower() != TaskType.Content.ToSafeStringWithTrim().ToLower())
                    {
                        task.YouTubeLink = null;
                        task.Instructions = task.Instructions.ToSafeStringWithTrim();
                    }

                    if (isAdd)
                    {
                        if (section.Tasks == null)// Create new in case no task exist.
                        {
                            section.Tasks = new List<LessonTaskModel>();
                        }
                        section.Tasks.Add(task);
                    }
                    else
                    {
                        int updatedIndex = section.Tasks.FindLastIndex(t => t.TaskId == task.TaskId);
                        section.Tasks[updatedIndex] = task;
                    }
                    task.Name = task.Name.ToSafeStringWithTrim();
                    task.Text = task.Text.ToSafeStringWithTrim();

                    await lessonRepository.Value.AddUpdateLesson(lesson);
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return Json(new
                        {
                            success = isSuccess,
                            isAdd = isAdd,
                            SectionId = task.SectionId.ToSafeStringWithTrim(),
                            TaskId = task.TaskId.ToSafeStringWithTrim(),
                            Title = task.Name.ToSafeStringWithTrim()
                        }
                    );
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 05th December, 2016
        /// Purpose : Get all lesson of Volunteer.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MyLesson()
        {
            var model = lessonRepository.Value.GetLessonByCreatedBy(User.Identity.GetUserId());
            return View("_MyLesson", model);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 05th December, 2016
        /// Purpose : Delete lesson.
        /// </summary>
        /// <param name="lessonId"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<JsonResult> DeleteLesson(string lessonId)
        {
            var retVal = string.Empty;
            try
            {
                retVal = await lessonRepository.Value.DeleteLesson(lessonId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Json(new { Message = retVal });
        }
        #endregion
    }
}

