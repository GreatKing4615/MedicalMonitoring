using System;

public class MedicalDevice
{
	/// <summary>
	/// Медицинское оборудование
	/// </summary>
	public MedicalDevice()
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime LastService { get; set; }
		public string Maunfactor { get; set; }
	}
}
