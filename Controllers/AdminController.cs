using Cascade.Poll.Models;
using Cascade.Poll.Services;
using Cascade.Poll.ViewModels;
using Orchard;
using Orchard.Core.Common.ViewModels;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.Localization;
using Orchard.Localization.Services;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cascade.Poll.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly IPollService _pollService;
        private readonly ISiteService _siteService;
        private readonly IDateLocalizationServices _dateLocalizationServices;

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public AdminController(
            IDateLocalizationServices dateLocalizationServices,
            IOrchardServices services,
            IPollService pollService,
            ISiteService siteService,
            IShapeFactory shapeFactory)
        {
            Services = services;
            _dateLocalizationServices = dateLocalizationServices;
            _pollService = pollService;
            _siteService = siteService;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public ActionResult Create()
        {
            var pollDetailsViewModel = new PollDetailsViewModel
            {
                StartDate = new DateTimeEditor
                {
                    ShowDate = true,
                    ShowTime = false
                },
                EndDate = new DateTimeEditor
                {
                    ShowDate = true,
                    ShowTime = false
                }
            };
            pollDetailsViewModel.Answers.Add(new PollAnswerViewModel());
            return View(pollDetailsViewModel);
        }
        public ActionResult Delete(int id)
        {
            _pollService.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            PollRecord poll = _pollService.Get(id);
            var pollDetailsViewModel = new PollDetailsViewModel
            {
                Id = poll.Id,
                MaxAnswers = poll.MaxAnswers,
                PollState = poll.PollState,
                Question = poll.Question,
                StartDate = new DateTimeEditor
                {
                    Date = _dateLocalizationServices.ConvertToLocalizedDateString(poll.StartDate),
                    ShowDate = true,
                    ShowTime = false
                },
                EndDate = new DateTimeEditor
                {
                    Date = _dateLocalizationServices.ConvertToLocalizedDateString(poll.EndDate),
                    ShowDate = true,
                    ShowTime = false
                }
            };
            foreach (var answer in poll.Answers)
            {
                var answerModel = new PollAnswerViewModel
                {
                    Id = answer.Id,
                    Answer = answer.Answer,
                    Votes = answer.Votes
                };
                pollDetailsViewModel.Answers.Add(answerModel);
            }
            return View(pollDetailsViewModel);
        }
        [HttpPost]
        public ActionResult Edit(PollDetailsViewModel poll)
        {
            if (ProcessPollPersistence(poll))
            {
                return RedirectToAction("Index");
            }
            poll.StartDate.ShowDate = true;
            poll.StartDate.ShowTime = false;
            poll.EndDate.ShowDate = true;
            poll.EndDate.ShowTime = false;

            return View(poll);
        }

        [HttpPost]
        public ActionResult Create(PollDetailsViewModel poll)
        {
            if (ProcessPollPersistence(poll))
            {
                return RedirectToAction("Index");
            }
            poll.StartDate.ShowDate = true;
            poll.StartDate.ShowTime = false;
            poll.EndDate.ShowDate = true;
            poll.EndDate.ShowTime = false;

            return View(poll);
        }

        private bool ProcessPollPersistence(PollDetailsViewModel poll)
        {
            if (ModelState.IsValid)
            {
                var pollRecord = _pollService.Get(poll.Id) ?? new PollRecord();
                pollRecord.MaxAnswers = poll.MaxAnswers;
                pollRecord.PollState = poll.PollState;
                pollRecord.Question = poll.Question;
                if (String.IsNullOrWhiteSpace(poll.StartDate.Date))
                {
                    pollRecord.StartDate = null;
                }
                else
                {
                    poll.StartDate.Time = "00:00:00";
                    try
                    {
                        pollRecord.StartDate = _dateLocalizationServices.ConvertFromLocalizedString(poll.StartDate.Date + " " + poll.StartDate.Time);
                    }
                    catch (FormatException)
                    {
                        ModelState.AddModelError("InvalidStart", "Invalid start date");
                        return false;
                    }
                }

                if (String.IsNullOrWhiteSpace(poll.EndDate.Date))
                {
                    pollRecord.EndDate = null;
                }
                else
                {
                    poll.EndDate.Time = "23:59:59";
                    try
                    {
                        pollRecord.EndDate = _dateLocalizationServices.ConvertFromLocalizedString(poll.EndDate.Date + " " + poll.EndDate.Time);
                    }
                    catch (FormatException)
                    {
                        ModelState.AddModelError("InvalidStart", "Invalid end date");

                        return false;
                    }
                }

                var idsToDlete = pollRecord.Answers.Select(a => a.Id).Except(poll.Answers.Select(a => a.Id)).ToList();
                foreach (var i in idsToDlete)
                {
                    var answ = pollRecord.Answers.First(a => a.Id == i);
                    pollRecord.Answers.Remove(answ);
                }

                foreach (var answer in poll.Answers)
                {
                    var existentAnswer = pollRecord.Answers.Where(a => a.Id > 0).FirstOrDefault(a => a.Id == answer.Id);
                    if (existentAnswer == null) //add new
                    {
                        var pollAnswer = new PollAnswerRecord
                        {
                            Id = answer.Id,
                            Answer = answer.Answer,
                            Votes = answer.Votes
                        };
                        pollRecord.Answers.Add(pollAnswer);
                    }
                    else
                    { //update
                        existentAnswer.Answer = answer.Answer;
                        existentAnswer.Votes = answer.Votes;
                    }
                }
                _pollService.SaveOrUpdate(pollRecord);
            }
            return ModelState.IsValid;
        }

        public ActionResult GetNewAnswer()
        {
            return PartialView("EditorTemplates/PollAnswerViewModel", new PollAnswerViewModel());
        }

        [HttpPost]
        [FormValueRequired("submit.BulkEdit")]
        public ActionResult Index(FormCollection input)
        {
            var viewModel = new PollsIndexViewModel { Options = new PollIndexOptions() };
            TryUpdateModel(viewModel);
            try
            {
                IEnumerable<PollSummaryViewModel> checkedEntries = viewModel.Polls.Where(c => c.IsChecked);
                switch (viewModel.Options.BulkAction)
                {
                    case PollBulkAction.None:
                        Services.Notifier.Add(NotifyType.Information, T("Did nothing. Item count: " + checkedEntries.Count()));
                        break;
                    case PollBulkAction.Delete:
                        foreach (var pollSummaryViewModel in checkedEntries)
                        {
                            _pollService.Delete(pollSummaryViewModel.Id);
                        }
                        break;
                    case PollBulkAction.ChangeStateToClosed:
                        foreach (var pollSummaryViewModel in checkedEntries)
                        {
                            _pollService.SetPollState(pollSummaryViewModel.Id, PollState.Closed);
                        }
                        break;
                    case PollBulkAction.ChangeStateToOpen:
                        foreach (var pollSummaryViewModel in checkedEntries)
                        {
                            _pollService.SetPollState(pollSummaryViewModel.Id, PollState.Open);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, exception, "Editing poll failed: {0}", exception.Message);
                return RedirectToAction("Index", "Admin", new { options = viewModel.Options });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index(PollIndexOptions options, PagerParameters pagerParameters)
        {
            var model = new PollsIndexViewModel();
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            // Default options
            if (options == null)
                options = new PollIndexOptions();

            var polls = _pollService.GetAllPollsWithoutLogs();

            switch (options.Filter)
            {
                case PollBulkFilter.All:
                    break;
                case PollBulkFilter.Closed:
                    polls = polls.Where(p => p.PollState == PollState.Closed);
                    break;
                case PollBulkFilter.Open:
                    polls = polls.Where(p => p.PollState == PollState.Open);
                    break;
            }

            polls = polls.OrderByDescending(p => p.Id)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);



            foreach (var pollRecord in polls.ToList())
            {
                var pollModel = new PollSummaryViewModel
                {
                    EndDate = pollRecord.EndDate,
                    StartDate = pollRecord.StartDate,
                    Id = pollRecord.Id,
                    MaxAnswers = pollRecord.MaxAnswers,
                    PollState = pollRecord.PollState,
                    Question = pollRecord.Question,
                    AnswersCount = pollRecord.Answers.Count
                };
                model.Polls.Add(pollModel);
            }
            model.Pager = Shape.Pager(pager).TotalItemCount(_pollService.GetAllPollsWithoutLogs().Count());
            model.Options = options;
            return View(model);
        }
    }
}
