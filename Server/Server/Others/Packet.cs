using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.Models;

namespace Server.Others
{
    public class Packet
    {
        public Device device { get; set; }
        public List<Time> time { get; set; }
    }
}