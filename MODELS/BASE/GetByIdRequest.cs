using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MODELS.BASE
{
    public class GetByIdRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Id không được để trống")]
        public Guid? Id { get; set; }
    }

    public class GetByIdRequestValidator : AbstractValidator<GetByIdRequest>
    {
        public GetByIdRequestValidator()
        {
            RuleFor(r => r.Id).NotNull().WithMessage("Mã không được để trống");
        }
    }
}
