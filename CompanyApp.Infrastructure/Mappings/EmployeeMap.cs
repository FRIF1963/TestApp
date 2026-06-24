using CompanyApp.Domain.Entities;
using CompanyApp.Domain.Enums;
using FluentNHibernate.Mapping;

namespace CompanyApp.Infrastructure.Mappings;

public class EmployeeMap : ClassMap<Employee>
{
    public EmployeeMap()
    {
        Table("employees");
        Id(x => x.Id).GeneratedBy.Identity();
        Map(x => x.FullName).Not.Nullable().Length(255);
        Map(x => x.Position).CustomType<EmployeePosition>().Not.Nullable();
        Map(x => x.BirthDate).Not.Nullable();
    }
}
