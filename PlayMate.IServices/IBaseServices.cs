using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PlayMate.IServices
{
    public interface IBaseServices<T> where T : class
    {
        Task<T> QueryById(object objId);
    }
}
