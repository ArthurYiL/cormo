﻿using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Cormo.Data.EntityFramework.Api;
using Cormo.Injects;
using Cormo.Interceptions;

namespace Cormo.Data.EntityFramework.Impl
{
    [Transactional]
    public class TransactionalInterceptor: IAroundInvokeInterceptor
    {
        [Inject] private IInstance<DbContext> _dbContext; 
        public async Task<object> AroundInvoke(IInvocationContext invocationContext)
        {
            var dbContext = _dbContext.Value;
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var result = await invocationContext.Proceed();
                    dbContext.SaveChanges();
                    transaction.Commit();
                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}