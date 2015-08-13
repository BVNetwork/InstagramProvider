using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;

namespace Hackathon.Business.InstagramProvider
{
    [ContentType(GUID = "C33D2B5C-3EF9-461B-9C4B-834356E08C1C")]
    public class InstaImage : ContentBase
    {
        public virtual string ImageUrl { get; set; }
        public virtual int Likes { get; set; }
        public virtual string InstagramId { get; set; }
        public virtual string Text{ get; set; }
        public virtual Blob Thumbnail { get{return new FileBlob(new Uri(""),"test");} }
        public virtual string ThumbnailUrl { get; set; }

    }

    public class InstagramFolder : ContentFolder
    {
    }
}