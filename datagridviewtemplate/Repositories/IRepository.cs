using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datagridviewtemplate.Repositories
{
    interface IRepository<T> where T : class
    {
        int Insert(T obj);   // create
        int Update(T obj); // update
        string Delete(string key); // delete
        T GetById(string key); // read
        BindingList<T> GetAll();    //  returns Bindinglist of all items of type obj
    }
}
