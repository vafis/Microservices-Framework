using Eid.Microservices.MongoDb.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Eid.Microservices.MongoDb.Interface
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities.
        /// </summary>
        /// <param name="where">Optional expression for filtered results.</param>
        /// <returns>Collection of entities with/-out a filter applied.</returns>
        IEnumerable<T> GetAll(Expression<Func<T, bool>> where = null);

        /// <summary>
        /// Retrieves all entities within a paginated result structure.
        /// </summary>
        /// <param name="pageIndex">Starting index of the result set.</param>
        /// <param name="pageSize">Number of entities in a result set.</param>
        /// <param name="where">Optional expression for filtered results.</param>
        /// <returns>Collection of entities with/-out a filter applied, from the starting index with and specified number of entities.</returns>
        PaginatedResult<T> GetAllPaginated(int pageIndex, int pageSize, Expression<Func<T,bool>> where = null);

        /// <summary>
        /// Retrieves a single entity by Id.
        /// </summary>
        /// <param name="id">Id property boxed to an object.</param>
        /// <returns>Single entity or null.</returns>
        T Get(object id);

        /// <summary>
        /// Inserts a new entity with specified set of properties and their values.
        /// </summary>
        /// <param name="entity">Entity to create/insert.</param>
        /// <returns>Boolean if operation successful, entity id property will be set/modified if key configured to be auto-generated.</returns>
        bool Insert(T entity);

        /// <summary>
        /// Upsert tries to update an existing entity (with replace operation) but if the entity is not present, it performs an insert.
        /// </summary>
        /// <param name="entity">Entity to update/insert.</param>
        /// <returns>Boolean representing success of the operation.</returns>
        bool Upsert(T entity);

        /// <summary>
        /// Update an exsisting entity.
        /// If propertiy names are defined, only those properties will be updated, otherwise a replace operation of the whole entity is done.
        /// </summary>
        /// <param name="entity">Entity to be updated.</param>
        /// <param name="propertiesToUpdate">Optional list/array of property names (Case-sensative).</param>
        /// <returns>Boolean if operation successful.</returns>
        bool Update(T entity, params string[] propertiesToUpdate);

        /// <summary>
        /// Delete a single entity by id value. Multiple enitites will be deleted if id value is not unique.
        /// </summary>
        /// <param name="id">Id value</param>
        /// <returns>Boolean if operation successful.</returns>
        bool Delete(object id);

        /// <summary>
        /// Delete a single entity by id property in T. Multiple enitites will be deleted if id property is not unique.
        /// </summary>
        /// <param name="entity">Entity to be deleted (only Id property used).</param>
        /// <returns>Boolean if operation successful.</returns>
        bool Delete(T entity);

        /// <summary>
        /// Get the count of all entities in a repository.
        /// </summary>
        /// <returns>Number of entities.</returns>
        long TotalCount();
    }
}
