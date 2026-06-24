using CompanyApp.Domain.Enums;

namespace CompanyApp.Domain.Entities;

public class Employee : Entity
{
    public virtual string FullName { get; set; } = string.Empty;

    public virtual EmployeePosition Position { get; set; }

    public virtual DateTime BirthDate { get; set; }
}
