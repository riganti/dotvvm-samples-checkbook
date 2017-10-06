using System.ComponentModel.DataAnnotations;

namespace CheckBook.DataAccess.Model
{
    public interface IEntity<TKey>
    {
        [Key]
        TKey Id { get; set; }
    }
}
