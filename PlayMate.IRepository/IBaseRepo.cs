using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PlayMate.IRepository
{
    public interface IBaseRepo<T> where T : class
    {
        Task<T> QueryById(object objId);
    }
}
