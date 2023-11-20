namespace Evaluation.Helpers;

using Dapper;
using System.Data;
using System.Data.SqlClient;

public class DataContext
{
    private readonly IConfiguration _config;

    private bool _inInitialization = true;


    public DataContext(IConfiguration config)
    {
        _config = config;
    }

    public IDbConnection CreateConnection()
    {
        try
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");
            if (_inInitialization)
            {
                connectionString = _config.GetConnectionString("MasterConnection");
                _inInitialization = false;
            }

            return new SqlConnection(connectionString);
        }
        catch (Exception e)
        {

            throw new AppException(e.Message);
        }
    }

    public async Task Init()
    {
        try
        {
            await InitDatabase();
            await InitTables();
        }
        catch (Exception e)
        {

            throw new AppException(e.Message);
        }
    }


    private async Task<bool> InitDatabase()
    {

        try
        {
            using var connection = CreateConnection();
            var sql = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Fleet') CREATE DATABASE [Fleet];";

            return (await connection.ExecuteAsync(sql)) > 0;
        }
        catch (Exception e)
        {

            throw new AppException(e.Message);
        }
    }

    private async Task<bool> InitTables()
    {
        try
        {

            using var connection = CreateConnection();
            return (await _initVehicles()) > 0;

            async Task<int> _initVehicles()
            {
                var sql = """
                IF NOT EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_Name = 'Vehicles')
                CREATE TABLE Vehicles (
                    VehicleID  INT NOT NULL PRIMARY KEY IDENTITY,
                    RegNo NVARCHAR(50),
                    Make NVARCHAR(50),
                    Model NVARCHAR(50),
                    Color NVARCHAR(50),
                    EngineNo NVARCHAR(50),
                    ChasisNo NVARCHAR(50),
                    DateOfPurchase DATETIME,
                    Active BIT
                );
            """;

                return await connection.ExecuteAsync(sql);
            }

        }
        catch (Exception e)
        {

            throw new AppException(e.Message);
        }


    }
}