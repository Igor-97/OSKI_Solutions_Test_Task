using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quiz_WebPlatform.ViewModels
{
    public class QuizCategoryDescriptionViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryDescription { get; set; }
        public bool Checkbox { get; set; }
    }
}