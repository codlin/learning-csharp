using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts;

public interface IDataShaper<T>
{
    IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString);
    ExpandoObject ShapeData(T entity, string fieldsString);
}
