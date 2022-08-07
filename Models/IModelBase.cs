
namespace LinkVault.Models
{
    public abstract class ModelBase<T> where T : class
    {
        public abstract T AsDto();
    }
}