using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quiz_WebPlatform.ViewModels
{
    public class QuizQuestionViewModel
    {
        public int CategoryId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<QuizOption> OptionsList { get; set; }
        public bool IsLast { get; set; }
    }

    public class QuizOption
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; }
    }

    public class UserAnswer
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
    }
}