using CompanyApp.Domain.Entities;
using FluentNHibernate.Mapping;

namespace CompanyApp.Infrastructure.Mappings;

public class OrderMap : ClassMap<Order>
{
    public OrderMap()
    {
        Table("orders");
        Id(x => x.Id).GeneratedBy.Identity();
        Map(x => x.OrderDate).Column("order_date").Not.Nullable();
        Map(x => x.Amount).Not.Nullable().Precision(18).Scale(2);
        References(x => x.Employee).Column("employee_id").Not.Nullable().Cascade.None();
        References(x => x.Counterparty).Column("counterparty_id").Not.Nullable().Cascade.None();
    }
}
