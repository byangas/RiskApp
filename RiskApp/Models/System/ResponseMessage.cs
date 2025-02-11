using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Models.System
{
    public class ResponseMessage
    {
        public static ResponseMessage Create(string message)
        {
            return new ResponseMessage() { Display = message };
        }
        public string Display { get; set; }
        public string Debug { get; set; }
    }
}
