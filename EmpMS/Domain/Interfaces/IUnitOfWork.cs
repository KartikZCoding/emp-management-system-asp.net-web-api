using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChanges();
        Task Begin();
        Task Commit();
        Task Rollback();
    }
}
