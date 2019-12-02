using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlayMate.IRepository;
using SqlSugar;

namespace PlayMate.Repository
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class, new()
    {
        private readonly ISqlSugarClient _db;

        public BaseRepo(ISqlSugarClient db)
        {
            _db = db;
        }

        public Task<T> QueryById(object objId)
        {
            throw new NotImplementedException();
        }
    }
}
