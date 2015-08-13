using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Construction;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;
using EPiServer.Shell.Search;
using Hackathon.Business.InstagramProvider;
using Newtonsoft.Json;

namespace Hackathon.Business.UIExtensions
{
    [ServiceConfiguration(typeof(IContentQuery))]
    public class InstagramQuery : ContentQueryBase
    {

        private readonly IContentRepository _contentRepository;
        private readonly SearchProvidersManager _searchProvidersManager;
        private readonly LanguageSelectorFactory _languageSelectorFactory;

        public InstagramQuery(
            IContentQueryHelper queryHelper,
            IContentRepository contentRepository,
            SearchProvidersManager searchProvidersManager,
            LanguageSelectorFactory languageSelectorFactory)
            : base(contentRepository, queryHelper)
        {
            _contentRepository = contentRepository;
            _searchProvidersManager = searchProvidersManager;
            _languageSelectorFactory = languageSelectorFactory;
        }
        /// <summary>        
        /// The key to trigger this query.        
        /// </summary>        
        public override string Name
        {
            get { return "InstagramQuery"; }
        }



        protected override IEnumerable<IContent> GetContent(ContentQueryParameters parameters)
        {
            var videoList = new List<InstaImage>();
            var queryText = HttpUtility.HtmlDecode(parameters.AllParameters["queryText"]);
            if (queryText != "episerver")
                return videoList;

            var providerManager = ServiceLocator.Current.GetInstance<IContentProviderManager>();
            var provider = providerManager.ProviderMap.GetProvider("instagramprovider") as InstagramContentProvider;

            provider.RefreshItems(new List<BasicContent>());
            var entryPoint = ContentRepository.Service.GetChildren<InstagramFolder>(ContentReference.RootPage).FirstOrDefault();
            var type = ContentTypeRepository.Service.Load<InstaImage>();
            WebResponse response = ProcessWebRequest("https://api.instagram.com/v1/tags/" + queryText + "/media/recent?client_id=YOUR_ID_HERE");

            using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
            {

                InstagramObject _instagram = JsonConvert.DeserializeObject<InstagramObject>(sr.ReadToEnd());


                int totalPhotos = _instagram.data.Count - 1;

                int count = 0;
                while (count < totalPhotos)
                {

                    string tags = "";

                    foreach (object o in _instagram.data[count].tags)
                    {
                        tags += "#" + o + ",";

                    }


                    var video =
                        ContentFactory.Service.CreateContent(type, new BuildingContext(type)
                        {
                            Parent = entryPoint,
                            LanguageSelector = LanguageSelector.AutoDetect(),
                            SetPropertyValues = true
                        }) as InstaImage;

                    video.ContentLink = new ContentReference(Convert.ToInt32(CountHack.count), "instagramprovider");
                    video.Name = _instagram.data[count].images.low_resolution.url; // _instagram.data[count].caption != null ? _instagram.data[count].caption.text : "No caption.";
                    video.Created = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
                    video.Saved = new DateTime(Convert.ToInt32(_instagram.data[count].created_time));
                    video.Changed = video.Created;
                    video.CreatedBy = _instagram.data[count].user.username;
                    video.InstagramId = _instagram.data[count].id;
                    video.IsPendingPublish = false;
                    video.StartPublish = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                    video.Status = VersionStatus.Published;
                    video.IsPendingPublish = false;
                    video.ImageUrl = _instagram.data[count].images.standard_resolution.url;
                    video.ThumbnailUrl = _instagram.data[count].images.thumbnail.url;
                    video.Likes = _instagram.data[count].likes.count;
                    video.Text = _instagram.data[count].caption.text;
                    video.MakeReadOnly();
                    CountHack.count++;
                    count++;
                    provider.Add(video);
                    //( yield return video;
                    videoList.Add(video);


                }

            }
            return videoList;





            //    return _contentRepository.Get<StartPage>(ContentReference.StartPage).Yield();


            //var provider = new InstagramContentProvider(ServiceLocator.Current.GetInstance<IContentTypeRepository>());
            ////return provider.GetAll();


            //var queryText = HttpUtility.HtmlDecode(parameters.AllParameters["queryText"]);
            //var searchQuery = new Query(queryText);
            //var contentReferences = Enumerable.Empty<ContentReference>();
            //var searchProvider = _searchProvidersManager.GetEnabledProvidersByPriority("CMS/Pages", true).FirstOrDefault();

            //if (searchProvider != null)
            //{
            //    contentReferences = searchProvider.Search(searchQuery).Select(result => ContentReference.Parse(result.Metadata["Id"])).Distinct();
            //}
            //return _contentRepository.GetItems(contentReferences, _languageSelectorFactory.AutoDetect(parameters.AllLanguages));
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
    }
}