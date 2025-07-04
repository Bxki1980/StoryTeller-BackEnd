﻿using System.ComponentModel.DataAnnotations;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Application.DTOs.Auth
{

    public class UserSignupDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }

    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }


    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

    }

    public class RefreshRequestDto
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}