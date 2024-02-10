using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uplay.Models.SurveyModels
{
	public class Cust_Survey
	{
		public int ID { get; set; }
		public int Satisfaction { get; set; } = 0;
		[MinLength(1), MaxLength(500)]
		public string Comments { get; set; } = string.Empty;
		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; }
		[Column(TypeName = "datetime")]
		public DateTime UpdatedAt { get; set; }

	}
}
