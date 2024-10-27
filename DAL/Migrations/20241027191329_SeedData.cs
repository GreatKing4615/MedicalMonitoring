using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string query = @"
    -- Добавляем устройства
    INSERT INTO public.""Devices"" (ModelName, Type, Status, CreateTs, BeginDate, EndDate)
    VALUES
    ('Siemens Magnetom', 2, 0, SYSDATETIMEOFFSET(), DATEADD(DAY, -365, SYSDATETIMEOFFSET()), DATEADD(DAY, 365, SYSDATETIMEOFFSET())),
    ('GE Discovery', 1, 0, SYSDATETIMEOFFSET(), DATEADD(DAY, -200, SYSDATETIMEOFFSET()), DATEADD(DAY, 200, SYSDATETIMEOFFSET())),
    ('Philips Affiniti', 0, 0, SYSDATETIMEOFFSET(), DATEADD(DAY, -100, SYSDATETIMEOFFSET()), DATEADD(DAY, 300, SYSDATETIMEOFFSET())),
    ('Toshiba Aquilion', 1, 0, SYSDATETIMEOFFSET(), DATEADD(DAY, -150, SYSDATETIMEOFFSET()), DATEADD(DAY, 150, SYSDATETIMEOFFSET())),
    ('Olympus EVIS EXERA', 4, 0, SYSDATETIMEOFFSET(), DATEADD(DAY, -90, SYSDATETIMEOFFSET()), DATEADD(DAY, 270, SYSDATETIMEOFFSET()));

    -- Добавляем исследования
    INSERT INTO public.""Researches"" (Name, Duration)
    VALUES
    ('МРТ головного мозга', 30),
    ('КТ легких', 20),
    ('УЗИ брюшной полости', 15),
    ('Рентген грудной клетки', 10),
    ('Эндоскопия желудка', 25);

    -- Добавляем пользователей
    INSERT INTO public.""Users"" (Login, Role, CreateDate)
    VALUES
    ('admin', 0, SYSDATETIMEOFFSET()),
    ('operator1', 1, SYSDATETIMEOFFSET()),
    ('operator2', 1, SYSDATETIMEOFFSET()),
    ('technician', 1, SYSDATETIMEOFFSET());

    -- Генерируем данные за последние 100 дней
    DECLARE @StartDate DATE = DATEADD(DAY, -99, CAST(GETDATE() AS DATE));
    DECLARE @EndDate DATE = CAST(GETDATE() AS DATE);

    WITH Dates AS (
        SELECT @StartDate AS TheDate
        UNION ALL
        SELECT DATEADD(DAY, 1, TheDate)
        FROM Dates
        WHERE DATEADD(DAY, 1, TheDate) <= @EndDate
    )
    INSERT INTO public.""ResearchHistories"" (ResearchId, ResearchDate, DeviceId)
    SELECT
        (SELECT TOP 1 Id FROM Research ORDER BY NEWID()),
        DATEADD(MINUTE, ABS(CHECKSUM(NEWID())) % 1440, CAST(D.TheDate AS DATETIMEOFFSET)),
        (SELECT TOP 1 Id FROM Device ORDER BY NEWID())
    FROM Dates D
    CROSS APPLY (
        SELECT TOP (ABS(CHECKSUM(NEWID())) % 11 + 5) 1 AS Dummy
        FROM sys.columns c1, sys.columns c2
    ) AS T
    OPTION (MAXRECURSION 0);

    -- Генерируем данные обслуживания для устройств
    INSERT INTO public.""ServiceHistories"" (DeviceId, ServiceDate, WorkType, UserId)
    SELECT
        D.Id,
        DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 90, SYSDATETIMEOFFSET()),
        ABS(CHECKSUM(NEWID())) % 2,
        (SELECT TOP 1 Id FROM [User] WHERE Role = 1 ORDER BY NEWID())
    FROM Device D
    WHERE D.Status = 0;
";


            migrationBuilder.Sql(query, true) ;

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
