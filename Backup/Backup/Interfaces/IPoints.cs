using System;
using System.Collections.Generic;

namespace Backup
{
    public interface IPoints
    {
        public int IndexOfDeltas { get; set; } 
        public DateTime _date { get; set; }
        public long _size { get; set; }
        public string _path { get; set; }

        public List<string> files
        {
            get;
            set;
        }
    }
}