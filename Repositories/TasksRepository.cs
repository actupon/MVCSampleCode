#region IMPORT NAMESPACE
using Sample.Models;
using Sample.Web.Models;
using Sample.Web.Repositories.Interfaces;
using log4net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Sample.Repositories
{
    public class TasksRepository: ITasksRepository
    {
        #region VARIABLE DECLARATION
        private  string endpointUri;
        private  string primaryKey;
        private DocumentClient client;

        readonly ILog _logger = LogManager.GetLogger(typeof(TasksRepository));
        private string databaseName= ConfigurationManager.AppSettings["docDBDatabaseName"]; 
        private string collectionName = ConfigurationManager.AppSettings["docDBCollectionName"];

        private NotificationRepository notificationRepository = new NotificationRepository();
        #endregion

        #region METHODS
        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 14th November, 2016
        /// Purpose : Initialization
        /// </summary>
        public TasksRepository()
        {
            endpointUri = ConfigurationManager.AppSettings["endpointUri"];
            primaryKey = ConfigurationManager.AppSettings["primaryKey"];
            this.client = new DocumentClient(new Uri(endpointUri), primaryKey);
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 14th November, 2016
        /// Purpose : Get task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public LearningTask GetTask(Guid taskId)
        {
            try
            {
                // Set some common query options
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = 1 };

                // Here we find the Andersen family via its LastName
                IQueryable<LearningTask> collection = this.client.CreateDocumentQuery<LearningTask>(
                        UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                        .Where(f => f.Id == taskId);

                return collection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 14th November, 2016
        /// Purpose : Get category data.
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoryData>> GetCategotyData()
        {
            List<CategoryData> subjects = await (ApplicationDbContext.Create()).Database.SqlQuery<CategoryData>("SELECT * From Subjects").ToListAsync();
            return subjects;

        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 14th November, 2016
        /// Purpose : Update assignment
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        public async Task UpdetAssignment(Assignment assignment)
        {
            if (assignment == null) return;

            try
            {
                if (assignment.Id == Guid.Empty)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), assignment);
                }
                else
                {
                    await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, assignment.Id.ToString()), assignment);
                }
            }
            catch (DocumentClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 14th November, 2016
        /// Purpose : Update task.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task UpdetTask(LearningTask task)
        {
            if (task == null) return;

            try
            {
                if (task.Id == Guid.Empty)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), task);
                }
                else
                {
                    await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, task.Id.ToString()), task);
                }
            }
            catch (DocumentClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 14th November, 2016
        /// Purpose : Get task by created by.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<LearningTask> GetTasksByCreatedBy(string userId)
        {
            try
            {
                Guid UserId = Guid.Empty;
                Guid.TryParse(userId, out UserId);
                // Set some common query options
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = 1000 };

                // Here we find the Andersen family via its LastName
                IQueryable<LearningTask> collection = this.client.CreateDocumentQuery<LearningTask>(
                        UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                        .Where(f => f.CreatedByUser == UserId);

                return collection.ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return new List<LearningTask>();
        }

        /// <summary>
        /// Created By : Maulik Joshi
        /// Created On : 14th November, 2016
        /// Purpose : Get Assignment data.
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        public Assignment GetAssignment(Guid assignmentId)
        {
            try
            {
                // Set some common query options
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = 1 };

                IQueryable<Assignment> collection = this.client.CreateDocumentQuery<Assignment>(
                        UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                        .Where(f => f.Id == assignmentId);


                return collection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }
        #endregion

    }
}