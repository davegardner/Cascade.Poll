using System;

namespace Cascade.Poll.Models {
    public class PollLogRecord
    {
        public virtual int Id { get; set; }
        public virtual PollRecord PollRecord { get; set; }
        public virtual DateTime VoteDate { get; set; }
        public virtual string UserDetail { get; set; }
    }
}