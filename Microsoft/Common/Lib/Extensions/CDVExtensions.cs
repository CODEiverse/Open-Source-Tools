/*****************************
Project:    CODEiverse - Open Source Tools (OST)
Created By: EJ Alexandra - 2016
            An Abstract Level, llc
License:    Mozilla Public License 2.0
*****************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODEiverse.OST.Lib
{
    public static class CDVExtensions
    {
        public static string SafeToString(this object objectToMakeText)
        {
            if (ReferenceEquals(objectToMakeText, null)) return String.Empty;
            return objectToMakeText.ToString();
        }

        public static T Get<T>(this string[] args)
            where T : class
        {
            if (typeof(T) == typeof(Uri))
            {
                var uriString = args.GetFirst<String>();
                if (!String.IsNullOrEmpty(uriString))
                {
                    return new Uri(uriString) as T;
                }
            }

            // If we get here - return nothing
            return default(T);
        }

        public static T GetFirst<T>(this String[] args)
            where T : class
        {
            if (typeof(T) == typeof(String))
            {
                if (ReferenceEquals(args, null)) return String.Empty as T;
                else if (!args.Any()) return String.Empty as T;
                else return args[0] as T;
            }
            else
            {
                var msg = String.Format("Handler not writtent o handle finding first argument of type '{0}'", typeof(T).Name);
                throw new NotImplementedException(msg);
            }
        }
    }
}
