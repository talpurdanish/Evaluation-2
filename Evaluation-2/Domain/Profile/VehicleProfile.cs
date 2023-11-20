using AutoMapper;
using Evaluation.Domain.Entities;
using Evaluation.Domain.Viewmodels;

namespace Evaluation.Profiles
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile()
        {
            CreateMap<VehicleViewModel, Vehicle>();
            CreateMap<Vehicle, VehicleViewModel>();
        }
    }
}
