using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProvincia.Application.DTOs;
using TestProvincia.Domain.Entities;
using TestProvincia.Domain.Enums;
using TestProvincia.Shared.Constants;

namespace TestProvincia.Application.Validations
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(u => u.Name).NotEmpty().WithMessage(Messages.Error.EmptyField );
            RuleFor(u => u.LastName).NotEmpty().WithMessage(Messages.Error.EmptyField); ;
            RuleFor(u => u.DocumentType).NotEmpty().WithMessage(Messages.Error.EmptyField).Must( x => Enum.IsDefined(typeof(DocumentType), x)).WithMessage(Messages.Error.InvalidDocumentType);
            RuleFor(u => u.DocumentNumber).NotEmpty().WithMessage(Messages.Error.EmptyField).Matches(@"^[0-9-]+$").WithMessage(Messages.Error.NotNumberField);
            RuleFor(u => u.Address).NotEmpty().WithMessage(Messages.Error.EmptyField);
            RuleFor(u => u.Province).NotEmpty().WithMessage(Messages.Error.EmptyField);
            RuleFor(u => u.PhoneNumber).NotEmpty().WithMessage(Messages.Error.EmptyField).Matches(@"^[0-9-]+$").WithMessage(Messages.Error.NotNumberField);
        }
    }
}