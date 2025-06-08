using FluentValidation;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Validators
{
    public class CreatePageDtoValidator : AbstractValidator<CreatePageDto>
    {
        public CreatePageDtoValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("SectionId is required.")
                .MaximumLength(10);

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Page content is required.");

            RuleFor(x => x.ImageBlobPath)
                .NotEmpty().WithMessage("Image blob path is required.");

            RuleFor(x => x.AudioBlobPath)
                .NotEmpty().WithMessage("Audio blob path is required.");
        }
    }
}
