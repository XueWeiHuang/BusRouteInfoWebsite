using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusRouteInfoWebsite.Models
{
    public partial class RouteStop
    {
        public int RouteStopId { get; set; }
        public string BusRouteCode { get; set; }
        public int? BusStopNumber { get; set; }
        [Display(Name ="Offset Minutes")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int? OffsetMinutes { get; set; }

        public BusRoute BusRouteCodeNavigation { get; set; }
        [Display(Name ="Bus Stop Number")]
        public BusStop BusStopNumberNavigation { get; set; }
    }
}
