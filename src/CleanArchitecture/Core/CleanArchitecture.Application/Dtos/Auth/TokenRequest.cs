﻿namespace CleanArchitecture.Application.Dtos.Auth
{
    public class TokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}