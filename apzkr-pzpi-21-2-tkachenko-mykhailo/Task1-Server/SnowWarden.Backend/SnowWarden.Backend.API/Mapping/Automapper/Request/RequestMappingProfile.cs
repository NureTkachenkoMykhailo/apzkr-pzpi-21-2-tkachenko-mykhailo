using AutoMapper;

using SnowWarden.Backend.API.Models.Requests.Dtos;

using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Core.Features.Training;

namespace SnowWarden.Backend.API.Mapping.Automapper.Request;

public class RequestMappingProfile : Profile
{
	public RequestMappingProfile()
	{
		CreateMap<TrainingSessionPostDto, TrainingSession>()
			.ForMember(s => s.GeneralInformation, opt => opt.MapFrom(ts => ts.Information));
		CreateMap<InventoryPostDto, Inventory>();
		CreateMap<InventoryItemPostDto, InventoryItem>();
		CreateMap<TrackPostDto, Track>();
		CreateMap<SectionPostDto, Section>()
			.ForMember(s => s.DeterminingFactors, opt => opt.MapFrom<FactorResolver>());
	}

	private class FactorResolver : IValueResolver<SectionPostDto, Section, SectionFactors>
	{
		public SectionFactors Resolve(SectionPostDto source, Section destination, SectionFactors destMember, ResolutionContext context)
		{
			destMember = new SectionFactors
			{
				{
					FactorKeys.WIND,
					new SectionFactor
					{
						Key = FactorKeys.WIND,
						MultiplicationValue = source.Factors.Wind
					}
				},
				{
					FactorKeys.SNOW,
					new SectionFactor
					{
						Key = FactorKeys.SNOW,
						MultiplicationValue = source.Factors.Snow
					}
				},
				{
					FactorKeys.ICINESS,
					new SectionFactor
					{
						Key = FactorKeys.ICINESS,
						MultiplicationValue = source.Factors.Iciness
					}
				},
			};

			return destMember;
		}
	}
}