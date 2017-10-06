using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBook.DataAccess.Model
{
    public interface ISoftDeleteEntity<TKey> : IEntity<TKey>
    {
        bool IsDeleted { get; set; }
    }
}
