namespace KotKonnect.Infrastructure.Data;

using System.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}