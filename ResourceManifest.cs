using Orchard.UI.Resources;

namespace Cascade.Poll {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("PollAdmin").SetUrl("/Modules/Orchard.jQuery/Styles/jquery-ui.css");
            manifest.DefineStyle("Poll").SetUrl("poll.css");
            manifest.DefineScript("ba-poll").SetUrl("ba-poll.js").SetDependencies("jQuery");
        }
    }
}
