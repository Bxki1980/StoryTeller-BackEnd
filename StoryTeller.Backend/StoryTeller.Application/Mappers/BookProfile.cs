using AutoMapper;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Books;
using StoryTeller.StoryTeller.Backend.StoryTeller.Domain.Entities;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.Mappers
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageBlobPath))
                .ReverseMap()
                .ForMember(dest => dest.CoverImageBlobPath, opt => opt.MapFrom(src => src.CoverImageUrl));

            CreateMap<Page, PageDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageBlobPath))
                .ForMember(dest => dest.AudioUrl, opt => opt.MapFrom(src => src.AudioBlobPath))
                .ReverseMap()
                .ForMember(dest => dest.ImageBlobPath, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.AudioBlobPath, opt => opt.MapFrom(src => src.AudioUrl));

            CreateMap<CreateBookDto, Book>(); // no need to ignore Pages if it doesn't exist

            CreateMap<CreatePageDto, Page>();

            CreateMap<UpdateBookDto, Book>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Book, BookDetailDto>();
            CreateMap<Book, BookCoverDto>();

        }
    }
}
