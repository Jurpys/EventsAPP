using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using EventsIP.Interfaces;
using EventsIP.Models;
using Newtonsoft.Json.Linq;
using Xamarin.Facebook;

namespace vEvents.Services
{
    public class FacebookService
    {
        private IEventsListView _view;

        public FacebookService(IEventsListView view)
        {
            _view = view;
        }

        public List<EventListItemModel> eventsForList = new List<EventListItemModel>();

        public void GetEventsForListByIds(List<EventReceiveFromAPIModel> eventsFromApi)
        {
            var events = new List<EventListItemModel>();

            var eventIds = eventsFromApi.Where(o => o.EndingDateTime.HasValue);

            GraphBatchCallback graphBatchCallBack = new GraphBatchCallback();
            graphBatchCallBack.RequestCompleted += OnGetEventsForListResponse;

            GraphCallback graphCallback = new GraphCallback();
            graphCallback.RequestCompleted += OnGetEventForListResponse;

            var batch = new GraphRequestBatch();
            batch.AddCallback(graphBatchCallBack);

            Bundle parameters = new Bundle();
            parameters.PutString("fields", "id,name,start_time,place");

            foreach (var eventId in eventIds)
            {
                batch.Add(new GraphRequest(AccessToken.CurrentAccessToken, "/" + eventId.FacebookId, parameters, HttpMethod.Get, graphCallback));           
            }
            batch.Add(new GraphRequest(AccessToken.CurrentAccessToken, "/1473351269639027", parameters, HttpMethod.Get, graphCallback));

            batch.ExecuteAsync();
        }

        public void OnGetEventsForListResponse(object response, GraphBatchResponseEventArgs b)
        {
            _view.UpdateEventsView();
        }

        public void OnGetEventForListResponse(object response, GraphResponseEventArgs b)
        {
            dynamic data = JObject.Parse(b.Response.RawResponse);

            eventsForList.Add(new EventListItemModel()
            {
                Address = (string)((JObject)((JObject)data["place"])["location"])["street"],
                EventName = (string)data["name"],
                FacebookId = (string)data["id"],
                StartingTime = (DateTime)data["start_time"]
            });
        }
    }




    public class GraphCallback : Java.Lang.Object, GraphRequest.ICallback
    {
        // Event to pass the response when it's completed
        public event EventHandler<GraphResponseEventArgs> RequestCompleted = delegate { };

        public void OnCompleted(GraphResponse response)
        {
            this.RequestCompleted(this, new GraphResponseEventArgs(response));
        }
    }

    public class GraphResponseEventArgs : EventArgs
    {
        GraphResponse _response;
        public GraphResponseEventArgs(GraphResponse response)
        {
            _response = response;
        }

        public GraphResponse Response { get { return _response; } }
    }

    public class GraphBatchCallback : Java.Lang.Object, GraphRequestBatch.ICallback
    {
        // Event to pass the response when it's completed
        public event EventHandler<GraphBatchResponseEventArgs> RequestCompleted = delegate { };

        public void OnBatchCompleted(GraphRequestBatch response)
        {
            this.RequestCompleted(this, new GraphBatchResponseEventArgs(response));
        }
    }

    public class GraphBatchResponseEventArgs : EventArgs
    {
        GraphRequestBatch _response;
        public GraphBatchResponseEventArgs(GraphRequestBatch response)
        {
            _response = response;
        }

        public GraphRequestBatch Response { get { return _response; } }
    }
}