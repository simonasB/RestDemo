using FluentValidation;

namespace DemoRest20232.Data.Dtos;


public record CreateTopicDto(string Name, string Description);

public record UpdateTopicDto(string Description);

public class CreateTopicDtoValidator : AbstractValidator<CreateTopicDto>
{
    public CreateTopicDtoValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
        RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}

public class UpdateTopicDtoValidator : AbstractValidator<UpdateTopicDto>
{
    public UpdateTopicDtoValidator()
    {
        RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}