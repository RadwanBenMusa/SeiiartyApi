 using static TamaApi.Services.db.DbService;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System;
using TamaApi.clsMod;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TamaApi.Controllers;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace TamaApi.Services.db
{
    public static class DoSomeThings
    {
        static public string GetFromat(DateTime dateTime) 
        {
            return $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day} 00:00:00";
        }
    }
}
