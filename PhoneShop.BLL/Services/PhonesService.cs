﻿using PhoneShop.BLL.Interfaces;
using PhoneShop.BLL.Messages;
using PhoneShop.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneShop.BLL.Services
{
    public class PhonesService : IPhonesService
    {
        private ApplicationDbContext applicationDbContext;
        public PhonesService(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public GetAllPhonesResponse GetAllPhones()
        {
            var response = new GetAllPhonesResponse() { Phones = applicationDbContext.Phones.AsEnumerable() };
            return response;
        }
        public GetPhoneByIdResponse GetPhoneById(GetPhoneByIdRequest request)
        {
            var phone = applicationDbContext.Phones.FirstOrDefault(p => p.PhoneId == request.PhoneId);
            if (phone == null)
                throw new Exception("No phone was found.");
            var response = new GetPhoneByIdResponse() { Phone = phone };
            return response;
        }
    }
}
