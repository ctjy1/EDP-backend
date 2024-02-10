using System.ComponentModel.DataAnnotations;

namespace Uplay.Models.ActivityModels
{
	public class UpdateActivityRequest
	{
		[MinLength(3), MaxLength(100)]
		public string? activity_Name { get; set; }
		[MinLength(3), MaxLength(100)]
		public string? tag_Name { get; set; }

		[MinLength(3), MaxLength(500)]
		public string? activity_Desc { get; set; }

		[MaxLength(20)]
		public string? ImageFile { get; set; }
	}
}
