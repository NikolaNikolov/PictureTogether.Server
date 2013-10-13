﻿using System.IO;
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
        [ActionName("get")]
        public AlbumFullModel Get(int id, string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions<AlbumFullModel>(() =>
            {
                using (var context = new PictureTogetherContext())
                {
                    UsersController.ValidateSessionKey(sessionKey);

                    var album = context.Albums.Find(id);
                    var albumFullModel = new AlbumFullModel
                    {
                        Id = album.Id,
                        Name = album.Name,
                        Latitude = album.Latitude,
                        Longitude = album.Longitude,
                        Pictures = album.Pictures.Select(p => new PictureModel
                        {
                            Id = p.Id,
                            Url = p.Url
                        }).ToList()
                    };

                    return albumFullModel;
                }
            });

            return responseMsg;
        }

        // POST api/albums/picture?sessionKey=...
        [ActionName("picture")]
        public HttpResponseMessage PostPicture(string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions<HttpResponseMessage>(() =>
            {
                UsersController.ValidateSessionKey(sessionKey);
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

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No file received from the server.");
            });

            return responseMsg;
        }

        // POST api/albums/album?sessionKey=...
        [ActionName("album")]
        public HttpResponseMessage PostAlbum(string sessionKey, AlbumFullModel albumFullModel)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions<HttpResponseMessage>(() =>
            {
                using (var context = new PictureTogetherContext())
                {
                    UsersController.ValidateSessionKey(sessionKey);
                    var currentUser = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                    if (currentUser == null)
                    {
                        throw new ArgumentException("Expired or invalid sessionKey. Please try to relog with your account.");
                    }

                    var newAlbum = new Album
                    {
                        Name = albumFullModel.Name,
                        Latitude = albumFullModel.Latitude,
                        Longitude = albumFullModel.Longitude,
                        Users =
                        {
                            currentUser
                        }
                    };

                    currentUser.Albums.Add(newAlbum);
                    context.Albums.Add(newAlbum);
                    context.SaveChanges();

                    albumFullModel.Id = newAlbum.Id;
                    var response = this.Request.CreateResponse(HttpStatusCode.Created, albumFullModel);
                    return response;
                }
            });

            return responseMsg;
        }
    }
}
