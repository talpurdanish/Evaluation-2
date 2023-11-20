using Evaluation.Domain.Entities;

namespace Evaluation.Domain.Viewmodels
{
    public class VehiclesViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }

        public IEnumerable<Vehicle> Vehicles { get; set; } = default!;

    }
}
