using System.ComponentModel;

namespace CompanyApp.Domain.Enums;

public enum EmployeePosition
{
    [Description("Руководитель")]
    Manager = 0,

    [Description("Работник")]
    Worker = 1
}
