using Cascade.Poll.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.Poll.Services 
{
    public class PollService : IPollService 
    {
        public const string PollCookieName = "pollData";
        public const string PollPrefix = "poll_";

        private readonly IRepository<PollLogRecord> _logRepo;
        private readonly IRepository<PollRecord> _pollRepo;
        private readonly IRepository<PollAnswerRecord> _answersRepo;
        public IOrchardServices Services { get; set; }
        private readonly  IContentManager _cms;

        Random random = new Random();

        public PollService(
            IRepository<PollLogRecord> logRepo,
            IRepository<PollRecord> pollRepo,
            IRepository<PollAnswerRecord> answersRepo,
            IOrchardServices orchardServices,
            IContentManager cms
            ) {
            _logRepo = logRepo;
            _pollRepo = pollRepo;
            _answersRepo = answersRepo;
            Services = orchardServices;
            _cms = cms;
        }

        public bool Delete(int id) {
            var poll = Get(id);
            if (poll == null) return false;
            _pollRepo.Delete(poll);
            return true;
        }

        public void SetPollState(int pollId, PollState state) {
            var poll = Get(pollId);
            poll.PollState = state;
            _pollRepo.Update(poll);
        }

        public IEnumerable<PollRecord> GetAllPollsWithoutLogs() {
            //XXX: find a BETTER way to eager load Answers!
            // DAG: The Orchard way to do this is to use IContentManage.Query.Join and to do that the
            // records need to derive from ContentRecord, and PollRecord may also need to have an associated Part.

            var polls = _pollRepo.Table.ToList();
            foreach (var poll in polls)
                poll.Answers = _answersRepo.Fetch(a => a.PollRecord.Id == poll.Id).ToList();
            return polls;
        }

        public bool CanVote(PollRecord selectedPoll) {
            if(selectedPoll==null) return false;

            if(selectedPoll.PollState == PollState.Closed) return false;
            if (selectedPoll.StartDate.HasValue && selectedPoll.StartDate > DateTime.Now) return false;
            if (selectedPoll.EndDate.HasValue && selectedPoll.EndDate < DateTime.Now) return false;

            var httpContext = Services.WorkContext.HttpContext;
            if (httpContext == null || httpContext.Request == null || httpContext.Request.Cookies == null) return false;

            var pollCookie = httpContext.Request.Cookies[PollCookieName];
            if (pollCookie != null) {
                string cookieValueName = PollPrefix + selectedPoll.Id;
                if (pollCookie.Values[cookieValueName] != null) return false;
            }

            return true;
        }
        public bool TryVote(PollRecord record, IEnumerable<int> answers)
        {
            if(record==null) return false;

            var httpContext = Services.WorkContext.HttpContext;
            if (httpContext == null || httpContext.Request == null || httpContext.Request.Cookies == null) return false;

            var pollCookie = httpContext.Request.Cookies[PollCookieName];
            if (pollCookie==null) pollCookie = new HttpCookie(PollCookieName);

            string cookieValueName = PollPrefix + record.Id;
            if(pollCookie.Values[cookieValueName]!=null) return false;
            pollCookie.Values.Add(cookieValueName, "1");
            pollCookie.Expires = DateTime.Now.AddYears(1);
            Services.WorkContext.HttpContext.Response.Cookies.Add(pollCookie);

            if (record.MaxAnswers > 0 && answers.Count() > record.MaxAnswers) return false;
            foreach (var pollAnswerRecord in record.Answers.Where(a=>answers.Contains(a.Id))) {
                pollAnswerRecord.Votes++;
                _answersRepo.Update(pollAnswerRecord);
            }
            PollLogRecord logRecord = new PollLogRecord {
                PollRecord = record,
                UserDetail = ""+httpContext.Request.UserHostAddress,
                VoteDate = DateTime.Now
            };
            _logRepo.Create(logRecord);

            return true;
        }

        public IEnumerable<PollRecord> GetOpenPollsWithoutLogs() {
            return GetAllPollsWithoutLogs()
                .Where(p => p.Answers.Count > 0)
                .Where(p => p.PollState != PollState.Closed)
                .Where(p =>
                    (p.StartDate == null && p.EndDate == null)
                    || (p.StartDate != null && p.StartDate <= DateTime.Now)
                    || (p.EndDate != null && p.EndDate >= DateTime.Now))
                    ;
        }

        public void SaveOrUpdate(PollRecord pollRecord) 
        {
            foreach (var pollAnswerRecord in pollRecord.Answers)
            {
                if (pollAnswerRecord.Id <= 0) {
                    pollAnswerRecord.PollRecord = pollRecord;
                    _answersRepo.Create(pollAnswerRecord);
                }
            } 
            if (pollRecord.Id > 0)
            {
                _pollRepo.Update(pollRecord);
            }
            else 
            {
                _pollRepo.Create(pollRecord);
            }
        }
        public PollRecord Get(int id) {
            if (id > 0) {
                return _pollRepo.Get(id);
            }
            //XXX: Change option to enum
            if (id == -1)//Random
            {
                var count = GetOpenPollsWithoutLogs().Count();
                var rnd = random.Next(0, Math.Max(0, count));
                return GetOpenPollsWithoutLogs().Skip(rnd).FirstOrDefault();
            }
            if (id == -2)//Lattest
            {
                return GetOpenPollsWithoutLogs().OrderByDescending(p=>p.Id).FirstOrDefault();
            }
            return null;
        }
    }
}