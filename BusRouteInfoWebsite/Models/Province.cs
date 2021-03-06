﻿using System;
using System.Collections.Generic;

namespace BusRouteInfoWebsite.Models
{
    public partial class Province
    {
        public Province()
        {
            Driver = new HashSet<Driver>();
        }

        public string ProvinceCode { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string TaxCode { get; set; }
        public double TaxRate { get; set; }
        public string Capital { get; set; }

        public Country CountryCodeNavigation { get; set; }
        public ICollection<Driver> Driver { get; set; }
    }
}
