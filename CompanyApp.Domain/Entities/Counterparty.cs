namespace CompanyApp.Domain.Entities;

public class Counterparty : Entity
{
    public virtual string Name { get; set; } = string.Empty;

    public virtual string Inn { get; set; } = string.Empty;

    public virtual Employee Curator { get; set; } = null!;
}
