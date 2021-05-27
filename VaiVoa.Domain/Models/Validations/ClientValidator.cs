using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace VaiVoa.Domain.Models.Validations
{
    public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator()
        {
            RuleFor(client => client.Name)
               .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
               .Length(2, 40).WithMessage("Digite um {PropertyName} com no mínimo 2 e no máximo 40 caracteres");

            RuleFor(client => client.Email)
                .NotEmpty().WithMessage("Digite um {PropertyName}")
                .EmailAddress().WithMessage("Digite um {PropertyName} válido");

            RuleFor(client => client.Password)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(client => client.ConfirmPassword)
                .Equal(client => client.Password).WithMessage("{PropertyName} e {ComparisonValue} não batem");
        }
    }
}
