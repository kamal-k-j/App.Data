using System;
using App.Data.Exceptions;
using NHibernate;
using Otb.Data.Standard.Properties;
using Serilog;

namespace App.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILogger _logger;
        private readonly ITransaction _transaction;
        private bool _disposed;

        public ISession Session { get; }

        public UnitOfWork(ISession session, ILogger logger)
        {
            Session = session;
            _logger = logger;
            _transaction = Session.Transaction;
        }

        public void BeginTransaction()
        {
            _transaction.Begin();
        }

        public void CommitTransaction()
        {
            if (_transaction.WasCommitted || _transaction.WasRolledBack)
            {
                throw new UnitOfWorkCompletedException();
            }

            try
            {
                _transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, Resources.CommitTransactionExceptionMessage);
                RollbackTransaction();
                throw;
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction.WasCommitted || _transaction.WasRolledBack)
            {
                throw new UnitOfWorkCompletedException();
            }

            try
            {
                _transaction.Rollback();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, Resources.RollbackTransactionExceptionMessage);
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (!_transaction.WasCommitted && !_transaction.WasRolledBack)
                {
                    CommitTransaction();
                }

                _transaction.Dispose();

                Session.Close();
                Session.Dispose();
            }

            _disposed = true;
        }
    }
}