using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.BASE
{
    public class GetByIdRequest
    {
        public Guid? Id { get; set; }
    }

    public class GetByIdRequestValidator : AbstractValidator<GetByIdRequest>
    {
        public GetByIdRequestValidator()
        {
            RuleFor(r => r.Id).NotNull().WithMessage("Mã không được rỗng");
        }
    }
}
