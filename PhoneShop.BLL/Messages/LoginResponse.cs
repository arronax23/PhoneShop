﻿namespace PhoneShop.BLL.Messages
{
    public class LoginResponse
    {
        public bool IsSuccesfull { get; set; }
        public string CurrentUserRole { get; set; }
        public string Token { get; set; }
    }
}