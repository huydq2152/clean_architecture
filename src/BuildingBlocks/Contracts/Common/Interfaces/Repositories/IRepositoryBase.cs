﻿using System.Linq.Expressions;
using Contracts.Domains;

namespace Contracts.Common.Interfaces.Repositories
{
    public interface IRepositoryBase<T,K> where T : EntityBase<K>
    {
        IQueryable<T> GetAll(bool trackChanges = false);
        IQueryable<T> GetAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
            params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIdAsync(K id);
        Task<T> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties);
        K Create(T entity);
        Task<K> CreateAsync(T entity);
        K CreateAndGetId(T entity);
        Task<K> CreateAndGetIdAsync(T entity);
        IList<K> CreateList(IEnumerable<T> entities);
        Task<IList<K>> CreateListAsync(IEnumerable<T> entities);
        void Update(T entity);
        Task UpdateAsync(T entity);
        void UpdateList(IEnumerable<T> entities);
        Task UpdateListAsync(IEnumerable<T> entities);
        void Delete(T entity);
        Task DeleteAsync(T entity);
        void DeleteList(IEnumerable<T> entities);
        Task DeleteListAsync(IEnumerable<T> entities);
    }
}