namespace CompanyApp.Domain.Entities;

public class Order : Entity
{
    public virtual DateTime OrderDate { get; set; }

    public virtual decimal Amount { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Counterparty Counterparty { get; set; } = null!;
}
