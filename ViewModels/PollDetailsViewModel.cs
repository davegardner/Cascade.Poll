using Cascade.Poll.Models;
using Orchard.Core.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Cascade.Poll.ViewModels
{
    public class PollDetailsViewModel
    {
        public PollDetailsViewModel() {
            Answers = new List<PollAnswerViewModel>();
        }

        public int Id { get; set; }
        [Required]
        public string Question { get; set; }
        public IList<PollAnswerViewModel> Answers { get; set; }

        public DateTimeEditor StartDate { get; set; }
        public DateTimeEditor EndDate { get; set; }
        public PollState PollState { get; set; }
        public int MaxAnswers { get; set; }

        public IEnumerable<SelectListItem> PollStates {
            get {
                return Enum.GetNames(typeof (PollState)).Select(
                    e => new SelectListItem {Text = e, Value = e});
            }
        }
    }
}
