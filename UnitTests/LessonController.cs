#region IMPORT NAMESPACE
using Sample.Web.Controllers;
using Sample.Web.Models;
using Sample.Web.Repositories.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
#endregion

namespace Sample.UnitTests.ContollerTests
{
    [TestClass]
    public class LessonControllerTests
    {
        #region VARIABLE DECLARATION
        private readonly Mock<Lazy<ILessonRepository>> lessonRepositoryMock;
        private readonly Mock<ILessonRepository> lessonRepository;
        private readonly Mock<Lazy<INotificationManager>> notificaitonManagerMock;
        private readonly Mock<Lazy<IAdminRepository>> adminRepositoryMock;
        private readonly Mock<Lazy<IUtility>> utilityMock;
        #endregion

        #region METHODS
        public LessonControllerTests()
        {
            lessonRepositoryMock = new Mock<Lazy<ILessonRepository>>();
            lessonRepository = new Mock<ILessonRepository>();
            notificaitonManagerMock = new Mock<Lazy<INotificationManager>>();
            adminRepositoryMock = new Mock<Lazy<IAdminRepository>>();
            utilityMock = new Mock<Lazy<IUtility>>();
        }

        [TestMethod]
        public void LessonController_CreateLesson()
        {
            //Arrange 
            LessonModel expectedLessonModel = new LessonModel();
            expectedLessonModel.Status = LessonStatus.Draft.ToString();
            expectedLessonModel.CreatedDate = DateTime.Now;
            expectedLessonModel.CreatedBy = new Guid();
            expectedLessonModel.Source = LessonSource.Elearning;
            expectedLessonModel.Lesson = new Dictionary<string, string>();
            expectedLessonModel.Lesson.Add(new KeyValuePair<string, string>(LessonSource.Elearning.ToString(), "http://www.youtube.com"));

            Dictionary<string, string> actualLesson = new Dictionary<string, string>();
            actualLesson.Add(LessonSource.Elearning.ToString(), "http://www.youtube.com");

            var actualModel = Task<LessonModel>.Factory.StartNew(() =>
            {
                return new LessonModel
                {
                    Source = LessonSource.Elearning,
                    Lesson = actualLesson
                };
            });

            //Act
            lessonRepository.Setup(x => x.UpsertLesson(It.IsAny<LessonModel>(), true)).Returns(actualModel);

            LessonController lessonController = new LessonController(new Lazy<ILessonRepository>(() => lessonRepository.Object), new Lazy<INotificationManager>(),new Lazy<IAdminRepository>(),new Lazy<IUtility>());
            Task<ActionResult> result = lessonController.CreateLesson() as Task<ActionResult>;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Exception);
            Assert.AreEqual(result.Status.ToString(), "RanToCompletion");
            Assert.AreEqual(expectedLessonModel.Lesson.Keys.ToString(), actualModel.Result.Lesson.Keys.ToString());

        }

        [TestMethod]
        public void LessonController_GetLesson()
        {
            //Arrange
            var expectedCategoryDatas = Task<List<CategoryData>>.Factory.StartNew(() =>
            {
                return new List<CategoryData>
                {
                    new CategoryData {
                    Category = "Language",
                    Desciption = "desciption",
                    Topics = "Topics"
                    }
                };
            });

            //Act
            lessonRepository.Setup(x => x.GetCategoryData()).Returns(expectedCategoryDatas);
            LessonController lessonController = new LessonController(new Lazy<ILessonRepository>(() => lessonRepository.Object), new Lazy<INotificationManager>(), new Lazy<IAdminRepository>(),new Lazy<IUtility>());
            var actionResult = lessonController.CreateLesson();// as Task<ActionResult>;
            var result = actionResult as Task<ActionResult>;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("_CreateLesson", (result.Result as ViewResult).ViewName);

        }
        #endregion

    }
}
