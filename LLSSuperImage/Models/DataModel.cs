using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace LLSSuperImage.Model
{
    public enum ImageSizeEnum
    {
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    public class GetEventsResponse
    {
        public Events events { get; set; }
    }

    public class ErrorResponse
    {
        public int error { get; set; }
        public string message { get; set; }
        public object[] links { get; set; }
    }

    public class ImageWithTextAsUrl
    {
        public string text { get; set; }
    }

    public class ImageCompact : ImageWithTextAsUrl
    {
        public string size { get; set; }
    }

    public class Tags
    {
        public object tag { get; set; }
    }

    public class GeoPoint
    {
        public string geolat { get; set; }
        public string geolong { get; set; }
    }

    public class Location
    {
        public GeoPoint geopoint { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string street { get; set; }
        public string postalcode { get; set; }

        public string DisplayValue
        {
            get
            {
                return string.Format("{0}\n{1} {2}", street, postalcode, city);
            }
        }
    }

    public class Venue : IComparable<Venue>
    {
        public string id { get; set; }
        public string name { get; set; }
        public Location location { get; set; }
        public string url { get; set; }
        public string website { get; set; }
        public string phonenumber { get; set; }
        public ImageCompact[] image { get; set; }

        public double DistanceToCurrentUserPosition
        {
            get;
            set;
        }

        public string DistanceToCurrentUserPositionDisplay
        {
            get { return string.Concat(Math.Round(this.DistanceToCurrentUserPosition, 2), "km"); }
        }

        public int CompareTo(Venue other)
        {
            return string.Compare(this.name, other.name, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class Artists
    {
        private List<string> _artistList;

        //Will contain one artist as string '***' or multiple artists as string array '[***,***,***]'
        public Object artist { get; set; }
        public string headliner { get; set; }

        public List<string> ArtistList
        {
            get
            {
                if (ReferenceEquals(_artistList, null))
                {
                    if (artist is JArray)
                        _artistList = ((JArray)artist).ToObject<List<string>>();
                    else
                    {
                        _artistList = new List<string>();
                        _artistList.Add((string)artist);
                    }
                }

                return _artistList;
            }
        }
    }

    public class Events
    {
        public Event[] eventlist { get; set; }
        public EventAttr attr { get; set; }
    }

    public class EventAttr
    {
        public string location { get; set; }
        public string page { get; set; }
        public string perPage { get; set; }
        public string totalPages { get; set; }
        public string total { get; set; }
        public string festivalsonly { get; set; }
        public string tag { get; set; }
    }

    public class Event : IComparable<Event>
    {
        private List<ImageCompact> _imageList;

        public string id { get; set; }
        public string title { get; set; }
        public Artists artists { get; set; }
        public Venue venue { get; set; }
        public string startDate { get; set; }
        public string description { get; set; }
        public ImageCompact[] image { get; set; }
        public string attendance { get; set; }
        public string reviews { get; set; }
        public string tag { get; set; }
        public string url { get; set; }
        public string website { get; set; }
        public string tickets { get; set; }
        public string cancelled { get; set; }
        public Tags tags { get; set; }

        public double DistanceToCurrentUserPosition
        {
            get;
            set;
        }

        public string DistanceToCurrentUserPositionDisplay
        {
            get { return string.Concat(Math.Round(this.DistanceToCurrentUserPosition, 2), "km"); }
        }

        public string Info
        {
            get
            {
                return string.Format("{0} • {1}", this.StartDate.ToString("ddd dd/MM", CultureInfo.InvariantCulture), this.venue.name);
            }
        }

        public DateTime StartDate
        {
            get
            {
                if (!string.IsNullOrEmpty(this.startDate))
                    return DateTime.Parse(this.startDate);

                return DateTime.MinValue;
            }
        }

        public ImageCompact EventImage
        {
            get
            {
                //TODO: What to do with NULL image? Go through other sizes?
                return this.GetEventImage(ImageSizeEnum.Large);
            }
        }

        public ImageCompact GetEventImage(ImageSizeEnum size)
        {
            ImageCompact venueImage = null;

            if (!ReferenceEquals(this.image, null) && this.image.Length > 0)
            {
                if (ReferenceEquals(_imageList, null))
                    _imageList = new List<ImageCompact>(this.image);

                venueImage = (from item in _imageList
                              where item.size.Equals(size.ToString(), StringComparison.OrdinalIgnoreCase)
                              select item).FirstOrDefault();
            }

            return venueImage;
        }

        public int CompareTo(Event other)
        {
            //If the Id's are the same we are talking about the same object!
            if (this.id.Equals(other.id, StringComparison.OrdinalIgnoreCase))
                return 0;

            int result = DateTime.Compare(this.StartDate, other.StartDate);

            //If the events occur on the same DateTime we compare the distance
            if (result == 0)
                result = this.DistanceToCurrentUserPosition.CompareTo(other.DistanceToCurrentUserPosition);

            //If the distance to the event is the same we compare the title
            if (result == 0)
                result = string.Compare(this.title, other.title, StringComparison.OrdinalIgnoreCase);

            return result;
        }
    }
}
