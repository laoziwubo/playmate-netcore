using System;
using System.Collections.Generic;
using System.Text;

namespace PlayMate.Model.Common
{
    public class JsonModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public object Payload { get; set; }
    }
}
