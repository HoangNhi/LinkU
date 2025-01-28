using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
