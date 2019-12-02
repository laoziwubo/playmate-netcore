using System;
using System.Collections.Generic;
using System.Text;

namespace PlayMate.Model.Record
{
    public class RecordModel
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
    }

    public class RecordDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}
