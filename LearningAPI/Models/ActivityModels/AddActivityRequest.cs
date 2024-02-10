using System.ComponentModel.DataAnnotations;

namespace Uplay.Models.ActivityModels
{
	public class AddActivityRequest
	{
		[Required, MinLength(3), MaxLength(100)]
		public string activity_Name { get; set; } = string.Empty;
		[Required, MinLength(3), MaxLength(100)]
		public string tag_Name { get; set; } = string.Empty;

		[Required, MinLength(3), MaxLength(500)]
		public string activity_Desc { get; set; } = string.Empty;

		[MaxLength(20)]
		public string? ImageFile { get; set; }
	}
}
