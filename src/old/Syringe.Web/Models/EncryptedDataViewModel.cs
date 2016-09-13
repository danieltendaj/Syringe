namespace Syringe.Web.Models
{
	public class EncryptedDataViewModel
	{
		public bool IsEnabled { get; set; }
		public string PlainValue { get; set; }
		public string EncryptedValue { get; set; }
	}
}