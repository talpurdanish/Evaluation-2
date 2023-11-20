namespace Evaluation.Domain.Viewmodels
{
    public class SearchCriteria
    {

        public SearchCriteria() { }

        public string Term { get; set; } = string.Empty;

        public bool SearchByDate { get; set; }
        public bool SearchByActive { get; set; }
        public bool SearchById { get; set; }

        public bool Active { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int Id { get; set; }



    }
}
