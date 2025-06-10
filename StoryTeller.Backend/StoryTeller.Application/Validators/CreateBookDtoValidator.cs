using FluentValidation;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Validators
{
    public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
    {
        public CreateBookDtoValidator() 
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("BookId is required.")
                .MaximumLength(10);

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100);

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(50);

            RuleFor(x => x.AgeRange)
                .NotEmpty().WithMessage("Age range is required.");

            RuleFor(x => x.CoverImageBlobPath)
                .NotEmpty().WithMessage("Cover image path is required.");

        }
    }
}
