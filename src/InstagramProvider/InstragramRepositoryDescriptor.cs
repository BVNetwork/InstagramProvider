using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Web;

namespace Hackathon.Business.InstagramProvider
{
    [ServiceConfiguration(typeof(IContentRepositoryDescriptor))]
    public class InstagramRepositoryDescriptor : ContentRepositoryDescriptorBase
    {
        public static string ProvideKeyValue
        {
            get { return "instagramprovider"; }
        }
        public static string RepositoryKey
        {
            get { return "instagramprovider"; }
        }

        public override string SearchArea
        {
            get { return "instagramprovider"; }
        }

        public override string Key
        {
            get { return RepositoryKey; }
        }

        public override string Name
        {
            get { return "instagramprovider"; }
        }

        public override IEnumerable<Type> ContainedTypes
        {
            get { return new Type[] { typeof(InstaImage) }; }
        }


        public override IEnumerable<Type> MainNavigationTypes
        {
            get
            {
                return new Type[2]
                        {
                          typeof (ContentFolder), typeof(InstaImage)
                        };
            }
        }


        public override int SortOrder
        {
            get { return 400; }
        }

        public bool ChangeContextOnItemSelection { get { return true; } }

        public override IEnumerable<ContentReference> Roots
        {
            get { return SiteDefinition.Current.GlobalAssetsRoot.Yield(); }
        }

        public override IEnumerable<string> PreventContextualContentFor
        {
            get
            {
                return (IEnumerable<string>)new string[3]
        {
          ContentReference.RootPage.ToString(),
          ContentReference.WasteBasket.ToString(),
          ContentReference.GlobalBlockFolder.ToString()
        };
            }
        }
    }
}