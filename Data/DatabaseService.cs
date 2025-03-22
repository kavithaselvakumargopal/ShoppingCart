using System;
using System.Configuration;

namespace ShoppingCart.Data
{
    public class DatabaseService
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["MyDbConnection"]?.ConnectionString;
        }
    }
}
