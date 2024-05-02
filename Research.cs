using System;

public class Research
{
	/// <summary>
	/// Вид исследования
	/// </summary>
	public Research()
	{
		public string Name { get;set; }
		public List<int> RequiredDeviceTypeIds { get; set; }
		public List<DeviceType> DeviceTypes { get; set; }
    }
}
