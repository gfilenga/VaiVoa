﻿using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace VaiVoa.Domain.Models.Validations
{
    public class CreditCardValidator : AbstractValidator<CreditCard>
    {
        public CreditCardValidator()
        {
            RuleFor(cc => cc.Number).
                NotEmpty().WithMessage("Digite um número de cartão")
                .Must(c => c.Substring(0,1) == "4").WithMessage("O número do cartão deve começar com 4 (Visa)")
                .Length(16).WithMessage("{PropertyName} deve ter {PropertyValue} caracteres");

            RuleFor(cc => cc.SecurityCode)
                .LessThan(999).WithMessage("Digite um {PropertyName} válido")
                .GreaterThan(100).WithMessage("Digite um {PropertyName} válido")
                .NotNull().WithMessage("Digite um {PropertyName}");

            RuleFor(cc => cc.ValidThru)
                .NotEmpty().WithMessage("Preencha a data de expiração")
                .GreaterThan(DateTime.Now).WithMessage("{PropertyName} deve ser maior que a data atual");
        }
    }
}
