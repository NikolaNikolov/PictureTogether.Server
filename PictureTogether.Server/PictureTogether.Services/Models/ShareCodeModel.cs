using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PictureTogether.Services.Models
{
    [DataContract]
    public class ShareCodeModel
    {
        [DataMember(Name = "albumId")]
        public int AlbumId { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}