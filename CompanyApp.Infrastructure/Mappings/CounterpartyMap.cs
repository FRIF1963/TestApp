using CompanyApp.Domain.Entities;
using FluentNHibernate.Mapping;

namespace CompanyApp.Infrastructure.Mappings;

public class CounterpartyMap : ClassMap<Counterparty>
{
    public CounterpartyMap()
    {
        Table("counterparties");
        Id(x => x.Id).GeneratedBy.Identity();
        Map(x => x.Name).Not.Nullable().Length(255);
        Map(x => x.Inn).Not.Nullable().Length(12);
        References(x => x.Curator).Column("curator_id").Not.Nullable().Cascade.None();
    }
}
