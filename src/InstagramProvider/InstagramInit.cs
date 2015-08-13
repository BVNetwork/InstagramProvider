using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Construction;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using log4net;

namespace Hackathon.Business.InstagramProvider
{
    [ModuleDependency(typeof(DataInitialization))]
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class InstagramProviderInitialization : IInitializableModule
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
     //   protected Injected<SettingsRepository> SettingsRepository { get; set; }

        public void Initialize(InitializationEngine context)
        {
     
            var registerFolder = context.Locate.Advanced.GetInstance<SingleModelRegister<InstagramFolder>>();
            registerFolder.RegisterType();
            var registerInstaImage = context.Locate.Advanced.GetInstance<SingleModelRegister<InstaImage>>();
            registerInstaImage.RegisterType();

            var contentRepository = context.Locate.ContentRepository();
            var entryPoint = contentRepository.GetChildren<InstagramFolder>(ContentReference.RootPage).FirstOrDefault();
            if (entryPoint == null)
            {
                entryPoint = contentRepository.GetDefault<InstagramFolder>(ContentReference.RootPage);
                entryPoint.Name = "Instagram";
                contentRepository.Save(entryPoint, SaveAction.Publish, AccessLevel.NoAccess);
            }

            // Register custom content provider
            var providerValues = new NameValueCollection();
            providerValues.Add(ContentProviderElement.EntryPointString, entryPoint.ContentLink.ID.ToString());
            providerValues.Add(ContentProviderElement.CapabilitiesString, ContentProviderElement.FullSupportString);
            var instagramProvider = new InstagramContentProvider((context.Locate.ContentTypeRepository()));
            instagramProvider.Initialize("instagramprovider", providerValues); 
            var providerManager = context.Locate.Advanced.GetInstance<IContentProviderManager>();
            providerManager.ProviderMap.AddProvider(instagramProvider);
        }

        protected Injected<IContentTypeRepository> ContentTypeRepository { get; set; }
        protected Injected<ContentFactory> ContentFactory { get; set; }


        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public InstaImage PopulateStandardVideoProperties(InstaImage instaImage)
        {
            instaImage.ContentLink = new ContentReference(Convert.ToInt32(1), 0, "instagramprovider");
            instaImage.Created = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
            instaImage.Changed = instaImage.Created;
            instaImage.IsPendingPublish = false;
            instaImage.StartPublish = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
            instaImage.Status = VersionStatus.Published;
            instaImage.MakeReadOnly();
            return instaImage;
        }
    }
}