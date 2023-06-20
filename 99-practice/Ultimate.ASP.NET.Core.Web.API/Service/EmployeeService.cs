using AutoMapper;

using Contracts;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using Entities.Models;
using Entities.Exceptions;

namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    // public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
    // {
    //     // var company = _repository.Company.GetCompany(companyId, trackChanges);
    //     // if (company is null) throw new CompanyNotFoundException(companyId);
    //     CheckIfCompanyExists(companyId, trackChanges);

    //     var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);
    //     var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
    //     return employeesDto;
    // }
    public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(
        Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
    {
        if (!employeeParameters.ValidAgeRange)
            throw new MaxAgeRangeBadRequestException();

        CheckIfCompanyExists(companyId, trackChanges);
        var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);
        return (employees: employeesDto, metaData: employeesWithMetaData.MetaData);
    }

    public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        // var company = _repository.Company.GetCompany(companyId, trackChanges);
        // if (company is null) throw new CompanyNotFoundException(companyId);

        // var employeeDb = _repository.Employee.GetEmployee(companyId, id, trackChanges);
        // if (employeeDb is null) throw new EmployeeNotFoundException(id);

        CheckIfCompanyExists(companyId, trackChanges);
        var employeeDb = GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);


        var employee = _mapper.Map<EmployeeDto>(employeeDb);
        return employee;
    }

    public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        CheckIfCompanyExists(companyId, trackChanges);

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        _repository.Save();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
        return employeeToReturn;
    }

    public void UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
    {
        CheckIfCompanyExists(companyId, compTrackChanges);

        var employeeEntity = GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);


        _mapper.Map(employeeForUpdate, employeeEntity);
        _repository.Save();
    }

    public (EmployeeForUpdateDto employeeToPatch, Employee employeeEntity) GetEmployeeForPatch(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, compTrackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);


        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
        return (employeeToPatch, employeeEntity);
    }

    public void SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
        _repository.Save();
    }

    private void CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        var company = _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null) throw new CompanyNotFoundException(companyId);
    }

    private Employee GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
    {
        var employeeDb = _repository.Employee.GetEmployee(companyId, id, trackChanges);
        if (employeeDb is null) throw new EmployeeNotFoundException(id);
        return employeeDb;
    }
}
