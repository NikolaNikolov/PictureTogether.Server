using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PictureTogether.Services.Controllers
{
    public class AppHarborController : BaseApiController
    {
        // GET api/appharbor/wakeup
        [ActionName("wakeup")]
        public bool Get()
        {
            return true;
        }
    }
}
