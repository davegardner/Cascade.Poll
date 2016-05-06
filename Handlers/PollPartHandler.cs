using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cascade.Poll.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Cascade.Poll.Handlers
{
    public class PollPartHandler : ContentHandler 
    {
        public PollPartHandler(IRepository<PollHolderRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
