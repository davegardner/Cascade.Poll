using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Data.Conventions;

namespace Cascade.Poll.Models {
    public class PollRecord
    {
        public PollRecord() {
            Answers = new List<PollAnswerRecord>();
            Log = new List<PollLogRecord>();
        }
        public virtual int Id { get; set; }
        public virtual string Question { get; set; }
        [Aggregate]
        public virtual IList<PollAnswerRecord> Answers { get; set; }
        public virtual IList<PollLogRecord> Log { get; set; }

        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual PollState PollState { get; set; }
        public virtual int MaxAnswers { get; set; }

    }
}