using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uplay.Models.SurveyModels
{
	public class Cust_Feedback
	{
		public int Id { get; set; }
		[MaxLength(100)]
		public string Enquiry_Subject { get; set; } = string.Empty;
		[MaxLength(500)]
		public string Customer_Enquiry { get; set; } = string.Empty;
		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; }
		[Column(TypeName = "datetime")]
		public DateTime UpdatedAt { get; set; }
	}
}
