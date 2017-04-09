using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PowerBISample.Controllers
{
    [Authorize]
    public class OrgPowerBIController : BaseController
    {
        // GET: OrgPowerBI
        public ActionResult Reports()
        {
            PTI.PowerBI.Wrapper.PowerBIClient objPowerBIClient = new PTI.PowerBI.Wrapper.PowerBIClient(base.PowerBIAccessToken);
            var reports = objPowerBIClient.GetReports();
            return View(reports.Data);
        }
    }
}