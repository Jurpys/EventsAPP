using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace EventsIP.Models
{
    public class EventReceiveModel
    {
        public string id { get; set; }
        public DateTime? end_time { get; set; }
        public DateTime? start_time { get; set; }
    }
}