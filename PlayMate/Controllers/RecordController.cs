using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayMate.IServices;
using PlayMate.Model.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayMate.Model.Record;

namespace PlayMate.Controllers
{
    /// <summary>
    /// 记录管理
    /// </summary>
    [Route("api/r")]
    [ApiController]
    [Produces("application/json")]
    public class RecordController : ControllerBase
    {
        private readonly IRecordServices _service;

        public RecordController(IRecordServices service)
        {
            _service = service;
        }

        /// <summary>
        /// 仓储取值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRecordForTest")]
        [Authorize(Roles = "Admin")]
        public async Task<object> Get(string a, string b)
        {
            return Ok(new JsonModel<RecordDto>
            {
                Code = 1,
                Msg = "yes",
                Payload = await _service.GetRecordForTest(a, b)
            });
        }

        /// <summary>
        /// 获取列表，需要Admin权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}", Name = "Get")]
        [Authorize]
        public string Get(int id)
        {
            return "value";
        }
    }
}
