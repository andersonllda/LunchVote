#region Header

// Copyright (c) Omar AL Zabir. All rights reserved.
// For continued development and updates, visit http://msmvps.com/omar

#endregion Header

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace HMV.Core.Framework.Extensions
{
    /// <summary>
    /// Summary description for ExtensionFunctions
    /// </summary>
    public static class ExtensionFunctions
    {
        #region Methods

        [DebuggerStepThrough]
        public static void Each<T>(this T[] array, Action<T, int> act)
        {
            int counter = 0;
            foreach (T item in array)
                act(item, counter++);
        }
        
        [DebuggerStepThrough]
        public static void Each<T>(this IList<T> list, Action<T, int> act)
        {
            int counter = 0;
            foreach (T item in list)
                act(item, counter++);
        }

        [DebuggerStepThrough]
        public static void Each<T>(this IList<T> list, Action<T> act)
        {
            foreach (T item in list)
                act(item);
        }

        [DebuggerStepThrough]
        public static bool HasItems<T>(this IList<T> list)
        {
            return (list != null) && (list.Count > 0);
        }
        

        [DebuggerStepThrough]
        public static bool HasItems<T>(this T[] list)
        {
            return (list != null) && (list.Count() > 0);
        }

        
        public static bool IsEmpty(this Guid g)
        {
            return g.Equals(Guid.Empty);
        }

        
        [DebuggerStepThrough]
        public static string Percent(this int value)
        {
            return value + "%";
        }

        [DebuggerStepThrough]
        public static string Pixels(this int value)
        {
            return value + "px";
        }

        [DebuggerStepThrough]
        public static void Times(this int count, Action<int> doSomething)
        {
            for (int i = 0; i < count; i++)
                doSomething(i);
        }

        [DebuggerStepThrough]
        public static string Xml(this XElement e)
        {
            StringBuilder builder = new StringBuilder();
            XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder));
            e.WriteTo(writer);
            return builder.ToString();
        }

        public static string ToXml(this XmlDocument doc)
        {
            var buffer = new StringBuilder(1000);
            doc.WriteContentTo(new XmlTextWriter(new StringWriter(buffer)));
            return buffer.ToString();
        }                   
        #endregion Methods
    }
}