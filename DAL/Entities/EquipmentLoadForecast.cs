using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class EquipmentLoadForecast
    {
        public int Id { get; set; }
        public DeviceType DeviceType { get; set; }
        public DateTime Date { get; set; }
        public int PredictedPatientCount { get; set; }
        public double LoadPercentage { get; set; }
        public bool IsOverloaded { get; set; }
        public DateTimeOffset GeneratedDate { get; set; } // Дата генерации прогноза
    }
}
