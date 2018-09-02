using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace UltimateFishBot
{
    internal class Translate
    {
        private const string MissingTranslation = "<MISSING TRANSLATION>";
        private static XmlElement _mElements = null;

        private static void ExtractElements()
        {
            if (_mElements == null)
            {
                XmlDocument doc = new XmlDocument();

                try
                {
                    // Example : ./Resources/English.xml
                    doc.Load("./Resources/" + Properties.Settings.Default.Language + ".xml");
                    _mElements = doc.DocumentElement;
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                }
            }
        }

        public static string GetTranslate(string formName, string nodeName, params object[] list)
        {
            ExtractElements();
            string returnText = MissingTranslation;

            // If we can't open the Translation file, everything will appear as "MISSING TRANSLATION"
            if (_mElements == null)
                return returnText;

            try
            {
                XmlNodeList formList = _mElements.GetElementsByTagName(formName);

                // Try to find the correct translation for formName and nodeName
                foreach (XmlNode mainNode in formList)
                    foreach (XmlNode node in mainNode.ChildNodes)
                        if (node.Name == nodeName)
                        {
                            returnText = node.InnerText;
                            break;
                        }

                // Remove the extras spaces from each lines
                returnText = string.Join("\n", returnText.Split('\n').Select(s => s.Trim()));

                // Replace {int} in text by variables. Ex : "Waiting for Fish ({0}/{1}s) ..."
                returnText = string.Format(returnText, list);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }

            return returnText;
        }
        public static IEnumerable<string> GetTranslates(string formName, string nodeName, params object[] list)
        {
            ExtractElements();
            List<string> returnList = new List<string>();

            // If we can't open the Translation file, everything will appear as "MISSING TRANSLATION"
            if (_mElements == null)
            {
                returnList.Add(MissingTranslation);
                return returnList;
            }

            try
            {
                XmlNodeList formList = _mElements.GetElementsByTagName(formName);

                // Try to find the correct translation for formName and nodeName
                foreach (XmlNode mainNode in formList)
                    foreach (XmlNode node in mainNode.ChildNodes)
                        if (node.Name == nodeName)
                            returnList.Add(node.InnerText);

                // Remove the extras spaces from each lines
                var enumerable = returnList.Select(text => string.Join("\n", text.Split('\n').Select(s => s.Trim())));

                // Replace {int} in text by variables. Ex : "Waiting for Fish ({0}/{1}s) ..."
                return enumerable.Select(text => string.Format(text, list));
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }

            return returnList;
        }
    }
}
