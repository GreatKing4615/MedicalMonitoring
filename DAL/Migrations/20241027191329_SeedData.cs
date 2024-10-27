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
INSERT INTO public.""Devices"" (""ModelName"", ""Type"", ""Status"", ""CreateTs"", ""BeginDate"", ""EndDate"")
VALUES
('Siemens Magnetom', 2, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '365 days', CURRENT_TIMESTAMP + INTERVAL '365 days'),
('GE Discovery', 1, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '200 days', CURRENT_TIMESTAMP + INTERVAL '200 days'),
('Philips Affiniti', 0, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '100 days', CURRENT_TIMESTAMP + INTERVAL '300 days'),
('Toshiba Aquilion', 1, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '150 days', CURRENT_TIMESTAMP + INTERVAL '150 days'),
('Olympus EVIS EXERA', 4, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP - INTERVAL '90 days', CURRENT_TIMESTAMP + INTERVAL '270 days');

-- Добавляем исследования
-- Добавляем исследования
INSERT INTO public.""Researches"" (""Name"", ""Duration"")
VALUES
('МРТ головного мозга', INTERVAL '30 minutes'),
('КТ легких', INTERVAL '20 minutes'),
('УЗИ брюшной полости', INTERVAL '15 minutes'),
('Рентген грудной клетки', INTERVAL '10 minutes'),
('Эндоскопия желудка', INTERVAL '25 minutes');


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
        
        INSERT INTO public.""ResearchHistories"" (""ResearchId"", ""ResearchDate"", ""DeviceId"")
        SELECT
            (SELECT ""Id"" FROM public.""Researches"" ORDER BY random() LIMIT 1), -- Случайное исследование
            Day.""TheDate"" + (random() * INTERVAL '1 day'), -- Дата исследования с случайным временем
            (SELECT ""Id"" FROM public.""Devices"" ORDER BY random() LIMIT 1) -- Случайное устройство
        FROM generate_series(1, FLOOR(random() * 11 + 5)::int);
    END LOOP;
END $$;


-- Генерируем данные обслуживания для устройств
INSERT INTO public.""ServiceHistories"" (""DeviceId"", ""ServiceDate"", ""WorkType"", ""ResponsibleId"")
SELECT
    D.""Id"",
    CURRENT_TIMESTAMP - (random() * INTERVAL '90 days'),
    FLOOR(random() * 2), -- 0 или 1
    (SELECT ""Id"" FROM public.""Users"" WHERE ""Role"" = 1 ORDER BY random() LIMIT 1)
FROM public.""Devices"" D
WHERE D.""Status"" = 0;
";


            migrationBuilder.Sql(query, true) ;

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
