using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlayMate.IRepository;
using PlayMate.Model.Record;
using SqlSugar;

namespace PlayMate.Repository
{
    public class RecordRepo : BaseRepo<RecordModel>, IRecordRepo
    {
        public RecordRepo(ISqlSugarClient db) : base(db)
        {
        }

        public Task<RecordModel> GetRecordForTest(string a, string b)
        {
            return Task.Run(() => new RecordModel
            {
                Author = "laoziwubo",
                Content = a + b,
                Id = 10086
            });
        }
    }
}
