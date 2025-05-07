using MODELS.BASE;

namespace MODELS.OTP.Dtos
{
    public class MODELOTP : MODELBase
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Code { get; set; } = null!;

        public DateTime Expires { get; set; }

    }
}
