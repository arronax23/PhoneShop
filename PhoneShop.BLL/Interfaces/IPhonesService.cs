﻿using PhoneShop.BLL.Messages;

namespace PhoneShop.BLL.Interfaces
{
    public interface IPhonesService
    {
        GetAllPhonesResponse GetAllPhones();
        GetPhoneByIdResponse GetPhoneById(GetPhoneByIdRequest request);
        void SavePhone(SavePhoneRequest request);
        void DeletePhoneById(DeletePhoneByIdRequest request);
        void UpdatePhone(UpdatePhoneRequest request);
        void AddPhoneToShoppingCard(AddPhoneToShoppingCardRequest request);
    }
}