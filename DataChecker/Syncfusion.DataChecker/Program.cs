using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.DataChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Get local log file location and the TSDB server location to test the data integrity.
                if (args.Length == 0)
                    Console.WriteLine("Empty arguments.\n Invoke below command with required arguments \n Syncfusion.DataChecker.exe \"logFileLocation\" \"TSDBHostName:PortNumber\"");

                string filePath = args[0];
                string tsdbHost = args[1];

                //Extract data from given log file as a dictionary.
                Dictionary<string, Dictionary<string, string>> metricCollection = ExtractData(filePath);

                foreach (string metric in metricCollection.Keys)
                {
                    //Compare time stamp data points for each metric
                    bool success = CompareWithTSDBData(tsdbHost, metric, metricCollection[metric]);

                    if (success)
                        Console.WriteLine("Data integrity check success." + metric);
                    else
                        Console.WriteLine("Data integrity check failed."+metric);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
        /// <summary>
        /// Returns true if TSDB data points are equal to the input data points
        /// </summary>
        /// <param name="tsdbHost"></param>
        /// <param name="metric"></param>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        private static bool CompareWithTSDBData(string tsdbHost, string metric, Dictionary<string, string> dataPoints)
        {
            //Get startDate and endDate from the time stamp values present in dataPoints
            List<String> timeStamps = new List<string>(dataPoints.Keys);
            timeStamps.Sort();
            string startDate = timeStamps[0];
            string endDate = timeStamps[timeStamps.Count - 1];

            //Get data points from TSDB server for each metric
            Dictionary<string, string> tsdbDataPoints = GetTSDBData(tsdbHost, metric, startDate, endDate);
            //Compare the data points
            return CompareDataPoints(dataPoints, tsdbDataPoints);
        }
        /// <summary>
        /// Get TSDB JSON data and return it as a dictionary collection
        /// </summary>
        /// <param name="tsdbHost"></param>
        /// <param name="metric"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetTSDBData(string tsdbHost, string metric, string startDate, string endDate)
        {
            Dictionary<string, string> tsdbDataPoints = new Dictionary<string, string>();
            string json;
            //REST call to fetch the JSON data from TSDB server for a given metric
            string url = "http://" + tsdbHost + "/api/query?start=" + startDate + "&end=" + endDate + "&m=sum:" + metric;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 1000000;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }

            //Parse the JSON Data into dictionary collection
            dynamic jObject = JsonConvert.DeserializeObject(json);
            JObject obj = jObject[0] as JObject;
            JToken jtoken;
            obj.TryGetValue("dps", out jtoken);
            dynamic jsonValue = JsonConvert.DeserializeObject(jtoken.ToString());
            foreach (var tempValue in jsonValue)
            {
                tsdbDataPoints.Add(tempValue.Name,Convert.ToString(tempValue.Value));
            }
            return tsdbDataPoints;
        }
        /// <summary>
        /// Compare each data point and return true if success.
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="tsdbDataPoints"></param>
        /// <returns></returns>
        private static bool CompareDataPoints(Dictionary<string, string> dataPoints, Dictionary<string, string> tsdbDataPoints)
        {
            
                if (dataPoints.Count == tsdbDataPoints.Count)
                {
                    bool success = false;
                    foreach (var timeStamp in dataPoints.Keys)
                    {
                        if (dataPoints[timeStamp] == tsdbDataPoints[timeStamp])
                            success = true;
                        else
                        {
                            success = false;
                            break;
                        }
                    }
                    return success;
                }
               else
                    return false;
       }
        /// <summary>
        /// Extracts data from the time series log files as dictionary collection
        /// </summary>
        /// <param name="filePath"></param>
        /// <remarks>
        /// Metric name is the key
        /// Value is again a key value pair collection with time stamp as a key and data point as value.
        /// </remarks>
        public static Dictionary<string, Dictionary<string, string>> ExtractData(string filePath)
        {
            Dictionary<string, Dictionary<string, string>> metricCollection = new Dictionary<string, Dictionary<string, string>>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string row = reader.ReadLine();
                    //Split each line (row) based on tab as a separator.
                    //Here column 0 = metric name
                    //     column 1 = time stamp
                    //     column 2 = value
                    string[] columns = row.Split(new char[] { '\t' });

                    //Check if metric name is already added as key.
                    if (metricCollection.ContainsKey(columns[0]))
                    {
                        metricCollection[columns[0]].Add(columns[1], columns[2]);
                    }
                    else
                    {
                        metricCollection.Add(columns[0], new Dictionary<string, string>());
                        metricCollection[columns[0]].Add(columns[1], columns[2]);
                    }
                }
            }
            return metricCollection;
        }
    }
}
