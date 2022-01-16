using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quiz_WebPlatform.ViewModels
{
    public class QuizCategoriesViewModel
    {
        public string QuizName { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem> CategoriesList { get; set; }
    }
}