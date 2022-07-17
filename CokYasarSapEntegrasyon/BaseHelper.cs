using System;
using System.Collections.Generic;
using System.IO;


namespace CokYasarSapEntegrasyon
{
    public static class BaseHelper
    {
        public static void Log(string msj)
        {
            try
            {
                msj = DateTime.Now.ToLongTimeString() + "  " + msj;
                //DateTime Bugun = new DateTime(); 
                //string filename = Path.Combine("Narsoft/",
                //string.Concat(DateTime.Now.ToShortDateString()
                // + "-hatasiz", ".txt"));
                //FileStream fs = null;
                string FileName = @"c:\NARSOFT\Log\" + DateTime.Now.ToShortDateString() +
                    "-GetPuantaj" + ".txt";
                lock (FileName)
                {
                    if (!File.Exists(FileName))
                    {
                        StreamWriter dosya = File.CreateText
                   (FileName);
                        dosya.WriteLine("-------");
                        dosya.WriteLine(msj);
                        dosya.Close();
                    }
                    else
                    {
                        StreamWriter dosya = File.AppendText(FileName);
                        dosya.WriteLine("-------");
                        dosya.WriteLine(msj);
                        dosya.Close();
                    }
                }
            }
            catch
            {
            }
        }

        public static string ConvertNumberToTime(string number)
        {
            if (string.IsNullOrEmpty(number))
                return number;

            if (number.Length != 6)
                return string.Empty;

            var numberLeft = number.Substring(0,2);
            var numberMiddle = number.Substring(2,2);
            var numberRight = number.Substring(4,2);

            return $"{numberLeft}:{numberMiddle}:{numberRight}";
        }
    }
}