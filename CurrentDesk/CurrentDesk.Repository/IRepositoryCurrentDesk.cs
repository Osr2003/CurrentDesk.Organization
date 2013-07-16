






using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;


namespace CurrentDesk.Repository.CurrentDesk
{ 
	internal interface IRepository<T> 
    {
		IUnitOfWork UnitOfWork { get; set; }
		IQueryable<T> All();
		IQueryable<T> Find(Func<T, bool> expression);
		void Add(T entity);
		void Delete(T entity);
		void Save();
    }
}


