using Corno.Data.Context;
using System;

namespace Corno.Services.Base.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        BaseContext DbContext { get; }

        void Save();
    }
}