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
using EventsIP.Interfaces;
using Org.Apache.Http.Impl.Client;
using EventsIP.Models;
using EventsIP.Services;
using vEvents.Services;
using Xamarin.Facebook.Ads;

namespace EventsIP.Activities
{
    [Activity(Label = "EventsList")]
    public class EventsList : Activity, IEventsListView
    {
        private FacebookService facebookService;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            InitializeLayout();

            // Create your application here
        }

        public void InitializeLayout()
        {
            List<EventListItemModel> events = new List<EventListItemModel>();

            facebookService = new FacebookService(this);

            facebookService.GetEventsForListByIds(EventsAPIService.updatedEvents);
            
            SetContentView(Resource.Layout.EventsList);
        }

        public void UpdateEventsView()
        {
            ViewGroup linearLayout = FindViewById<ViewGroup>(Resource.Id.event_list);

            var events = facebookService.eventsForList;

            foreach (var item in events)
            {
                View tableLayout = LayoutInflater.Inflate(Resource.Layout.EventListItem, linearLayout, false);

                TextView distanceTextView = tableLayout.FindViewById<TextView>(Resource.Id.distance);
                TextView addressTextView = tableLayout.FindViewById<TextView>(Resource.Id.address);
                TextView startTimeTextView = tableLayout.FindViewById<TextView>(Resource.Id.start_time);
                TextView eventNameTextView = tableLayout.FindViewById<TextView>(Resource.Id.event_name);

                distanceTextView.Text = item.Distance.ToString();
                addressTextView.Text = item.Address;
                startTimeTextView.Text = ((DateTime)item.StartingTime).ToLongDateString();
                eventNameTextView.Text = item.EventName;

                linearLayout.AddView(tableLayout);
            }
        }
    }
}