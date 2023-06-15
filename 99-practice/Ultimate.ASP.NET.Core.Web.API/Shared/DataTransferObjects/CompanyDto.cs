using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects;

public record CompanyDto(Guid Id, string Name, string FullAddress);