using Evaluation.Domain.Entities;
using Evaluation.Domain.Viewmodels;
using Evaluation.Helpers;
using Evaluation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Evaluation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IRazorRenderService _renderService;
        private readonly IVehicleService _service;

        public VehiclesViewModel Vehicles { get; set; } = default!;
        [BindProperty]
        public VehicleViewModel Vehicle { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Sort { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Direction { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }


        [BindProperty(SupportsGet = true)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? SearchByDate { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public bool? SearchById { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public int? CurrentPage { get; set; } = 1;

        public int Count { get; set; }
        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; } = 10;

        public string? Message { get; set; } = String.Empty;

        public bool ShowMessage => !String.IsNullOrEmpty(Message);


        public IndexModel(IVehicleService service, IRazorRenderService renderService)

        {
            _renderService = renderService;
            //_repository = repository;
            _service = service;
            Vehicle = new VehicleViewModel();

        }

        public async Task<PageResult> OnGet(int id)
        {
            try
            {


                if (id > 0)

                    Vehicle = await _service.GetVehicle(id);

                Vehicles = new VehiclesViewModel();

            }
            catch (AppException e)
            {

                Message = e.Message;
            }
            catch (Exception)
            {

                Message = "Sorry we are unable to process your request";
            }
            return Page();
        }


        public JsonResult OnGetCount()
        {
            try
            {
                int[] count = _service.GetCount();

                return new JsonResult(new { isValid = true, pages = count[0], page = count[1] });
            }
            catch (AppException e)
            {

                return new JsonResult(new { isValid = false, message = e.Message });
            }
            catch (Exception)
            {

                return new JsonResult(new { isValid = false, message = "Sorry we are unable to process your request" });
            }

        }

        public async Task<PageResult> OnGetEdit(int id)
        {

            try
            {
                Vehicle = await _service.GetVehicle(id);

                return Page();
            }
            catch (AppException e)
            {

                Message = e.Message;
            }
            catch (Exception)
            {

                Message = "Sorry we are unable to process your request";
            }
            return Page();

        }


        public async Task<PartialViewResult> OnGetViewAllPartial(string searchString, string sort, string direction, string searchById, string searchBydate, string id, string startdate, string enddate, int currentpage)
        {

            try
            {
                var isValidStartDate = DateTime.TryParse(startdate, out DateTime startDate);

                var isValidEndDate = DateTime.TryParse(enddate, out DateTime endDate);

                var isValidId = int.TryParse(id, out int Id);

                var criteria = new SearchCriteria
                {
                    Term = searchString,
                    StartDate = isValidStartDate ? startDate : DateTime.Now,
                    EndDate = isValidEndDate ? endDate : DateTime.Now,
                    Id = isValidId ? Id : -1,
                    SearchById = searchById == "true",
                    SearchByDate = searchBydate == "true",


                };
                Vehicles = await _service.GetVehicles(criteria, ConvertStringToSortBy(sort), ConvertStringToDirection(direction), currentpage);
                CurrentPage = currentpage;

                return new PartialViewResult
                {
                    ViewName = "_View",
                    ViewData = new ViewDataDictionary<IEnumerable<Vehicle>>(ViewData, Vehicles.Vehicles)
                };
            }
            catch (Exception)
            {
                Vehicles = new VehiclesViewModel
                {
                    Vehicles = new List<Vehicle>()
                };
                return new PartialViewResult
                {
                    ViewName = "_View",
                    ViewData = new ViewDataDictionary<IEnumerable<Vehicle>>(ViewData, Vehicles.Vehicles)
                };
            }

        }


        private static SortBy ConvertStringToSortBy(string columnName)
        {
            SortBy sort = SortBy.VehicleId;
            switch (columnName)
            {
                case "RegNo":
                    sort = SortBy.RegNo;
                    break;

                case "Make":
                    sort = SortBy.Make;
                    break;

                case "Model":
                    sort = SortBy.Model;
                    break;

                case "Color":
                    sort = SortBy.Color;
                    break;

                case "EngineNo":
                    sort = SortBy.EngineNo;
                    break;

                case "ChasisNo":
                    sort = SortBy.ChassisNo;
                    break;

                case "DateOfPurchase":
                    sort = SortBy.DateOfPurchase;
                    break;

                case "Active":
                    sort = SortBy.Active;
                    break;
            }
            return sort;


        }



        private static SortOrder ConvertStringToDirection(string order)
        {
            return order == "asc" ? SortOrder.Ascending : SortOrder.Descending;
        }


        public async Task<JsonResult> OnPostCreateOrEditAsync(VehicleViewModel vehicle)
        {
            try
            {
                var returnValue = false;
                if (ModelState.IsValid)
                {
                    if (vehicle.VehicleId == 0)
                    {
                        returnValue = await _service.CreateVehicle(vehicle);
                    }
                    else
                    {
                        returnValue = await _service.UpdateVehicle(vehicle);
                    }
                    if (returnValue)
                    {
                        var criteria = new SearchCriteria();
                        Vehicles = await _service.GetVehicles(criteria);

                        var html = await _renderService.ToStringAsync("_View", Vehicles.Vehicles);
                        return new JsonResult(new { isValid = returnValue, html });
                    }
                    return new JsonResult(new { isValid = false ,message="Sorry we are unable to process your request, Please try again"});
                }
                else
                {
                    return new JsonResult(new { isValid = false ,message="There are some errors in Form"});
                }
            }
            catch (AppException e)
            {

                return new JsonResult(new { isValid = false, message = e.Message });
            }
            catch (Exception)
            {

                return new JsonResult(new { isValid = false, message = "Sorry we are unable to process your request" });
            }
        }

        public async Task<JsonResult> OnPostDeleteAsync(int id)
        {
            try
            {
                if (id > 0)
                {
                    var returnValue = await _service.DeleteVehicle(id);
                    if (returnValue)
                    {
                        var criteria = new SearchCriteria();
                        Vehicles = await _service.GetVehicles(criteria);

                        var html = await _renderService.ToStringAsync("_View", Vehicles.Vehicles);
                        return new JsonResult(new { isValid = returnValue,  html, message="Record has been deleted" });
                    }
                    return new JsonResult(new { isValid = false , message="Record has not been deleted"});
                }
                else
                {
                    return new JsonResult(new { isValid = false , message="Record has not been deleted"});
                }
            }
            catch (AppException e)
            {

                return new JsonResult(new { isValid = false, message = e.Message });
            }
            catch (Exception)
            {

                return new JsonResult(new { isValid = false, message = "Sorry we are unable to process your request" });
            }
        }
    }
}
