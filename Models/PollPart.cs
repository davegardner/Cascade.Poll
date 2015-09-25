using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System.ComponentModel.DataAnnotations;

namespace Cascade.Poll.Models
{
    public class PollHolderRecord : ContentPartRecord
    {
        public virtual int PollId { get; set; }
    }
    public class PollPart : ContentPart<PollHolderRecord>
    {
        [Required]
        [Display(Name = "Poll")]
        public int PollId
        {
            get { return Retrieve(x => x.PollId); }
            set { Store(x => x.PollId, value); }
        }
    }
}
