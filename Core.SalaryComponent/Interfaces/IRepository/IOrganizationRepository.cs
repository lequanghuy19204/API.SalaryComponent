using Core.SalaryComponent.DTOs;

namespace Core.SalaryComponent.Interfaces.IRepository;

/// <summary>
/// Interface Repository quản lý đơn vị/tổ chức (Organization)
/// Cung cấp các phương thức truy xuất dữ liệu đơn vị từ database
/// </summary>
public interface IOrganizationRepository
{
    /// <summary>
    /// Lấy cây đơn vị tổ chức theo cấu trúc phân cấp (parent-child)
    /// </summary>
    /// <returns>Danh sách đơn vị dạng cây với các node con lồng nhau</returns>
    Task<IEnumerable<OrganizationTreeDto>> GetTreeAsync();
}
