#region IMPORT NAMESPACE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
#endregion

namespace Sample.Web.Models
{
    /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 15th November, 2016
    /// Purpose : Learning task
    /// </summary>
    public class LearningTask
    {
        #region PROPERTY
        public List<CategoryData> CategoryData { get; set; }

        public int Subject_CategoryId { get; set; }

        public string DefaultCategory { get; } = "Other"; //Other

        public string TopicName { get; set; }

        public Guid TopicId { get; set; }

        public Guid Id { get;set;}

        public Guid CreatedByUser { get; set; }

        public string Language { get; set; }

        public DateTime CreatedOn { get; set; }

        public string  Name { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        [AllowHtml]
        public string Instructions { get; set; }

        public List<string> Tags { get; set; }

        public int XPPoints { get; set; }

        public TypeOfExpectedAnswer Type { get; set; }
        #endregion
    }

      /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 15th November, 2016
    /// Purpose : Type of expected anser enum.
    /// </summary>
    public enum TypeOfExpectedAnswer
    {
        [Description("Text")]
        Text,
        [Description("File")]
        File,
        [Description("Codepen")]
        Codepen
    }

     /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 15th November, 2016
    /// Purpose : Category data
    /// </summary>
    public class CategoryData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }
        public string Category { get; set; }
        public string Topics { get; set; }
    }

    /// <summary>
    /// Created By : Maulik Joshi
    /// Created On : 15th November, 2016
    /// Purpose : Learning topic
    /// </summary>
    public class LearningTopic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}