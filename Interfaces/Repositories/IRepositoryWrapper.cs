namespace Bookshop.Interfaces.Repositories;

public interface IRepositoryWrapper
{
    IBookRepo Books { get; }
    
    ICategoryRepo Categories { get; }
    
    #region General Methods
    void Save();
    Task<bool> SaveAsync();
    #endregion
}