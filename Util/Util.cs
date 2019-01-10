
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MpesaSTKPushAPI
{
    public class Util
    {
       
	    //public static string GetDbConnString()
        //{
			//add your db connection here
			 //        return connString;
		//}
        public static void LogError(string moduleName, Exception ex, bool isError = true)
        {
            try
            {
               string logDir = Path.Combine(@"C:\ApplicationLogs", "MpesaSTKPushAPI");

                //---- Create Directory if it does not exist              
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                string logFile = Path.Combine(logDir, "ErrorLog.log");
                //--- Delete log if it more than 500Kb
                if (File.Exists(logFile))
                {
                    FileInfo fi = new FileInfo(logFile);
                    if ((fi.Length / 1000) > 500)
                        fi.Delete();
                }
                //--- Create stream writter
                StreamWriter stream = new StreamWriter(logFile, true);
                stream.WriteLine(string.Format("{0}|{1:dd-MMM-yyyy HH:mm:ss}|{2}|{3}",
                    isError ? "ERROR" : "INFOR",
                    DateTime.Now,
                    moduleName,
                    isError ? ex.ToString() : ex.Message));
                stream.Close();
            }
            catch (Exception e) { }
        }