using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PictureTogether.Services.Models
{
    [DataContract]
    public class AlbumFullModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "pictures")]
        public List<PictureModel> Pictures { get; set; }
    }
}