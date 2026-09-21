namespace ITSM.Web.Client.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ITSM.Web.Client.Models;

    public interface IStatusesClient
    {
        Task<IEnumerable<StatusDto>> GetAllAsync(CancellationToken ct = default);
        Task<StatusDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<StatusDto> CreateAsync(StatusDto status, CancellationToken ct = default);
        Task UpdateAsync(int id, StatusDto status, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
