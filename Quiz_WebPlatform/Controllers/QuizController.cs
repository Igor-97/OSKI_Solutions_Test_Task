using Quiz_WebPlatform.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Quiz_WebPlatform.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private QuizWebPlatformEntities _dbContext;

        public QuizController()
        {
            _dbContext = new QuizWebPlatformEntities();
        }

        public ActionResult Index()
        {
            if (Session["QuizId"] == null)
            {
                FormsAuthentication.SignOut();

                return RedirectToAction("Index", "Account");
            }

            var viewModel = new QuizCategoriesViewModel();

            int quizId = int.Parse(Session["QuizId"].ToString());

            var quiz = _dbContext.Quizzes
                .SingleOrDefault(item => item.QuizId == quizId);

            if (quiz == null)
            {
                FormsAuthentication.SignOut();

                return RedirectToAction("Index", "Account");
            }

            viewModel.QuizName = quiz.QuizName;

            viewModel.CategoriesList = _dbContext.Categories
                .Where(item => item.QuizId == quizId)
                .Select(item => new SelectListItem()
                {
                    Value = item.CategoryId.ToString(),
                    Text = item.CategoryName
                })
                .ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(string CategoryId)
        {
            if (CategoryId == null)
                return RedirectToAction("Error");

            var viewModel = new QuizCategoryDescriptionViewModel();

            viewModel.CategoryId = int.Parse(CategoryId);

            Category category = _dbContext.Categories
                .SingleOrDefault(item => item.CategoryId == viewModel.CategoryId);

            viewModel.CategoryDescription = category.CategoryDescription;

            return View("Terms", viewModel);
        }

        [HttpPost]
        public ActionResult Terms(QuizCategoryDescriptionViewModel viewModel)
        {
            if (viewModel == null)
                return RedirectToAction("Error");

            if (viewModel.Checkbox != true)
            {
                ModelState.AddModelError(string.Empty, "Terms must be accepted!");
                return View(viewModel);
            }

            var routeValues = new RouteValueDictionary();
            routeValues.Add("CategoryId", viewModel.CategoryId);

            return RedirectToAction("Question", routeValues);
        }

        public ActionResult Question(int? CategoryId)
        {
            if (CategoryId == null)
                return RedirectToAction("Error");

            return View(GetQuestionViewModel((int)CategoryId, 0));
        }

        [HttpPost]
        public ActionResult Question(string CategoryId, string QuestionId, 
            string OptionText, string IsLast)
        {
            if (CategoryId == null || QuestionId == null)
                return RedirectToAction("Error");

            if (OptionText == null)
            {
                ModelState.AddModelError(string.Empty, "Answer must be selected!");

                return View(GetQuestionViewModel(int.Parse(CategoryId), int.Parse(QuestionId), true));
            }

            if (Session["UserAnswers"] == null)
            {
                var userAnswers = new List<UserAnswer>();
                userAnswers.Add(new UserAnswer()
                {
                    QuestionId = int.Parse(QuestionId),
                    AnswerText = OptionText
                });

                Session["UserAnswers"] = userAnswers;
            }
            else
            {
                var userAnswers = Session["UserAnswers"] as List<UserAnswer>;
                userAnswers.Add(new UserAnswer()
                {
                    QuestionId = int.Parse(QuestionId),
                    AnswerText = OptionText
                });

                Session["UserAnswers"] = userAnswers;
            }

            if (IsLast != null)
            {
                var routeValues = new RouteValueDictionary();
                routeValues.Add("CategoryId", int.Parse(CategoryId));

                return RedirectToAction("Result", routeValues);
            }

            return View(GetQuestionViewModel(int.Parse(CategoryId), int.Parse(QuestionId)));
        }

        public ActionResult Result(int? CategoryId)
        {
            if (CategoryId == null)
            {
                if (Session["UserAnswers"] != null)
                    Session["UserAnswers"] = null;

                return RedirectToAction("Error");
            }
                
            if (Session["UserAnswers"] == null)
                return RedirectToAction("Error");

            var userAnswers = Session["UserAnswers"] as List<UserAnswer>;

            var categoryAnswers = _dbContext.Answers
                .Where(item => item.CategoryId == CategoryId)
                .ToDictionary(item => item.QuestionId, item => item.AnswerText);

            if (categoryAnswers.Count != userAnswers.Count)
            {
                ModelState.AddModelError(string.Empty, "An issue occured during the quiz, please, try again!");

                Session["UserAnswers"] = null;

                return View(new QuizResultModelView()
                {
                    RightAnswers = 0,
                    Score = 0,
                    TotalQuestions = 0
                });
            }

            var viewModel = new QuizResultModelView();
            viewModel.TotalQuestions = userAnswers.Count;
            viewModel.RightAnswers = 0;

            foreach (var userAnswer in userAnswers)
            {
                if (userAnswer.AnswerText == categoryAnswers[userAnswer.QuestionId])
                    viewModel.RightAnswers++;
            }

            viewModel.Score = viewModel.RightAnswers * 10;

            var result = new Result();
            result.CategoryId = (int)CategoryId;
            result.ResultScore = viewModel.Score;
            result.UserId = int.Parse(Session["UserId"].ToString());
            result.DateTime = DateTime.Now;

            Session["UserAnswers"] = null;

            var user = _dbContext.Users
                .SingleOrDefault(item => item.UserId == result.UserId);

            if (user == null)
                return RedirectToAction("Error");

            var lastResult = _dbContext.Results
                .Where(item => item.UserId == result.UserId && item.CategoryId == result.CategoryId)
                .OrderByDescending(item => item.DateTime)
                .FirstOrDefault();

            if (lastResult != null)
                viewModel.PreviousResult = new UserResult() { Score = lastResult.ResultScore, Date = lastResult.DateTime };

            _dbContext.Results.Add(result);
            _dbContext.SaveChanges();

            return View(viewModel);
        }

        private QuizQuestionViewModel GetQuestionViewModel(int categoryId, int questionId,
            bool reCreate = false)
        {
            var viewModel = new QuizQuestionViewModel();

            viewModel.CategoryId = categoryId;

            Question question;

            if (reCreate == true)
            {
                question = _dbContext.Questions
                    .FirstOrDefault(item => item.CategoryId == categoryId && item.QuestionId == questionId);
            }
            else
            {
                question = _dbContext.Questions
                    .FirstOrDefault(item => item.CategoryId == categoryId && item.QuestionId > questionId);
            }

            viewModel.QuestionId = question.QuestionId;
            viewModel.QuestionText = question.QuestionText;

            var lastQuestion = _dbContext.Questions
                .FirstOrDefault(item => item.CategoryId == categoryId && item.QuestionId > question.QuestionId);

            if (lastQuestion == null)
                viewModel.IsLast = true;

            if (question != null)
            {
                var optionsList = _dbContext.Options
                    .Where(item => item.QuestionId == question.QuestionId)
                    .ToList();

                if (optionsList != null)
                {
                    viewModel.OptionsList = new List<QuizOption>();

                    foreach (var item in optionsList)
                    {
                        viewModel.OptionsList.Add(new QuizOption()
                        {
                            OptionId = item.OptionId,
                            OptionText = item.OptionText
                        });
                    }
                }
            }

            return viewModel;
        }
    }
}