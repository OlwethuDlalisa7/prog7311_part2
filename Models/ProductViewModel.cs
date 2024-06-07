using Agricultural_Web_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agricultural_Web_Application.Models
{
    public class ProductViewModel
    {
        public int PId { get; set; }
        public string Pimage { get; set; }
        public string PName { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
        public string Production_date { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public int UId { get; set; }
    }
}