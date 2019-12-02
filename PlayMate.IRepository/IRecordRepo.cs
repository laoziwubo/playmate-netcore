using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlayMate.Model.Record;

namespace PlayMate.IRepository
{
    public interface IRecordRepo : IBaseRepo<RecordModel>
    {
        Task<RecordModel> GetRecordForTest(string a, string b);
    }
}
