using System.Collections.Generic;
using System.Linq;
using System.Net;
using EPiServer;
using EPiServer.Construction;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using log4net;

namespace Hackathon.Business.InstagramProvider
{

    public class InstagramContentProvider : ContentProvider
    {
        private IdentityMappingService _identityMappingService;
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<BasicContent> _items = new List<BasicContent>();
        private readonly IContentTypeRepository _contentTypeRepository;

        public InstagramContentProvider(IContentTypeRepository contentTypeRepository)
        {
            _contentTypeRepository = contentTypeRepository;
        }

        public override ContentProviderCapabilities ProviderCapabilities
        {
            get { return ContentProviderCapabilities.Create | ContentProviderCapabilities.Edit; }
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            var item = _items.FirstOrDefault(p => p.ContentLink.Equals(contentLink));
            return item ?? _items.FirstOrDefault();
        }


        protected ContentResolveResult ResolveContent(BasicContent content)
        {
            var contentItem = new ContentCoreData()
            {
                ContentGuid = content.ContentGuid,
                ContentReference = content.ContentLink,
                ContentTypeID = ContentTypeRepository.Service.Load(typeof(InstaImage)).ID,
            };
            return base.CreateContentResolveResult(contentItem);
        }

        public void RefreshItems(List<BasicContent> items)
        {
            _items = items;
            this.ClearProviderPagesFromCache();
        }

        public List<BasicContent> GetAll()
        {
            return _items;
        }

        protected Injected<IContentTypeRepository> ContentTypeRepository { get; set; }
        protected Injected<ContentFactory> ContentFactory { get; set; }
        protected Injected<IContentRepository> ContentRepository { get; set; }

        private WebResponse ProcessWebRequest(string url)
        {
            WebRequest request;
            WebResponse response;

            request = WebRequest.Create(url);
            response = request.GetResponse();

            return response;

        }

        public void Add(InstaImage content)
        {
            _items.Add(content);
        }
    }
}