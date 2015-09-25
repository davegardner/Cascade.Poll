using System.ComponentModel;

namespace Cascade.Poll.Models {

    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum PollState : int 
    {
        Open,
        Closed
    }
}