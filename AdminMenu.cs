using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Core.Contents;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Cascade.Poll
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("polls")
                .Add(T("Polls"), "6",
                    menu => menu.Add(T("List"), "0", item => item.Action("Index", "Admin", new { area = "Cascade.Poll" })
                        ));
        }
    }
}
