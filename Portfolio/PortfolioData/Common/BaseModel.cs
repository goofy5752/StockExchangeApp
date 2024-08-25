using System.ComponentModel.DataAnnotations;

namespace PortfolioData.Common
{
    public abstract class BaseModel<TKey>
    {
        [Key]
        public TKey Id { get; set; }
    }
}