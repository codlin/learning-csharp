namespace Shared.DataTransferObjects;

[Serializable]
public record EmployeeForCreationDto(string Name, int Age, string Position);
