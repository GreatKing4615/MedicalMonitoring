﻿using DAL.Enums;

namespace BL.Dtos
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string ModelName { get; set; }
        public DeviceType Type { get; set; }
        public DeviceStatus Status { get; set; }
    }

    public class ResearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DeviceType DeviceType { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public Role Role { get; set; }
    }

    public class ResearchHistoryDto
    {
        public int Id { get; set; }
        public int ResearchId { get; set; }
        public DateTimeOffset ResearchDate { get; set; }
        public int DeviceId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }

    public class ServiceHistoryDto
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTimeOffset ServiceDate { get; set; }
        public WorkType WorkType { get; set; }
        public int ResponsibleUserId { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
    public class SimulationResultDto
    {
        public int DeviceId { get; set; }
        public string DeviceModelName { get; set; }
        public double LoadPercentage { get; set; }
        public bool IsOverloaded { get; set; }
        public int RecommendedAdditionalUnits { get; set; }
    }
    public class PurchaseRecommendationDto
    {
        public DeviceType DeviceType { get; set; }
        public string DeviceTypeName { get; set; }
        public int ExistingUnits { get; set; }
        public double TotalLoadPercentage { get; set; }
        public double AverageLoadPercentage { get; set; }
        public int RecommendedAdditionalUnits { get; set; }
    }
 

}

