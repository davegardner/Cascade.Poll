using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Cascade.Poll.Models;

namespace Cascade.Poll.ViewModels
{
    public class PollsIndexViewModel
    {
        public PollsIndexViewModel() {
            Polls = new List<PollSummaryViewModel>();
        }
        public PollIndexOptions Options { get; set; }
        public IList<PollSummaryViewModel> Polls { get; set; }
        public dynamic Pager { get; set; }
    }
}
