using AutoMapper;

namespace NodeTrees.Server
{
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.Shared.DTO;

    internal static class AutoMapperConfiguration
    {
        public static void ConfigureMapping(this WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                // --- Domain → DTO ---

                CreateMap<JournalEvent, JournalInfoDto>()
                    .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                    .ForMember(d => d.EventId, m => m.MapFrom(s => s.EventId))
                    .ForMember(d => d.CreatedAt, m => m.MapFrom(s => s.TimestampUtc));

                CreateMap<JournalEvent, JournalDto>()
                    .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                    .ForMember(d => d.EventId, m => m.MapFrom(s => s.EventId))
                    .ForMember(d => d.CreatedAt, m => m.MapFrom(s => s.TimestampUtc))
                    .ForMember(d => d.Text, m => m.MapFrom(s => s.Message ?? string.Empty));

                CreateMap(typeof(Range<>), typeof(RangeDto<>));

                CreateMap<TreeNode, NodeDto>()
                    .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                    .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                    .ForMember(d => d.Children, m => m.MapFrom(s => s.Children));

                // --- DTO → Domain ---

            }
        }
    }
}