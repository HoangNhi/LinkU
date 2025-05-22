using MODELS.BASE;

namespace MODELS.GROUP.Dtos
{
    public class MODELGroup : MODELBase
    {
        public Guid Id { get; set; }

        public string GroupName { get; set; } = null!;

        /// <summary>
        /// True: public - Cho phép tham gia bằng Link, False: ngược lại
        /// </summary>
        public bool GroupType { get; set; } = true;
    }
}
