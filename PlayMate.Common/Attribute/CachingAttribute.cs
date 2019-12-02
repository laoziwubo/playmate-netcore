using System;
using System.Collections.Generic;
using System.Text;

namespace PlayMate.Common.Attribute
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CachingAttribute : System.Attribute
    {
        public int AbsoluteExpiration { get; set; } = 30;
    }
}
