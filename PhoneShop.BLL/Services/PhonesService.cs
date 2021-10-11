﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PhoneShop.BLL.Interfaces;
using PhoneShop.BLL.Messages;
using PhoneShop.DAL.Data;
using PhoneShop.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneShop.BLL.Services
{
    public class PhonesService : IPhonesService
    {
        private ApplicationDbContext _applicationDbContext;
        private IConfiguration _configuration;

        private int numberOfPhonesPerPage;

        public PhonesService(ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            numberOfPhonesPerPage = int.Parse(_configuration["Config:NumberOfPhonesPerPage"]);
        }

        public GetAllPhonesResponse GetAllPhones()
        {
            var response = new GetAllPhonesResponse() { Phones = _applicationDbContext.Phones.AsEnumerable() };
            return response;
        }

        public GetPhonesForOnePageResponse GetPhonesForOnePage(GetPhonesForOnePageRequest request)
        {
            var response = new GetPhonesForOnePageResponse() 
            { 
                Phones = _applicationDbContext.Phones
                .OrderBy(p => p.Brand)
                .Skip(numberOfPhonesPerPage * (request.PageNumber-1))
                .Take(numberOfPhonesPerPage)
                .AsEnumerable() 
            };
            return response;
        }

        public GetNumberOfPagesInPhoneListResponse GetNumberOfPagesInPhoneList()
        {
            double numberOfAllPhones = _applicationDbContext.Phones.Count();   
            double numberOfPages = Math.Ceiling(numberOfAllPhones / numberOfPhonesPerPage);
            int numberOfPagesInt = (int)numberOfPages;

            var response = new GetNumberOfPagesInPhoneListResponse()
            {
                NumberOfPages = numberOfPagesInt
            };

            return response;
        }

        public GetPhoneByIdResponse GetPhoneById(GetPhoneByIdRequest request)
        {
            var phone = _applicationDbContext.Phones.SingleOrDefault(p => p.PhoneId == request.PhoneId);
            if (phone == null)
                throw new Exception("No phone was found.");
            var response = new GetPhoneByIdResponse() { Phone = phone };
            return response;
        }
        public void SavePhone(SavePhoneRequest request)
        {
            _applicationDbContext.Phones.Add(request.Phone);
            _applicationDbContext.SaveChanges();
        }

        public void DeletePhoneById(DeletePhoneByIdRequest request)
        {
            var phoneToBeRemoved = _applicationDbContext.Phones.SingleOrDefault(p => p.PhoneId == request.PhoneId);
            if (phoneToBeRemoved == null)
                throw new Exception("No phone was found.");

            _applicationDbContext.Phones.Remove(phoneToBeRemoved);
            _applicationDbContext.SaveChanges();
        }

        public void UpdatePhone(UpdatePhoneRequest request)
        {
            _applicationDbContext.Phones.Update(request.Phone);
            _applicationDbContext.SaveChanges();
   
        }

        public void AddPhoneToShoppingCart(AddPhoneToShoppingCardRequest request)
        {
            var customer = _applicationDbContext.Customers.SingleOrDefault(c => c.CustomerId == request.CustomerId);
            if (customer == null)
                throw new Exception("No customer was found.");
            var phone = _applicationDbContext.Phones.SingleOrDefault(p => p.PhoneId == request.PhoneId);
            if (phone == null)
                throw new Exception("No phone was found.");

            var currentOrder = _applicationDbContext.Orders
                .Include(o => o.PhoneOrder)
                .Where(o => o.Status == OrderStatus.Open && o.CustomerId == request.CustomerId) //only 1 order can be open
                .SingleOrDefault();

            if (currentOrder == null)
            {
                var phoneOrder = new PhoneOrder() { Phone = phone };
                currentOrder = new Order()
                {
                    Customer = customer,
                    PhoneOrder = new List<PhoneOrder>() { phoneOrder },
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                var orderStatusWorkflow = new OrderStatusWorkflow()
                {
                    Order = currentOrder,
                    Status = OrderStatus.Open,
                    WorkflowDate = DateTime.Now
                };

                _applicationDbContext.Orders.Add(currentOrder);
                _applicationDbContext.OrderStatusWorkflows.Add(orderStatusWorkflow);
            }
            else
            {
                var phoneOrder = new PhoneOrder() { Phone = phone };
                currentOrder.PhoneOrder.Add(phoneOrder);
                currentOrder.ModifiedDate = DateTime.Now;
            }

            _applicationDbContext.SaveChanges();
        }
        public IsPhoneInShoppingCartResponse IsPhoneInShoppingCart(IsPhoneInShoppingCartRequest request)
        {
            var resposne = new IsPhoneInShoppingCartResponse();

            var currentOrder = _applicationDbContext.Orders
                .Include(o => o.PhoneOrder)
                .Where(o => o.Status == OrderStatus.Open && o.CustomerId == request.CustomerId) //only 1 order can be open so I don't sort descending by Created Date
                .SingleOrDefault();

            if (currentOrder == null)
            {
                resposne.IsPhoneInShoppingCart = false;    
            }
            else
            {
                var phoneOrder = _applicationDbContext.PhoneOrders
                    .SingleOrDefault(po => po.OrderId == currentOrder.OrderId && po.PhoneId == request.PhoneId);

                if (phoneOrder == null)
                    resposne.IsPhoneInShoppingCart = false;
                else
                    resposne.IsPhoneInShoppingCart = true;
            }

            return resposne;
        }

        public void RemovePhoneFromShoppingCart(RemovePhoneFromShoppingCartRequest request)
        {
            var currentOrder = _applicationDbContext.Orders
            .Include(o => o.PhoneOrder)
            .Where(o => o.Status == OrderStatus.Open && o.CustomerId == request.CustomerId) //only 1 order can be open
            .SingleOrDefault();

            if (currentOrder == null)
                throw new Exception("No order was found");

            var phoneOrder = currentOrder.PhoneOrder.SingleOrDefault(po => po.PhoneId == request.PhoneId);

            if (phoneOrder == null)
                throw new Exception("No phone was found in open order");

            _applicationDbContext.PhoneOrders.Remove(phoneOrder);
            _applicationDbContext.SaveChanges();
        }

        public GetPhonesInOrderResponse GetPhonesInOrder(GetPhonesInOrderRequest request)
        {
            var getPhonesInOrderResponse = new GetPhonesInOrderResponse();

            var phones = _applicationDbContext.PhoneOrders
                .Include(po => po.Phone)
                .Where(po => po.OrderId == request.OrderId)
                .Select(po => new Phone() 
                {
                    PhoneId = po.Phone.PhoneId,
                    Brand = po.Phone.Brand,
                    Model = po.Phone.Model,
                    RAM = po.Phone.RAM,
                    Camera = po.Phone.Camera,
                    Memory = po.Phone.Memory,
                    Color = po.Phone.Color,
                    ImageUrl = po.Phone.ImageUrl,
                    OS = po.Phone.OS,
                    Price = po.Phone.Price
                })
                .AsEnumerable();

            getPhonesInOrderResponse.Phones = phones;

            return getPhonesInOrderResponse;

        }

    }
}
