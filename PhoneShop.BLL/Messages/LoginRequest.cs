﻿using System.Security.Claims;

namespace PhoneShop.BLL.Messages
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public ClaimsPrincipal CurrentUser { get; set; }
    }
}