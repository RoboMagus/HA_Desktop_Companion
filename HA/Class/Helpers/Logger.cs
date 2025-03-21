﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using static System.Net.Mime.MediaTypeNames;

namespace HA.Class.Helpers
{
    public class Logger
    {
        private static bool initialized = false;
        private static string path1;
        private static string[] secreetsStrings = new string[] { };

#if DEBUG
        private static string appDir = Directory.GetCurrentDirectory();
#else
        private static string appDir = AppDomain.CurrentDomain.BaseDirectory;
#endif

        public static void setSecreets(string[] strings)
        {
            if (!initialized)
            {
                init();
            }
            secreetsStrings = strings.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }

        public static void init(string path = "./log.log")
        {
            path1 = Path.Combine(appDir, path).ToString();
            if (!File.Exists(path1))
            {
                File.WriteAllText(path1, getMessage("Initialization", 0 /*info*/), System.Text.Encoding.UTF8);
            }
            initialized = true;
        }

        /*
            0 - Info
            1 - warning
            3 - error
            4 - seecrets
         */

        public static void write(string msg, int level = 0)
        {
            if (!initialized)
            {
                init();
            }

            Debug.WriteLine(msg);
            var writer = new FileStream(path1, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            var streamWriter = new StreamWriter(writer);

            streamWriter.Write(getMessage(msg, level));

            streamWriter.Dispose();
            writer.Dispose();
        }

        public static void write(object msg, int level = 0)
        {
            if (!initialized)
            {
                init();
            }

            string msg_str = msg.ToString();
            Debug.WriteLine(msg_str);
            
            File.AppendAllText(path1, getMessage(msg_str, level), System.Text.Encoding.UTF8);

        }

        public static string getLogPath()
        {
            return path1;
        }
        
        private static string getMessage(string text, int level = 0)
        {
            string parsedText = text;
            if (secreetsStrings.Length > 0)
            {
                foreach (string secret in secreetsStrings) {
                    parsedText = parsedText.Replace(secret, "***SECRET****");
                }
            }

            return DateTime.Now.ToString("[MM/dd/yyyy-HH:mm:ss]") + "[" + level + "]" + parsedText + "\n";
        }
    }
}
