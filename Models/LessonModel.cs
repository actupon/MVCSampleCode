#region IMPORT NAMESPACE
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
#endregion

namespace Sample.Web.Models
{
    public class LessonModel
    {
        #region PROPERTY
        [JsonProperty("id")]
        public Guid LessonId { get; set; }
        public string LessonTitle { get; set; }
        public string Language { get; set; }
        [AllowHtml]
        public string Overview { get; set; }
        public string Status { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        [JsonIgnore]
        public List<CategoryData> CategoryData { get; set; }
        public IList<Section> Sections { get; set; }
        public LessonSource Source { get; set; }
        public string SourceUrl { get; set; }
        public IDictionary<string, string> Lesson { get; set; }
        //Used button Type
        [JsonIgnore]
        public CustomMessage buttonType { get; set; }

        //Verify lesson created or edited.
        [JsonIgnore]
        public bool isAdded { get; set; }

        //pass activeTab property value to LessonController
        [JsonIgnore]
        public int activeTab { get; set; }
        #endregion
    }

    /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 19/11/2016
    /// Purpose : Section class details.
    /// </summary>
    public class Section
    {
        public Guid SectionId { get; set; }
        public string Title { get; set; }
        public List<LessonTaskModel> Tasks { get; set; }
    }

    /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 19/11/2016
    /// Purpose : Lesson Status enum
    /// </summary>
    public enum LessonStatus
    {
        InActive,
        Draft,
        SubmitedByAuthor,
        Published
    }

    /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 19/11/2016
    /// Purpose : Lesson Source enum
    /// </summary>
    public enum LessonSource
    {
        Sample,
        Youtube,
        Elearning
    }

    /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 19/11/2016
    /// Purpose : Task type enum
    /// </summary>
    public enum TaskType
    {
        [Description("Create Assignment")]
        Assignment,
        [Description("Create Content")]
        Content
    }

    /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 20/11/2016
    /// Purpose : Custom Message enum
    /// </summary>
    public enum CustomMessage
    {
        [Description("Success")]
        Success,
        [Description("Error")]
        Error,
        [Description("Not Exists")]
        NotExists,
        [Description("Publish")]
        Publish,
        [Description("Save")]
        Save
    }

    /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 02/12/2016
    /// Purpose : Custom Active tab
    /// </summary>
    public enum ActiveTab
    {
        ActiveAssignments = 0,
        AssignWork = 1,
        CreateLesson = 2,
        MyLesson = 3
    }

    /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 02/12/2016
    /// Purpose : HttpStatusCode
    /// </summary>
    public enum StatusCode
    {
        Ok = 200,
        Error = 404
    }
}