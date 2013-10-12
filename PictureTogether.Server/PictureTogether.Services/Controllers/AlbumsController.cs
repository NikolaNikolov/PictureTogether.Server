using System.IO;
using System.Web;
using PictureTogether.Data;
using PictureTogether.Models;
using PictureTogether.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PictureTogether.Services.Controllers
{
    public class AlbumsController : BaseApiController
    {
        // GET api/albums/id?sessionKey=...
        //public IEnumerable<AlbumFullModel> Get(int id, string sessionKey)
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // POST api/albums
        //public void Post([FromBody]string value)
        //{
        //}

        // POST api/albums/picture
        [ActionName("picture")]
        public HttpResponseMessage PostPicture()
        {
            var responseMsg = this.PerformOperationAndHandleExceptions<HttpResponseMessage>(
               () =>
               {
                   var files = HttpContext.Current.Request.Files;

                   if (files.Count > 0)
                   {
                       for (int i = 0; i < files.Count; i++)
                       {
                           var postedFile = files[i];
                           var filePath = HttpContext.Current.Server.MapPath(Path.GetTempPath() + postedFile.FileName);
                           postedFile.SaveAs(filePath);

                           using (var context = new PictureTogetherContext())
                           {
                               context.Albums.First().Pictures.Add(
                                   new Picture
                                   {
                                       Url = filePath
                                   });

                               context.SaveChanges();
                           }
                       }

                       return this.Request.CreateResponse(HttpStatusCode.Created);
                   }

                   return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "error");
               });

            return responseMsg;
        }
    }
}
