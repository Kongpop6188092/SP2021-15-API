using MySqlConnector;
using System;
using System.Collections.Generic;

namespace MUDENT_API.Services.Interface
{
    public interface IUtilService
    {
        public string GetExceptionMessage(Exception ex);
        public string GetValidateMessage(string s);
        public bool ConvertStatus(MySqlParameter status);
        public List<string> DatetimeToString(DateTime dt);
    }
}
