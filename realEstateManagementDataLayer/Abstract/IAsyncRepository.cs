using System;
using Ardalis.Specification;
using realEstateManagementEntities.Models;

namespace realEstateManagementDataLayer.Abstract
{
        public interface IAsyncRepository<T> where T : BaseEntity
        {
            Task<T> GetByIdAsync(int id);

            Task<List<T>> ListAllAsync();

            Task<List<T>> ListAsync(ISpecification<T> spec);

            Task<T> AddAsync(T entity);

            Task UpdateAsync(T entity);

            Task DeleteAsync(T entity);

            Task<int> CountAsync(ISpecification<T> spec);

            Task<T> FirstAsync(ISpecification<T> spec);

            Task<T> FirstOrDefaultAsync(ISpecification<T> spec);

        }   
}

