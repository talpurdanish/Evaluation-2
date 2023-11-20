using Evaluation.Domain.Entities;
using Evaluation.Helpers;
using Evaluation.Repositories;
using ExcelDataReader;
using System.Data;

namespace Evaluation.Services
{

    public interface IBulkService
    {
        IEnumerable<Vehicle>? ReadFile(string filePath);
        Task<bool> AddToDatabase();

    }
    public class BulkService : IBulkService
    {
        private readonly IVehicleRepository _repository;
        private readonly IHttpContextAccessor _contextAccessor;

        public BulkService(IVehicleRepository repository, IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _contextAccessor = contextAccessor;

        }

        //private IEnumerable<Vehicle> UploadedVehicles = default!;

        public IEnumerable<Vehicle>? ReadFile(string filePath)
        {

            try
            {
                if (File.Exists(filePath))
                {
                    using FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(fs);
                    DataSet result = reader.AsDataSet();
                    var excelDataTable = result.Tables[0];

                    var sessionData = new SessionData();

                    var vehicles = ConvertToVehicles(excelDataTable);

                    sessionData.Vehicles = vehicles;

                    SessionHelper.SetObjectAsJson(_contextAccessor!.HttpContext!.Session, "Evaluation", sessionData);

                    return vehicles;
                }
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }
            return null;



        }

        public async Task<bool> AddToDatabase()
        {

            try
            {
                var vehicles = SessionHelper.GetObjectFromJson<SessionData>(_contextAccessor!.HttpContext!.Session, "Evaluation")!.Vehicles;

                if (vehicles != null)
                {

                    var uploadedFiles = 0;
                    var count = vehicles!.Count();
                    if (count > 0)
                    {
                        foreach (var vehicle in vehicles)
                        {
                            if (await _repository.CreateVehicle(vehicle))
                            {
                                uploadedFiles++;
                            }
                        }

                    }
                    return count > 0 && uploadedFiles == count;
                }
            }

            catch (Exception e)
            {

                throw new AppException(e.Message);
            }

            return false;


        }

        private static IEnumerable<Vehicle> ConvertToVehicles(DataTable dataTable)
        {
            try
            {
                return dataTable.AsEnumerable().Select(row => new Vehicle
                {
                    RegNo = row[0].ToString() ?? "",
                    Model = row[1].ToString() ?? "",
                    Make = row[2].ToString() ?? "",
                    Color = row[3].ToString() ?? "",
                    EngineNo = row[4].ToString() ?? "",
                    ChasisNo = row[5].ToString() ?? "",
                    DateOfPurchase = ConvertToDateTime(row[6].ToString()),
                    Active = ((row[7].ToString() ?? "0") == "1"),

                });
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }
        }

        private static DateTime ConvertToDateTime(string? data)
        {


            if (DateTime.TryParse(data, out DateTime dt))
            {
                return dt;
            }
            else
            {
                return DateTime.Now;
            }


        }
    }
}
