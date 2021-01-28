using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoucherCreateApp
{
    class Project
    {
        static int num = 0;
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsValid { get; set; }
        public Project(int id, string name, DateTime startDateTime, DateTime endDateTime ,bool isvalid)
        {
            ID = id;
            Name = name;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            IsValid = isvalid;
        }
    }
}
