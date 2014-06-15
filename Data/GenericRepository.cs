using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Data
{
    public interface IGenericRepository<TEntity> where TEntity : class 
    {
        TEntity GetByID(object id);
        void Insert(TEntity entity);
        List<TEntity> GetAll();
        void Update(TEntity entityToUpdate);
        void Delete(object id);
        void Remove(TEntity entityToRemove);

        void SaveChanges();
    }

    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class 
    {
        internal System.Data.Entity.DbContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(System.Data.Entity.DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Remove(entityToDelete);
        }

        public List<TEntity> GetAll()
        {
            return dbSet.ToList();
        }

        public virtual void Remove(TEntity entityToRemove)
        {
            if (context.Entry(entityToRemove).State == EntityState.Detached)
            {
                dbSet.Attach(entityToRemove);
            }
            dbSet.Remove(entityToRemove);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void SaveChanges()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {


            }
        }


    }
}