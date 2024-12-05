using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    public partial class AddEquipmentLoadForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentLoadForecasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceType = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PredictedPatientCount = table.Column<int>(type: "integer", nullable: false),
                    LoadPercentage = table.Column<double>(type: "double precision", nullable: false),
                    IsOverloaded = table.Column<bool>(type: "boolean", nullable: false),
                    GeneratedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentLoadForecasts", x => x.Id);
                });

            string query = @"
-- Очищаем таблицы
TRUNCATE TABLE public.""ResearchHistories"" RESTART IDENTITY CASCADE;
TRUNCATE TABLE public.""ServiceHistories"" RESTART IDENTITY CASCADE;
TRUNCATE TABLE public.""Devices"" RESTART IDENTITY CASCADE;
TRUNCATE TABLE public.""Researches"" RESTART IDENTITY CASCADE;
TRUNCATE TABLE public.""Users"" RESTART IDENTITY CASCADE;

-- Добавляем устройства
INSERT INTO public.""Devices"" (""ModelName"", ""Type"", ""Status"", ""CreateTs"", ""BeginDate"", ""EndDate"")
VALUES
('Siemens Magnetom', 2, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '365 days', CURRENT_TIMESTAMP + INTERVAL '365 days'),
('GE Discovery', 1, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '200 days', CURRENT_TIMESTAMP + INTERVAL '200 days'),
('Philips Affiniti', 0, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '100 days', CURRENT_TIMESTAMP + INTERVAL '300 days'),
('Toshiba Aquilion', 1, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '150 days', CURRENT_TIMESTAMP + INTERVAL '150 days'),
('Olympus EVIS EXERA', 4, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '90 days', CURRENT_TIMESTAMP + INTERVAL '270 days');

-- Добавляем исследования
INSERT INTO public.""Researches"" (""Name"", ""Duration"", ""DeviceTypes"")
VALUES
('МРТ головного мозга', INTERVAL '30 minutes', ARRAY[2]),
('КТ легких', INTERVAL '20 minutes', ARRAY[1]),
('УЗИ брюшной полости', INTERVAL '15 minutes', ARRAY[0]),
('Рентген грудной клетки', INTERVAL '10 minutes', ARRAY[3]),
('Эндоскопия желудка', INTERVAL '25 minutes', ARRAY[4]);

-- Добавляем пользователей
INSERT INTO public.""Users"" (""Login"", ""Role"", ""CreateDate"")
VALUES
('admin', 0, CURRENT_TIMESTAMP),
('operator1', 1, CURRENT_TIMESTAMP),
('operator2', 1, CURRENT_TIMESTAMP),
('technician', 1, CURRENT_TIMESTAMP);

-- Генерируем данные за последние 100 дней
DO $$
DECLARE
    StartDate DATE := CURRENT_DATE - INTERVAL '99 days';
    EndDate DATE := CURRENT_DATE;
    Day RECORD;
BEGIN
    FOR Day IN 
        SELECT generate_series(StartDate, EndDate, INTERVAL '1 day') AS ""TheDate""
    LOOP
        -- Генерируем от 5 до 15 записей на каждый день
        PERFORM * FROM generate_series(1, FLOOR(random() * 11 + 5)::int);
        
        INSERT INTO public.""ResearchHistories"" (""ResearchId"", ""ResearchDate"", ""DeviceId"", ""StartTime"", ""EndTime"")
        SELECT
            R.""Id"",
            RH_StartTime, -- Используем StartTime как ResearchDate
            D.""Id"",
            RH_StartTime,
            RH_EndTime
        FROM
            (SELECT * FROM public.""Researches"" ORDER BY random() LIMIT 1) R,
            (SELECT * FROM public.""Devices"" ORDER BY random() LIMIT 1) D,
            LATERAL (
                SELECT
                    RH_StartTime,
                    (RH_StartTime + INTERVAL '10 minutes' + (random() * INTERVAL '50 minutes'))::timestamp AS RH_EndTime
                FROM (
                    SELECT
                        (Day.""TheDate"" + (random() * INTERVAL '1 day'))::timestamp AS RH_StartTime
                ) sub
            ) t,
            generate_series(1, FLOOR(random() * 11 + 5)::int);
    END LOOP;
END $$;

-- Генерируем данные обслуживания для устройств
DO $$
DECLARE
    rec RECORD;
BEGIN
    FOR rec IN
        SELECT D.""Id"" AS DeviceId
        FROM public.""Devices"" D
        WHERE D.""Status"" = 0
    LOOP
        -- Генерируем от 1 до 5 записей обслуживания для каждого устройства
        FOR i IN 1..FLOOR(random() * 4 + 1)::int LOOP
            INSERT INTO public.""ServiceHistories"" (""DeviceId"", ""ServiceDate"", ""WorkType"", ""ResponsibleId"", ""StartTime"", ""EndTime"")
            SELECT
                rec.DeviceId,
                SH_StartTime, -- Используем StartTime как ServiceDate
                FLOOR(random() * 2)::int AS ""WorkType"", -- 0 или 1
                (SELECT ""Id"" FROM public.""Users"" WHERE ""Role"" = 1 ORDER BY random() LIMIT 1) AS ""ResponsibleId"",
                SH_StartTime,
                SH_EndTime
            FROM
                LATERAL (
                    SELECT
                        SH_StartTime,
                        (SH_StartTime + INTERVAL '1 hour' + (random() * INTERVAL '71 hours'))::timestamp AS SH_EndTime
                    FROM (
                        SELECT
                            (CURRENT_TIMESTAMP - (random() * INTERVAL '90 days'))::timestamp AS SH_StartTime
                    ) sub
                ) t;
        END LOOP;
    END LOOP;
END $$;

";


            migrationBuilder.Sql(query, true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentLoadForecasts");
        }
    }
}
