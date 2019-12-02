using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PlayMate.Common.Attribute;
using PlayMate.IRepository;
using PlayMate.IServices;
using PlayMate.Model.Record;

namespace PlayMate.Services
{
    public class RecordServices : BaseServices<RecordModel>, IRecordServices
    {
        private readonly IRecordRepo _dao;
        private readonly IMapper _mapper;
        public RecordServices(IRecordRepo dao, IMapper mapper)
        {
            _dao = dao;
            _mapper = mapper;
            base.BaseDao = dao;
        }

        [Caching]
        public async Task<RecordDto> GetRecordForTest(string a, string b)
        {
            var model = await _dao.GetRecordForTest(a, b);
            var dto = _mapper.Map<RecordDto>(model);
            return dto;
        }
    }
}
