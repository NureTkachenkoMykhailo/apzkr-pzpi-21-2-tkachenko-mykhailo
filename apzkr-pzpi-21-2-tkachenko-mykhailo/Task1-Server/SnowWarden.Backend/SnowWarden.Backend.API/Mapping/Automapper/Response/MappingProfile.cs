using AutoMapper;
using SnowWarden.Backend.API.Models.Response.Dtos;
using SnowWarden.Backend.Core.Features.Booking;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Core.Features.Training;

namespace SnowWarden.Backend.API.Mapping.Automapper.Response;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<ApplicationUser, UserResponseDto>();
		CreateMap<Track, TrackResponseDto>();
		CreateMap<Section, SectionResponseDto>()
			.ForMember(
				s => s.Factors,
				opt => opt.MapFrom(x => x.DeterminingFactors as Dictionary<string, SectionFactor>));
		CreateMap<TrainingSession, TrainingSessionResponseDto>()
			.ForMember(s => s.Levels, opt => opt.MapFrom(s => s.Levels.Select(l => new
			{
				Key = l.Level,
				Name = l.Level.ToString()
			})))
			.ForMember(s => s.Information, opt => opt.MapFrom(x => x.GeneralInformation))
			.ForMember(s => s.ReservedItems, opt => opt.MapFrom(x => x.InventoryItemsTook));
		CreateMap<Booking, BookingResponseDto>().ForMember(s => s.Training, opt => opt.MapFrom(x => x.TrainingSession));
		CreateMap<Inventory, InventoryResponseDto>();
		CreateMap<InventoryItem, InventoryItemResponseDto>();
	}
}