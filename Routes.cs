using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Cascade.Poll
{
    public class Routes : IRouteProvider
    {
        #region Implementation of IRouteProvider

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Priority = 6,
                    Route = new Route(
                        "Poll/{action}",
                        new RouteValueDictionary {
                            {"area", "Cascade.Poll"},
                            {"controller", "Poll"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Poll"}
                        },
                        new MvcRouteHandler())
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        #endregion
    }
}
