using Entities.Models;

namespace Contracts;

public interface ICompanyRepository
{
    IEnumerable<Company> GetAllCompanies(bool trackChanges);
    Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);

    Company GetCompany(Guid companyId, bool trackChanges);
    void DeleteCompany(Company company);
    Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges);

    IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
    Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

    void CreateCompany(Company company);
}
