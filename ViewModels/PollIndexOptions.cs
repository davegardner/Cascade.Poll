using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cascade.Poll.ViewModels
{
    public class PollIndexOptions 
    {
        public PollBulkFilter Filter { get; set; }
        public PollBulkAction BulkAction { get; set; }
    }


    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum PollBulkFilter
    {
        All,
        Closed,
        Open
    }
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum PollBulkAction
    {
        None,
        Delete,
        ChangeStateToClosed,
        ChangeStateToOpen
    }
}
