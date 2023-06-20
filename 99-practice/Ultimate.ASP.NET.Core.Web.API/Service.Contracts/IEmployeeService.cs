using Shared.DataTransferObjects;
using Shared.RequestFeatures;

using Entities.Models;

namespace Service.Contracts;

public interface IEmployeeService
{
    Task<(IEnumerable<ExpandoObject> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);
    EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges);
    EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges);
    void UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges);
    (EmployeeForUpdateDto employeeToPatch, Employee employeeEntity) GetEmployeeForPatch(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges);
    void SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity);
}
