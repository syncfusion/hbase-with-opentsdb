using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MonitoringDashboard.Helper;

namespace MonitoringDashboard
{
    public class ViewModel :INotifyPropertyChanged
    {

        #region Properties
        private string host = string.Empty;
        private DateTime startDate = DateTime.Now;
        private BaseCommand fetchDataCommand;
        private string timeStamp;
        private ObservableCollection<TSDB> cpuUsage;
        private ObservableCollection<TSDB> diskUsage;
        private ObservableCollection<TSDB> memoryUsage;
        private ObservableCollection<TSDB> networkUsage;
        private BackgroundWorker bgWorker;
        private string TimePicker;
        private string[] metricNames = new string[] 
        {
            "cpu.usage", 
            "disk.usage", 
            "memory.usage", 
            "network.usage" 
        };
        
        /// <summary>
        /// Host Name 
        /// </summary>
        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
                NotifyPropertyChanged("Host");
            }
        }

        /// <summary>
        /// Date for data insertioned.
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }

        /// <summary>
        /// Time for data insertioned.
        /// </summary>
        public string TimeStamp
        {
            get
            {
                return timeStamp;
            }
            set
            {
                timeStamp = value;
                NotifyPropertyChanged("TimeStamp");
            }
        }

        /// <summary>
        /// Collection for Cpu Usage Details
        /// </summary>        
        public ObservableCollection<TSDB> CPUUsage 
        {
            get
            {
                return cpuUsage;
            }
            set
            {
                cpuUsage = value;
                NotifyPropertyChanged("CPUUsage");
            }
        }

        /// <summary>
        /// Collection for Disk Usage Details
        /// </summary>       
        public ObservableCollection<TSDB> DiskUsage
        {
            get
            {
                return diskUsage;
            }
            set
            {
                diskUsage = value;
                NotifyPropertyChanged("DiskUsage");
            }
        }

        /// <summary>
        /// Collection for NetworkUsage Details
        /// </summary>        
        public ObservableCollection<TSDB> NetworkUsage
        {
            get
            {
                return networkUsage;
            }
            set
            {
                networkUsage = value;
                NotifyPropertyChanged("NetworkUsage");
            }
        }

        /// <summary>
        /// Collection for memory usage details
        /// </summary>    
        public ObservableCollection<TSDB> MemoryUsage
        {
            get
            {
                return memoryUsage;
            }
            set
            {
                memoryUsage = value;
                NotifyPropertyChanged("MemoryUsage");
            }
        }

        #endregion
        #region Methods
        #region FetchDataCommand
        /// <summary>
        /// Command for fetch data from server.
        /// </summary>
        public BaseCommand FetachDataCommand
        {
            get
            {
                if (fetchDataCommand == null)
                {
                    fetchDataCommand = new BaseCommand(FetchData);
                }
                return fetchDataCommand;                
            }
        }
        #endregion  
        #region constructor
        public ViewModel()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += DoWorker;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
        
        }
        #endregion
        #region Run_worker
        /// <summary>
        /// Binds the data source value into chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Binds the values to Chart
                var TSDBDataPoints = e.Result as Dictionary<string, ObservableCollection<TSDB>>;
                CPUUsage = TSDBDataPoints["chart.cpu.usage"];
                DiskUsage = TSDBDataPoints["chart.disk.usage"];
                MemoryUsage = TSDBDataPoints["chart.memory.usage"];
                NetworkUsage = TSDBDataPoints["chart.network.usage"];
            }
            catch(Exception Ex)
            {
                System.Windows.MessageBox.Show(Ex.Message.ToString());
            }
        }

        #endregion
        #region Do_Worker
        private void DoWorker(object sender, DoWorkEventArgs e)
        {
            
                Dictionary<string, ObservableCollection<TSDB>> TSDBDataPoints = new Dictionary<string, ObservableCollection<TSDB>>();
                
                foreach (var metricName in metricNames)
                {
                    TSDBDataPoints.Add(metricName, new ObservableCollection<TSDB>());
                    
                    //Returns the JSON value for each metrics
                    dynamic jsonValue = GetTSDBData(metricName);
                    if (jsonValue != null)
                    {
                        foreach (var temp in jsonValue)
                        {
                            TSDB tsdb = new TSDB();
                            tsdb.DataPoint = (int)temp.Value;
                            tsdb.DateTime = DateTimeFromUnixTimestampMillis(long.Parse(temp.Name)); 
                            TSDBDataPoints[metricName].Add(tsdb);
                        }
                    }
                }
                e.Result = TSDBDataPoints;
           
        }
        #endregion
        #region TimeConversion
        /// <summary>
        /// It converts the Unix epoch time into LocalTime
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>
        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            //convert the unix time into local date time
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(millis).ToLocalTime();
            return dtDateTime;
        }

        #endregion
        #region GetTSDBData
        /// <summary>
        /// Parse the Json value
        /// </summary>
        /// <param name="metricName"></param>
        /// <returns>JSONValue</returns>
        private dynamic GetTSDBData(string metricName)
        {
            try
            {
                string JsonValue = string.Empty;

                //REST call to fetch the JSON data from TSDB server for a given metric
                string url = "http://" + Host + "/api/query?start=" + TimePicker + "&m=sum:" + metricName;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 10000;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            JsonValue = reader.ReadToEnd();
                        }

                //Parse the JSON Data into dictionary collection
                dynamic jObject = JsonConvert.DeserializeObject(JsonValue);
                JObject obj = jObject[0] as JObject;
                JToken jtoken;
                obj.TryGetValue("dps", out jtoken);
                dynamic jsonValue = JsonConvert.DeserializeObject(jtoken.ToString());

                return jsonValue;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return null;
            }
         
        }
        #endregion  
        #region
        /// <summary>
        /// call the background worker.
        /// </summary>
        /// <param name="param"></param>

        void FetchData(object param)
        {
            try
            {
                DateTime result;
                result = DateTime.Parse(StartDate.ToString());
                TimePicker = result.ToString("yyyy/MM/dd");
                result = DateTime.Parse(TimeStamp.ToString());
                TimePicker = TimePicker + " " + result.ToString("hh:mm:ss");
                //Call background do worker and run worker method
                if (!bgWorker.IsBusy)
                {
                    bgWorker.RunWorkerAsync();
                }
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message);
            }
        }
        #endregion
        #region PropertyEventHanlder
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion 
        #endregion
    }
}
