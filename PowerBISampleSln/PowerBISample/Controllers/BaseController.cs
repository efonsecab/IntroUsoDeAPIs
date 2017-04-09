using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PowerBISample.Controllers
{
    public class BaseController : Controller
    {
        public string PowerBIAccessToken
        {
            get
            {
                return Convert.ToString(Session[GlobalConstants.PowerBIAccessToken]);
            }
        }
    }
}