using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {

        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        private ApplicationDbContext db;

        public Repositorio(ApplicationDbContext db)
        {
            this.db = db;
        }

        public Repositorio(ApplicationDbContext db, DbSet<T> dbSet)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public void Agregar(T entidad)
        {
            dbSet.Add(entidad);   // Insert into Table
        }

        public T Obtener(int id)
        {
            return dbSet.Find(id); // Select * from 
        }

        // Obtener Primero
        public T ObtenerPrimero(Expression<Func<T, bool>> filter = null, string incluirPropiedades = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter); // Select * from ...
            }

            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp);
                }
            }
                       

            return query.FirstOrDefault();
        }


        // Obtener Todos

        
        public IEnumerable<T> ObtenerTodos(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null)
        {
            IQueryable<T> query = dbSet;

            if(filter != null)
            {
                query = query.Where(filter); // Select * from ...
            }

            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();

        }
        

        // Remover entidad por ID
        public void Remover(int id)
        {
            T entidad = dbSet.Find(id);
            Remover(entidad);
        }


        // Remover entidad
        public void Remover(T entidad)
        {
            dbSet.Remove(entidad);
        }


        // Remover por rango
        public void RemoverRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }
    }
}
