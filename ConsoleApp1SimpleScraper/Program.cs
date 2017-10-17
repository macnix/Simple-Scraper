using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1SimpleScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            #region read csv
            var sites = File.ReadLines(@"c:\tmp\1-SampleInputFile.csv")
               .Select(line => line.Split(','))
               .Skip(1)
               .Select(fields => new inputwww { URL = fields[0]})
               .ToList();
            #endregion

            #region parse urls
            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            List<outputwww> output = new List<outputwww>();
            foreach (var input in sites)
            {
                doc = hw.Load(input.URL);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    // Get the value of the HREF attribute
                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                    if (!hrefValue.Contains(input.URL) && hrefValue.Contains("//"))
                    {
                        output.Add(new outputwww { INPUT_URL = input.URL, EXTERNAL_URL = hrefValue });
                    }
                }
            }
            #endregion

            #region Save result in csv
            string csvHeaderRow = String.Join(",", typeof(outputwww).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name).ToArray<string>()) + Environment.NewLine;
            string csv = csvHeaderRow + String.Join(Environment.NewLine, output.Select(x => string.Join(",", x.INPUT_URL, x.EXTERNAL_URL)).ToArray());
            File.WriteAllLines(@"c:\tmp\2-SampleOutputFile.csv", new string[] { csv }, Encoding.Unicode);

            Console.WriteLine("Done!");
            Console.ReadKey();
            #endregion

        }
    }

    internal class inputwww
    {
        public string URL { get; set; }
    }
    internal class outputwww
    {
        public string INPUT_URL { get; set; }
        public string EXTERNAL_URL { get; set; }
    }
}
