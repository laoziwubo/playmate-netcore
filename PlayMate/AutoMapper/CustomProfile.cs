using AutoMapper;
using PlayMate.Model.Record;

namespace PlayMate.AutoMapper
{
    public class CustomProfile : Profile
    {
        public CustomProfile()
        {
            CreateMap<RecordModel, RecordDto>();
            //CreateMap<RecordModel, RecordDto>().ForMember(e => e.Content, o => o.MapFrom(s => s.Author));
            CreateMap<RecordDto, RecordModel>();
        }
    }
}
