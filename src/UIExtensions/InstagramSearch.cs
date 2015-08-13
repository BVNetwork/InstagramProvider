using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;
using Hackathon.Business.InstagramProvider;

namespace Hackathon.Business.UIExtensions
{
    [Component]
    public class InstagramSearch : ComponentDefinitionBase
    {
        public InstagramSearch()
            : base("alloy.components.CustomSearch")
        {
            Categories = new[] {"cms"};
            Title = "Instagram";
            Description = "Search for instagram content";
            SortOrder = 10000;
            PlugInAreas = new[] { PlugInArea.AssetsDefaultGroup };
            Settings.Add(new Setting("repositoryKey", InstagramRepositoryDescriptor.RepositoryKey));
        }
    }
}