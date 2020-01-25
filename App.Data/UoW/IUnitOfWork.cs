using System;
using NHibernate;

namespace App.Data.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        ISession Session { get; }

        void BeginTransaction();
        
        void CommitTransaction();
        
        void RollbackTransaction();
    }
}