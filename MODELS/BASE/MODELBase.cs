namespace MODELS.BASE
{
    public class MODELBase
    {
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public bool IsActived { get; set; } = true;
        public bool IsEdit { get; set; } = false;
        public int? Sort { get; set; }

        public string DateTime => $"{NgayTao.ToString("HH")}:{NgayTao.ToString("mm")} {NgayTao.ToString("dd")} tháng {NgayTao.ToString("MM")}, {NgayTao.ToString("yyyy")}";
    }
}
