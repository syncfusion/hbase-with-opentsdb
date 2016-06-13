using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringDashboard
{
    public class TSDB
    {
        #region Properties
      
        /// <summary>
        /// Time
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Utilization 
        /// </summary>
        public int DataPoint { get; set; }

        #endregion 
    }
}
