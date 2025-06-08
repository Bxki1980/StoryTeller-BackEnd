using FluentValidation;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Validators
{
    public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
    {
        public UpdateBookDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Author).NotEmpty().MaximumLength(50);
            RuleFor(x => x.AgeRange).NotEmpty();
            RuleFor(x => x.CoverImageBlobPath).NotEmpty();

            When(x => x.Pages != null, () =>
            {
                RuleForEach(x => x.Pages!).SetValidator(new CreatePageDtoValidator());
            });
        }
    }
}
