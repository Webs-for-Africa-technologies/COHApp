﻿using BataCMS.Data.Interfaces;
using BataCMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BataCMS.Data.Repositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly AppDbContext _appDbContext;

        public PaymentMethodRepository(AppDbContext appDbContext )
        {
            _appDbContext = appDbContext;
        }

        public void CreatePaymentMethod(PaymentMethod paymentMethod, Transaction purchase)
        {
            _appDbContext.AddAsync(paymentMethod);
            _appDbContext.SaveChanges();
        }

        public PaymentMethod GetMethodByPurchaseId(int purchaseId)
        {
            throw new NotImplementedException();
        }

        public PaymentMethod GetPaymentMethodById(int paymentMethodId)
        {
           return _appDbContext.PaymentMethods.FirstOrDefault(p => p.PaymentMethodId == paymentMethodId);
        }

        public async Task UpdatePaymentMethod(PaymentMethod paymentMethod)
        {
            _appDbContext.PaymentMethods.Update(paymentMethod);
            await _appDbContext.SaveChangesAsync();
        }

    }
}
