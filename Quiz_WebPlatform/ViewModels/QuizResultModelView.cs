using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quiz_WebPlatform.ViewModels
{
    public class QuizResultModelView
    {
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int RightAnswers { get; set; }
        public UserResult PreviousResult { get; set; }
    }

    public class UserResult
    {
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }
}