using Cascade.Poll.Services;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Cascade.Poll.Controllers
{
    public class PollController : Controller
    {
        private readonly IPollService _pollService;
        public IOrchardServices Services { get; set; }
        public ILogger Logger { get; set; }
        public Localizer T { get; set; }
        dynamic Shape { get; set; }

        public PollController(
            IOrchardServices services, 
            IShapeFactory shapeFactory,
            IPollService pollService) {
            _pollService = pollService;
            Services = services;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        public ActionResult Index() {
            return new ContentResult{Content = ""};
        }

        [HttpPost]
        public ActionResult Vote(IEnumerable<int> answers, int pollId) {
            var poll = _pollService.Get(pollId);
            if (poll == null || answers==null) return HttpNotFound("Poll not found");
            if (_pollService.CanVote(poll)) {
                _pollService.TryVote(poll, answers);
            }
            return new ShapeResult(this, Shape.Parts_Poll(new { PollId = poll.Id, Poll = poll, CanVote = false }));
        }
    }
}
