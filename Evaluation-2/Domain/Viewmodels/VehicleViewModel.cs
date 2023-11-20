using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Evaluation.Domain.Viewmodels
{
    [BindProperties]
    public class VehicleViewModel
    {
        [Key]
        public int VehicleId { get; set; }
        [Required]
        [DisplayName("Registration No")]
        [StringLength(50, ErrorMessage = "Registration No cannot be longer than 50")]
        public string RegNo { get; set; } = String.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Make cannot be longer than 50")]
        public string Make { get; set; } = String.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Model cannot be longer than 50")]
        public string Model { get; set; } = String.Empty;

        [StringLength(50, ErrorMessage = "Color cannot be longer than 50")]
        public string Color { get; set; } = String.Empty;
        [DisplayName("Engine No")]
        [Required]
        [StringLength(50, ErrorMessage = "Engine No cannot be longer than 50")]
        public string EngineNo { get; set; } = String.Empty;
        [DisplayName("Chassis No")]
        [Required]
        [StringLength(50, ErrorMessage = "Chasis No cannot be longer than 50")]
        public string ChasisNo { get; set; } = String.Empty;
        [DisplayName("Purchase Date")]
        [DataType(DataType.Text)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfPurchase { get; set; } = DateTime.Now;


        public bool Active { get; set; }
    }
}
