using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace deneme5
{
    internal class Helper
    {
        delegate string CommandDelegete(params string[] obj);
        static Dictionary<string,CommandDelegete> commandDictionary=new Dictionary<string, CommandDelegete>()
                                                             {
                                                                 {"getLogs",GetLogList},
                                                                 {"getFile",GetFile}
                                                             };
        internal static string RunMethod(string command,params string[] param)
        {
            if (!commandDictionary.ContainsKey(command))
            {
                return "istenen metot bulunamadı.";
            }
            return commandDictionary[command].Invoke(param);
        }

        private static string GetLogList(params string[] obj)
        {
            string path = @"C:\Tsoft\Entegrasyon\Log\";
            string retval = "[";
            foreach (string file in System.IO.Directory.GetFiles(path, "*.log"))
            {
                retval += "\"" + file.Replace(path, "") + "\",";
            }
            retval += "]";
            return retval;
        }
        private static string GetFile(params string[] fileName)
        {
            string path = @"C:\Tsoft\Entegrasyon\Log\" + (fileName[0] as string);
            if (!System.IO.File.Exists(path))
                return "{fileContent:\"Dosya Bulunamadı\"}";
            System.IO.FileStream fs=new FileStream(path,FileMode.Open,FileAccess.Read);
            StreamReader sr=new StreamReader(fs);
            var content = sr.ReadToEnd().Replace("\\","\\\\").Replace(Environment.NewLine,"<br/>");
            string retval = "\"" + content + "\"";
            return retval;
        }
    }
}
