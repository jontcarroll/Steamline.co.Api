using System;
using System.Collections.Generic;

namespace Steamline.co.Api.V1.Models
{
    public class Page<T>
    {
        public int TotalRecords { get; set; }
        public List<T> Records { get; set; }
    }
}