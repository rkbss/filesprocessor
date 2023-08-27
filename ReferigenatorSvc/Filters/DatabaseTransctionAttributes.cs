﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ReferigenatorSvc.dbcontext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReferigenatorSvc.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionRequiredAttribute : Attribute, IAsyncActionFilter
    {
        private readonly AppDbContext _dbContext;
        public TransactionRequiredAttribute(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context is null || next is null)
                throw new ArgumentNullException();
            IExecutionStrategy executionStrategy = _dbContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                var _tran = await _dbContext.Database.BeginTransactionAsync();
                var resut = await next();
                if (resut.Exception is null)
                    await _tran.CommitAsync();
                else
                    await _tran.RollbackAsync();
            });
        }
       
    }
}
