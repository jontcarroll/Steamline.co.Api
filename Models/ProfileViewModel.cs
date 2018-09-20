using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Steamline.co.Models
{
    public class ProfileViewModel
    {
        public string GroupCode { get; set; }
        public List<Game> Games { get; set; }
    }
}