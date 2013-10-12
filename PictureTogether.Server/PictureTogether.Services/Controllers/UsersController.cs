using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using PictureTogether.Data;
using PictureTogether.Models;
using PictureTogether.Services.Models;

namespace PictureTogether.Services.Controllers
{
    public class UsersController : BaseApiController
    {
        private static readonly Random rand = new Random();

        public const int MinUsernameLength = 6;
        public const int MaxUsernameLength = 30;
        public const string ValidUsernameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.";
        public const string SessionKeyChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
        public const int SessionKeyLength = 50;
        public const int Sha1Length = 40;

        // POST api/users/register
        [ActionName("register")]
        public HttpResponseMessage PostRegister(UserModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions<HttpResponseMessage>(
                () =>
                {
                    using (var context = new PictureTogetherContext())
                    {
                        this.ValidateUsername(model.Username);
                        this.ValidateAuthCode(model.AuthCode);

                        var usernameToLower = model.Username.ToLower();
                        var user = context.Users.FirstOrDefault(u => u.Username == usernameToLower);

                        if (user != null)
                        {
                            throw new InvalidOperationException("User already exists.");
                        }

                        user = new User
                        {
                            Username = model.Username,
                            AuthCode = model.AuthCode
                        };

                        context.Users.Add(user);
                        context.SaveChanges();

                        user.SessionKey = this.GenerateSessionKey(user.Id);
                        context.SaveChanges();

                        var loggedModel = new LoggedUserModel
                        {
                            Username = user.Username,
                            SessionKey = user.SessionKey
                        };

                        var response =
                            this.Request.CreateResponse(HttpStatusCode.Created,
                                loggedModel);
                        return response;
                    }
                });

            return responseMsg;
        }

        // POST api/users/login
        [ActionName("login")]
        public HttpResponseMessage PostLogin(UserModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions<HttpResponseMessage>(
                () =>
                {
                    using (var context = new PictureTogetherContext())
                    {
                        this.ValidateUsername(model.Username);
                        this.ValidateAuthCode(model.AuthCode);

                        var usernameToLower = model.Username.ToLower();
                        var user = context.Users.FirstOrDefault(
                            usr => usr.Username == usernameToLower &&
                                   usr.AuthCode == model.AuthCode);

                        if (user == null)
                        {
                            throw new InvalidOperationException("Wrong username or password.");
                        }

                        if (user.SessionKey == null)
                        {
                            user.SessionKey = this.GenerateSessionKey(user.Id);
                            context.SaveChanges();
                        }

                        var loggedModel = new LoggedUserModel
                        {
                            Username = user.Username,
                            SessionKey = user.SessionKey,
                            Albums = user.Albums.Select(a => new AlbumModel
                            {
                                Id = a.Id,
                                Name = a.Name
                            }).ToList()
                        };

                        var response =
                            this.Request.CreateResponse(HttpStatusCode.Created,
                                loggedModel);
                        return response;
                    }
                });

            return responseMsg;
        }

        // PUT api/users/logout?sessionKey=...
        [ActionName("logout")]
        public HttpResponseMessage PutLogout(string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions<HttpResponseMessage>(
                () =>
                {
                    using (var context = new PictureTogetherContext())
                    {
                        ValidateSessionKey(sessionKey);

                        var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                        if (user != null)
                        {
                            user.SessionKey = null;
                            context.SaveChanges();
                        }

                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        return response;
                    }
                });

            return responseMsg;
        }

        public static void ValidateSessionKey(string sessionKey)
        {
            if (sessionKey == null || sessionKey.Length != SessionKeyLength)
            {
                throw new ArgumentOutOfRangeException("Invalid SessionKey.");
            }
        }

        private string GenerateSessionKey(int userId)
        {
            StringBuilder skeyBuilder = new StringBuilder(SessionKeyLength);
            skeyBuilder.Append(userId);
            while (skeyBuilder.Length < SessionKeyLength)
            {
                var index = rand.Next(SessionKeyChars.Length);
                skeyBuilder.Append(SessionKeyChars[index]);
            }
            return skeyBuilder.ToString();
        }

        private void ValidateAuthCode(string authCode)
        {
            if (authCode == null)
            {
                throw new ArgumentNullException("authCode", "Password cannot be null.");
            }

            if (authCode.Length != Sha1Length)
            {
                throw new ArgumentException("authCode", "Password should be encrypted.");
            }
        }

        private void ValidateUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username", "Username cannot be null.");
            }
            else if (username.Length < MinUsernameLength)
            {
                throw new ArgumentException(string.Format("Username should be at least {0} characters.", MinUsernameLength));
            }
            else if (username.Length > MaxUsernameLength)
            {
                throw new ArgumentException(string.Format("Username should be less than {0} characters.", MaxUsernameLength));
            }
            else if (username.Any(ch => !ValidUsernameCharacters.Contains(ch)))
            {
                throw new ArgumentException(string.Format("Username should contains only these characters {0}.", ValidUsernameCharacters));
            }
        }
    }
}
