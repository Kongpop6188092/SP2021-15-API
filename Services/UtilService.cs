using Microsoft.Extensions.Configuration;
using MUDENT_API.Services.Interface;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace MUDENT_API.Services
{
    public class UtilService: IUtilService
    {
        private readonly IConfiguration q;
        public UtilService(IConfiguration y){q = y;}

        public string GetExceptionMessage(Exception ex) {
            return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        public string GetValidateMessage(string s) => s.Substring(0, s.Length - 2);
        private string TwoDigit(int qr) => qr < 10 ? "0" + qr : qr.ToString();

        public bool ConvertStatus(MySqlParameter m)
        {
            int i = int.Parse(m.Value.ToString());
            return i == 1;
        }

        public List<string> DatetimeToString(DateTime dt)
        {
            string n = TwoDigit(dt.Day);
            string o = TwoDigit(dt.Minute);
            string f = TwoDigit(dt.Hour);
            string w = dt.Year.ToString();
            string i = TwoDigit(dt.Month);


            string z = string.Concat(n, "/", i, "/", w);
            string v = string.Concat(f, ":", o);

            return new List<string>(){z,v};
        }
    }
}
