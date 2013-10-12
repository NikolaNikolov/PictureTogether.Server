using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PictureTogether.Services.Models
{
    [DataContract]
    public class LoggedUserModel
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "sessionKey")]
        public string SessionKey { get; set; }

        [DataMember(Name = "albums")]
        public List<AlbumModel> Albums { get; set; }
    }
}