using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlayMate.IRepository;
using PlayMate.IServices;

namespace PlayMate.Services
{
    public class BaseServices<T> : IBaseServices<T> where T : class, new()
    {
        public IBaseRepo<T> BaseDao;

        public async Task<T> QueryById(object objId)
        {
            return await BaseDao.QueryById(objId);
        }
    }
}
