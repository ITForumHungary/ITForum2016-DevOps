namespace Mshudx.Vote.Results.Models
{
    public class Result
    {
        public long Likes { get; set; }
        public long Dislikes { get; set; }
        public long Total { get; set; }
        public long UniqueTotal { get; set; }
        public decimal UniqueDislikes { get; set; }
        public decimal UniqueLikes { get; set; }
    }
}