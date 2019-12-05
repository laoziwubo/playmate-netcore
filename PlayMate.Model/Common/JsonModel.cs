using System;
using System.Collections.Generic;
using System.Text;

namespace PlayMate.Model.Common
{
    public class JsonModel<T> where T : class
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public T Payload { get; set; }
    }

    public class JsonModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
    }
}
