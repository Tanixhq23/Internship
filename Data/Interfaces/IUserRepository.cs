using Entity;

namespace Data.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task GetUserByIdAsync(int id);
    }
}
