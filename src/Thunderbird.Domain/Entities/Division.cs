namespace Thunderbird.Domain.Entities {
    public class Division {
        public byte DivisionId { get; set; }
        public byte ProvinceId { get; set; }
        public string DivisionName { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
