
namespace Cascade.Poll.Models {
    public class PollAnswerRecord
    {
        public virtual int Id { get; set; }
        public virtual PollRecord PollRecord { get; set; }
        public virtual string Answer { get; set; }
        public virtual int Votes { get; set; }

      
    }
}