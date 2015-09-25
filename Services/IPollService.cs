using Cascade.Poll.Models;
using Orchard;
using System.Collections.Generic;

namespace Cascade.Poll.Services
{
    public interface IPollService : IDependency
    {
        bool TryVote(PollRecord record, IEnumerable<int> answers);
        void SaveOrUpdate(PollRecord pollRecord);
        PollRecord Get(int id);
        bool Delete(int id);

        void SetPollState(int pollId, PollState state);

        IEnumerable<PollRecord> GetAllPollsWithoutLogs();
        bool CanVote(PollRecord selectedPoll);
    }
}
