using System.ComponentModel.DataAnnotations;

namespace Evaluation.Domain.Entities
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }
        [Required]
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

        [Required]
        [StringLength(50, ErrorMessage = "Engine No cannot be longer than 50")]
        public string EngineNo { get; set; } = String.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Chasis No cannot be longer than 50")]
        public string ChasisNo { get; set; } = String.Empty;
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required]
        public DateTime DateOfPurchase { get; set; }


        public bool Active { get; set; }
        public bool IsValid => String.IsNullOrEmpty(RegNo) ||
    String.IsNullOrEmpty(Model) ||
    String.IsNullOrEmpty(Make) ||
    String.IsNullOrEmpty(Color) ||
    String.IsNullOrEmpty(EngineNo) ||
    String.IsNullOrEmpty(ChasisNo);
    }
}
