using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Cascade.Poll.Models;

namespace Cascade.Poll.ViewModels
{
    public class PollHolderViewModel
    {
        [Required]
        [Display(Name = "Poll")]
        public int PollId { get; set; }
        public IEnumerable<PollRecord> Polls { get; set; }
    }
}
