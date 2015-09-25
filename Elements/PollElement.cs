using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Poll.Elements
{
    class PollElement: Element
    {
        public override string Category
        {
            get{ return "PollElement";}
            
        }

        [Display(Name = "Poll")]
        public int PollId
        {
            get { return this.Retrieve(x => x.PollId); }
            set { this.Store(x => x.PollId, value); }
        }
    }
}
