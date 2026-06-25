namespace CompanyApp.Application.Abstractions;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}
