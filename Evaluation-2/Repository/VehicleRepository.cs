using Dapper;
using Evaluation.Domain.Entities;
using Evaluation.Domain.Viewmodels;
using Evaluation.Helpers;
using System.Data.SqlClient;


namespace Evaluation.Repositories
{

    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetVehicles(SearchCriteria criteria,
            SortBy sortBy = SortBy.VehicleId, SortOrder order = SortOrder.Ascending);
        Task<Vehicle> GetVehicle(int id);
        Task<bool> CreateVehicle(Vehicle vehicle);
        Task<bool> UpdateVehicle(Vehicle vehicle);
        Task<bool> DeleteVehicle(int id);



    }
    public class VehicleRepository : IVehicleRepository
    {
        private readonly DataContext _context;


        public VehicleRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateVehicle(Vehicle vehicle)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var sql = """
                INSERT INTO Vehicles ( RegNo, Make, Model, Color, EngineNo, ChasisNo, DateOfPurchase, Active)
                VALUES (@RegNo, @Make, @Model, @Color, @EngineNo, @ChasisNo, @DateOfPurchase, @Active)
            """;
                return (await connection.ExecuteAsync(sql, vehicle)) > 0;
            }
            catch (Exception)
            {
                throw new AppException("Error occurred connecting with database");

            }
        }


        public async Task<bool> UpdateVehicle(Vehicle vehicle)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var sql = """
                UPDATE Vehicles SET RegNo = @RegNo,
                Make = @Make, 
                Model = @Model, 
                Color = @Color,
                EngineNo =  @EngineNo, 
                ChasisNo = @ChasisNo,
                DateOfPurchase = @DateOfPurchase, 
                Active = @Active
                WHERE VehicleID = @VehicleId
                
            """;
                return (await connection.ExecuteAsync(sql, vehicle)) > 0;
            }
            catch (Exception)
            {

                throw new AppException("Error occurred connecting with database");

            }
        }

        public async Task<bool> DeleteVehicle(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var sql = " DELETE FROM Vehicles Where VehicleId = " + id;
                return (await connection.ExecuteAsync(sql)) > 0;
            }
            catch (Exception)
            {

                throw new AppException("Error occurred connecting with database");

            }
        }

        public async Task<IEnumerable<Vehicle>> GetVehicles(SearchCriteria criteria,
            SortBy sortBy = SortBy.VehicleId, SortOrder order = SortOrder.Ascending)
        {
            try
            {


                using var connection = _context.CreateConnection();

                string queryTemplate = "SELECT VehicleId, RegNo, Make, Model, Color, " +
                    "EngineNo, ChasisNo, DateOfPurchase, Active" +
                    " FROM Vehicles {0} {1} {2}";


                var whereString = !String.IsNullOrEmpty(criteria.Term)
                    || criteria.SearchByDate || criteria.SearchByActive || criteria.SearchById ? "Where" : "";

                string query = String.Format(queryTemplate, whereString, GenerateWherePart(criteria), GenerateSortPart(sortBy, order));


                return await connection.QueryAsync<Vehicle>(query);


            }
            catch (Exception)
            {
                throw new AppException("Error occurred connecting with database");
            }

        }

        public async Task<Vehicle> GetVehicle(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new AppException("Invalid Id");
                }
                using var connection = _context.CreateConnection();
                var queryTemplate = "SELECT * FROM Vehicles WHERE VehicleId = {0}";
                var query = String.Format(queryTemplate, id);
                var vehicle = await connection.QuerySingleOrDefaultAsync<Vehicle>(query);


                return vehicle is null ? throw new AppException("Vehicle could not be found") : vehicle;
            }
            catch (Exception)
            {
                throw new AppException("Error occurred connecting with database");

            }
        }


        private static string GenerateWherePart(SearchCriteria searchCriteria)
        {

            var templateLike = "{0} like '%{1}%'";
            var templateBetween = "CAST(DateOfPurchase AS DATETIME) BETWEEN '{0}' AND '{1}'";
            var templateEqual = "{0} = {1}";



            var idWhere = searchCriteria.SearchById && searchCriteria.Id > 0 ? String.Format(templateEqual, "VehicleId", searchCriteria.Id) : "";

            var regNoWhere = !String.IsNullOrEmpty(searchCriteria.Term) ? String.Format(templateLike, "RegNo", searchCriteria.Term) : "";
            var makeWhere = !String.IsNullOrEmpty(searchCriteria.Term) ? String.Format(templateLike, "Make", searchCriteria.Term) : "";
            var modelWhere = !String.IsNullOrEmpty(searchCriteria.Term) ? String.Format(templateLike, "Model", searchCriteria.Term) : "";
            var colorWhere = !String.IsNullOrEmpty(searchCriteria.Term) ? String.Format(templateLike, "Color", searchCriteria.Term) : "";
            var engineNoWhere = !String.IsNullOrEmpty(searchCriteria.Term) ? String.Format(templateLike, "EngineNo", searchCriteria.Term) : "";
            var chassisNoWhere = !String.IsNullOrEmpty(searchCriteria.Term) ? String.Format(templateLike, "ChasisNo", searchCriteria.Term) : "";

            var dateOfPurchaseWhere = searchCriteria.SearchByDate ? String.Format(templateBetween, searchCriteria.StartDate, searchCriteria.EndDate) : "";
            var activeWhere = searchCriteria.SearchByActive ? String.Format(templateEqual, searchCriteria.Active ? "1" : "0") : "";

            return String.Concat(idWhere, GetOrString(!String.IsNullOrEmpty(searchCriteria.Term) && searchCriteria.SearchById),
                regNoWhere, GetOrString(!String.IsNullOrEmpty(searchCriteria.Term)),
                makeWhere, GetOrString(!String.IsNullOrEmpty(searchCriteria.Term)),
                modelWhere, GetOrString(!String.IsNullOrEmpty(searchCriteria.Term)),
                colorWhere, GetOrString(!String.IsNullOrEmpty(searchCriteria.Term)),
                engineNoWhere, GetOrString(!String.IsNullOrEmpty(searchCriteria.Term)),
                chassisNoWhere, GetOrString(!String.IsNullOrEmpty(searchCriteria.Term) && (searchCriteria.SearchByDate || searchCriteria.SearchByActive)),
                dateOfPurchaseWhere, GetOrString(searchCriteria.SearchByActive),
                activeWhere);
        }

        private static string GetOrString(bool include)
        {
            return include ? " OR " : "";
        }

        private static string GenerateSortPart(SortBy sortBy, SortOrder order)
        {
            var templateSort = "Order by {0} {1}";
            var orderStr = order == SortOrder.Ascending ? " asc" : " desc";
            string returnQuery = sortBy switch
            {
                SortBy.VehicleId => String.Format(templateSort, "VehicleId", orderStr),
                SortBy.RegNo => String.Format(templateSort, "RegNo", orderStr),
                SortBy.Make => String.Format(templateSort, "Make", orderStr),
                SortBy.Model => String.Format(templateSort, "Model", orderStr),
                SortBy.Color => String.Format(templateSort, "Color", orderStr),
                SortBy.EngineNo => String.Format(templateSort, "EngineNo", orderStr),
                SortBy.ChassisNo => String.Format(templateSort, "ChasisNo", orderStr),
                SortBy.DateOfPurchase => String.Format(templateSort, "DateOfPurchase", orderStr),
                SortBy.Active => String.Format(templateSort, "Active", orderStr),
                _ => "",
            };
            ;
            return returnQuery;
        }


    }
}
