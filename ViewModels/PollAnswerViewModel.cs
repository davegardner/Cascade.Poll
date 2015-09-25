using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Cascade.Poll.Models;

namespace Cascade.Poll.ViewModels
{
    public class PollAnswerViewModel
    {
        public virtual int Id { get; set; }
        [Required]
        public virtual string Answer { get; set; }
        [Range(0,int.MaxValue)]
        public virtual int Votes { get; set; }
    }
}
