using AutoMapper;
using Evaluation.Domain.Entities;
using Evaluation.Domain.Viewmodels;
using Evaluation.Helpers;
using Evaluation.Repositories;
using System.Data.SqlClient;

namespace Evaluation.Services
{
    public interface IVehicleService
    {
        Task<bool> CreateVehicle(VehicleViewModel vehicle);
        Task<bool> UpdateVehicle(VehicleViewModel vehicle);
        Task<bool> DeleteVehicle(int id);
        Task<VehicleViewModel> GetVehicle(int id);
        int[] GetCount();
        Task<VehiclesViewModel> GetVehicles(SearchCriteria criteria,
            SortBy sortBy = SortBy.VehicleId, SortOrder order = SortOrder.Descending, int page = 1, int pageSize = 10);
    }

    public class VehicleService : IVehicleService
    {

        private readonly IVehicleRepository _repository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;


        public VehicleService(IVehicleRepository repository, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _repository = repository;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }



        public async Task<bool> CreateVehicle(VehicleViewModel vehicle)
        {
            try
            {
                if (vehicle is null)
                    return false;

                var vehicleModel = _mapper.Map<Vehicle>(vehicle);

                if (vehicleModel.IsValid)
                {
                    return false;
                }
                return await _repository.CreateVehicle(vehicleModel);
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }
        }

        public async Task<bool> UpdateVehicle(VehicleViewModel vehicle)
        {
            try
            {
                if (vehicle is null || vehicle.VehicleId <= 0)
                {
                    return false;
                }



                var foundVehicle = await GetVehicle(vehicle.VehicleId);
                if (foundVehicle is null)
                {
                    return false;
                }
                var vehicleModel = _mapper.Map<Vehicle>(vehicle);
                return await _repository.UpdateVehicle(vehicleModel);
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }
        }

        public async Task<bool> DeleteVehicle(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return false;
                }
                var foundVehicle = await GetVehicle(id);
                if (foundVehicle is null)
                {
                    return false;
                }

                return await _repository.DeleteVehicle(id);
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }
        }

        public async Task<VehicleViewModel> GetVehicle(int id)
        {
            try
            {
                if (id < 0)
                {
                    throw new AppException("Vehicle not found");
                }
                var foundVehicle = await _repository.GetVehicle(id);
                return foundVehicle is null ? throw new AppException("Vehicle not found") : _mapper.Map<VehicleViewModel>(foundVehicle);
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }
        }

        public async Task<VehiclesViewModel> GetVehicles(SearchCriteria criteria, SortBy sortBy = SortBy.VehicleId, SortOrder order = SortOrder.Ascending, int page = 1, int pageSize = 10)
        {

            try
            {
                var viewModel = new VehiclesViewModel();
                var vehicles = await _repository.GetVehicles(criteria, sortBy, order);

                var TotalPages = (int)Math.Ceiling(decimal.Divide(vehicles.Count(), pageSize));
                viewModel.CurrentPage = page;
                viewModel.TotalCount = TotalPages;
                viewModel.Vehicles = vehicles.Skip((page - 1) * pageSize).Take(pageSize).ToList();


                SessionHelper.Remove(_contextAccessor.HttpContext!.Session, "Evaluation-Pages");
                SessionHelper.Remove(_contextAccessor.HttpContext!.Session, "Evaluation-CurrentPage");

                SessionHelper.SetInt(_contextAccessor!.HttpContext!.Session, "Evaluation-Pages", TotalPages);
                SessionHelper.SetInt(_contextAccessor!.HttpContext!.Session, "Evaluation-CurrentPage", page);


                return viewModel;
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }

        }
        public int[] GetCount()
        {
            try
            {
                int[] array = {SessionHelper.GetInt(_contextAccessor!.HttpContext!.Session, "Evaluation-Pages") ?? 0,
                SessionHelper.GetInt(_contextAccessor!.HttpContext!.Session, "Evaluation-CurrentPage") ?? 0};
                return array;
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }


        }

    }

}



