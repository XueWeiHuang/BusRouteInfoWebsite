﻿using System;
using System.Collections.Generic;

namespace BusRouteInfoWebsite.Models
{
    public partial class Bus
    {
        public Bus()
        {
            Trip = new HashSet<Trip>();
        }

        public int BusId { get; set; }
        public int BusNumber { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }

        public ICollection<Trip> Trip { get; set; }
    }
}
