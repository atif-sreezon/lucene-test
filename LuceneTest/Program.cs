using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using static System.Console;
using System.Windows.Forms;

namespace LuceneTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string filepath = null;
            string curPath = Directory.GetCurrentDirectory();
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "JSON|*.json";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filepath = ofd.SafeFileName;
            }

            string json = null;

            try { json = File.ReadAllText(filepath); }
            catch (Exception ex) { WriteLine("Error: " + ex.Message.ToString()); }
            
            List<records> storeRecords = DeserializeJSON(json);

            WriteLine("Hello Lucene.Net");

            LuceneApplication LuceneApp = new LuceneApplication();

            LuceneApp.CreateIndex(curPath);
            WriteLine(curPath);
            WriteLine("Adding Documents to Index");

            for (int x = 0; x < storeRecords.Count; x++)
            {
                WriteLine("Adding record no #{0}", x+1);
                for (int y = 0; y < storeRecords[x].passages.Count; y++)
                {
                    string single_text = storeRecords[x].passages[y].url.ToString() + storeRecords[x].passages[y].passage_text.ToString();
                    //WriteLine("URL: {0}", storeRecords[x].passages[y].url.ToString());
                    //WriteLine("Passage Text: {0}", storeRecords[x].passages[y].passage_text.ToString());
                    LuceneApp.IndexText(single_text);
                    //LuceneApp.IndexText(storeRecords[x].passages[y].passage_text);
                }
            }

            WriteLine("All documents added.");

            // clean up
            LuceneApp.CleanUpIndexer();

            LuceneApp.CreateSearcher();
            LuceneApp.CreateParser();

            string QUIT = "q";

            Write("Enter your query >>");
            string queryText = ReadLine();

            while (queryText != QUIT)
            {
                LuceneApp.DisplayResults(LuceneApp.SearchIndex(queryText));
                Write("Enter your query or press 'q' to exit >>");
                queryText = ReadLine();
            }            

            //WriteLine("Press Enter to exit.");
            //ReadLine();
        }
        public static List<records> DeserializeJSON(string jsonText)
        {
            try
            {
                var readRecords = JsonConvert.DeserializeObject<List<records>>(jsonText);
                
                return readRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message.ToString());
                return null;
            }
        }

        public static void DisplayOutput(string text)
        {
            Console.WriteLine(text);
        }
    }
}
