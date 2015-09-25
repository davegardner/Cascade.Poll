using Cascade.Poll.Models;
using Cascade.Poll.Services;
using Cascade.Poll.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using System.Linq;

namespace Cascade.Poll.Drivers
{
    public class PollDriver : ContentPartDriver<PollPart>
    {
        private readonly IPollService _pollService;
        public Localizer T { get; set; }

        public PollDriver(IPollService pollService)
        {
            T = NullLocalizer.Instance;
            _pollService = pollService;
        }

        protected override string Prefix
        {
            get
            {
                return "PollWidget";
            }
        }

        protected override DriverResult Display(
            PollPart part, string displayType, dynamic shapeHelper)
        {
            PollRecord selectedPoll = _pollService.Get(part.PollId);
            bool canVote = _pollService.CanVote(selectedPoll);
            //return ContentShape("Parts_Poll", () => shapeHelper.Parts_Poll(
            //    PollId: part.PollId, Poll: selectedPoll));

            return Combined(
                ContentShape("Parts_Poll", () => shapeHelper.Parts_Poll(
                    PollId: part.PollId, Poll: selectedPoll, CanVote: canVote)),
                ContentShape("Parts_Poll_Summary", () => shapeHelper.Parts_Poll_Summary(
                    PollId: part.PollId, Poll: selectedPoll))
                );

        }

        //GET
        protected override DriverResult Editor(PollPart part, dynamic shapeHelper)
        {
            //XXX: dropdown list amount should be manageable from somewhere
            var polls = _pollService.GetAllPollsWithoutLogs()
                .OrderByDescending(p => p.Id).Take(10).ToList();

            //XXX: Change option to enum
            polls.Insert(0, new PollRecord { Id = -2, Question = T("Lattest").ToString() });
            polls.Insert(1, new PollRecord { Id = -1, Question = T("Random").ToString() });
            polls.Insert(2, new PollRecord { Id = -0, Question = T("None").ToString() });

            var model = new PollHolderViewModel();
            model.PollId = part.PollId;
            model.Polls = polls;

            return ContentShape("Parts_Poll_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: "Parts/Poll",
                                    Model: model,
                                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(
            PollPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var result = updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
