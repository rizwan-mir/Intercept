using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using Intercept.Models;

namespace Intercept.Data
{
    public class DataAccess
    {

        public double GetConversionRate(string currName, DateTime currDate)
        {
            double cRate = 0.00;
            DateTime cDate = currDate.AddDays(-1);
            DateTime cDate2;
            int i = 0;
            XmlDocument doc = new XmlDocument();
            var path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            doc.Load(path + @"\Data\Currency.xml");
            XmlNode currencyNodes = doc.SelectSingleNode("Currencies");
            foreach (XmlNode node in currencyNodes)
            {
                if (node.Attributes["Name"].Value.ToString() == currName)
                {
                    foreach (XmlNode rateNode in node["Rates"])
                    {
                        if (i == 0)
                        {
                            cDate2 = DateTime.Parse(rateNode.Attributes["Date"].Value.ToString());
                            if (cDate2 <= currDate)
                            {
                                cRate = double.Parse(rateNode.Attributes["Amount"].Value.ToString());
                                cDate = cDate2;
                            }
                        }
                        else
                        {
                            cDate2 = DateTime.Parse(rateNode.Attributes["Date"].Value.ToString());
                            if (currDate >= cDate2 && cDate2 >= cDate)
                            {
                                cDate = DateTime.Parse(rateNode.Attributes["Date"].Value.ToString());
                                cRate = double.Parse(rateNode.Attributes["Amount"].Value.ToString());
                            }
                        }                        
                        i++;
                    }
                }
            }
            return cRate;
        }

        public void AuditRate(Currency currency, out string errMsg)
        {
            try
            {
                errMsg = "";
                XmlDocument doc = new XmlDocument();
                var path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + @"\Data\Audit.xml";
                doc.Load(path);
                XmlElement root = doc.CreateElement("Audits");
                XmlElement xCurrency = doc.CreateElement("Audit");
                xCurrency.SetAttribute("Currency", currency.CurrToConvert);
                xCurrency.SetAttribute("Amount", currency.Amount.ToString());
                xCurrency.SetAttribute("Rate", currency.Rate.ToString());
                xCurrency.SetAttribute("Date", currency.Date.ToString("yyyy-MM-dd"));
                xCurrency.SetAttribute("NewAmount", currency.NewAmount.ToString());
                //root.AppendChild(xCurrency);
                doc.DocumentElement.AppendChild(xCurrency);
                doc.Save(Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + @"\Data\Audit.xml");
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
            }
        }

        public List<Currency> GetAudit()
        {
            List<Currency> audit = new List<Currency>();
            XmlDocument doc = new XmlDocument();
            var path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            doc.Load(path + @"\Data\Audit.xml");
            XmlNode auditNodes = doc.SelectSingleNode("Audits");
            foreach (XmlNode node in auditNodes)
            {
                Currency currency = new Currency();
                currency.CurrToConvert = node.Attributes["Currency"].Value;
                currency.Amount = double.Parse(node.Attributes["Amount"].Value);
                currency.Rate = double.Parse(node.Attributes["Rate"].Value);
                currency.Date = DateTime.Parse(node.Attributes["Date"].Value);
                currency.NewAmount = double.Parse(node.Attributes["NewAmount"].Value);

                audit.Add(currency);
            }

            return audit;
        }

    }
}