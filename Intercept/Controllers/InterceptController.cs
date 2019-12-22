using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intercept.Models;
using Intercept.Data;

namespace Intercept.Controllers
{
    public class InterceptController : Controller
    {
        // GET: Intercept
        public ActionResult Index()
        {
            Currency currency = new Currency();
            currency.Date = DateTime.Today;
            return View(currency);
        }

        public ActionResult GetRate(Currency currency)
        {
            if (ModelState.IsValid)
            {
                DataAccess dataAccess = new DataAccess();
                currency.Rate = dataAccess.GetConversionRate(currency.CurrToConvert, currency.Date);
                currency.NewAmount = currency.Amount * double.Parse(currency.Rate.ToString());
            }
            return View(currency);
        }

        public ActionResult Save(Currency currency)
        {
            if (ModelState.IsValid)
            {
                DataAccess dataAccess = new DataAccess();
                string errMsg = "";
                dataAccess.AuditRate(currency, out errMsg);
                if (errMsg.Length == 0)
                    return RedirectToAction("Home");
                else
                    Response.Write(errMsg);
            }
            return View();
        }
        
        [HttpGet]
        public ActionResult Home()
        {
            Data.DataAccess dataAccess = new DataAccess();
            List<Currency> audit = dataAccess.GetAudit();
            return View(audit);
        }

        [HttpPost]
        public ActionResult Home(DateTime startDate, DateTime endDate)
        {
            Data.DataAccess dataAccess = new DataAccess();
            List<Currency> audit = dataAccess.GetAudit();
            if (startDate != null)
            {
                var filteredAudits = (from a in audit
                                      where a.Date >= startDate && a.Date <= endDate
                                      select a);
                return View(filteredAudits);
            }
            else
                return View(audit);
            
        }

    }
}