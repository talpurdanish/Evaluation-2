using Evaluation.Domain.Entities;
using Evaluation.Helpers;
using Evaluation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace Evaluation.Pages
{
    public class BulkModel : PageModel
    {
        private readonly IRazorRenderService _renderService;

        private readonly IBulkService _bulkService;
        private readonly IHostEnvironment _environment;

        public IEnumerable<Vehicle> Vehicles { get; set; } = default!;

        [BindProperty]
        public IFormFile UploadedExcelFile { get; set; } = default!;

        public BulkModel(IBulkService bulkService, IRazorRenderService renderService, IHostEnvironment environment)

        {
            _renderService = renderService;
            _bulkService = bulkService;
            _environment = environment;
        }

        public PageResult OnGet()
        {
            return Page();
        }

        public async Task<JsonResult> OnPostUploadToDatabaseAsync()
        {

            try
            {


                if (await _bulkService.AddToDatabase())
                {
                    RedirectToPage("/Index");
                    return new JsonResult(new { isValid = true, message = "Data been added to database" });
                }

                return new JsonResult(new { isValid = false, message = "Data has not been added to database" });

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

        public async Task<JsonResult> OnPostUploadAsync(IFormFile Upload)
        {

            try
            {

                if (Upload is null || Upload.Length <= 0)
                {

                    return new JsonResult(new { isValid = false, message = "Uploades file is not a valid file" });
                }
                if (Upload.Length > 500000)
                {

                    return new JsonResult(new { isValid = false, message = "File should be less then 0.5 Mb\"" });
                }
                var fileName = DateTime.Now.ToString("dd-mm-yyyy hhmm");
                var fileExtension = Path.GetExtension(Upload.FileName);
                if (!ValidateFileExtension(fileExtension))
                {

                    return new JsonResult(new { isValid = false, message = "Wrong file format. Should be xlsx or xls" });
                }


                fileName += fileExtension;

                var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", fileName);


                using var stream = System.IO.File.Create(filePath);
                await Upload.CopyToAsync(stream);
                stream.Close();
                Vehicles = _bulkService.ReadFile(filePath)!;
                if (Vehicles is null)
                    return new JsonResult(new { isValid = false, message = "Unable to read the file properly" });
                var html = await _renderService.ToStringAsync("_ViewBulk", Vehicles);

                return new JsonResult(new { isValid = filePath != "", html, message = "File has been Uploaded" });

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

        public PartialViewResult OnGetViewAllPartial()
        {

            return new PartialViewResult
            {
                ViewName = "_ViewBulk",
                ViewData = new ViewDataDictionary<IEnumerable<Vehicle>>(ViewData, Vehicles)
            };

        }


        private static bool ValidateFileExtension(string extension)
        {

            return string.Equals(extension, ".xls", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase);

        }

    }
}
