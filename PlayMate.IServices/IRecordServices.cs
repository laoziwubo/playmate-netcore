using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlayMate.Model.Record;

namespace PlayMate.IServices
{
    public interface IRecordServices : IBaseServices<RecordModel>
    {
        Task<RecordDto> GetRecordForTest(string a, string b);
    }
}
