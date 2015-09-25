using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Cascade.Poll.Models;

namespace Cascade.Poll.ViewModels
{
    public class PollSummaryViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Question { get; set; }
        public int AnswersCount { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PollState PollState { get; set; }
        public int MaxAnswers { get; set; }

        public bool IsChecked { get; set; }
    }
}
