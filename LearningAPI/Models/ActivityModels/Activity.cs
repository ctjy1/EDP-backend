using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uplay.Models.ActivityModels
{
	public class Activity
	{
		public int Id { get; set; }

		[Required, MinLength(3), MaxLength(100)]
		public int tag_Id { get; set; }
		[Required, MinLength(1), MaxLength(100)]
		public string activity_Name { get; set; } = string.Empty;
		[Required, MinLength(3), MaxLength(500)]
		public string tag_Name { get; set; } = string.Empty;
		[Required, MinLength(1), MaxLength(100)]
		public string activity_Desc { get; set; } = string.Empty;

		[MaxLength(20)]
		public string? ImageFile { get; set; }

		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; }

		[Column(TypeName = "datetime")]
		public DateTime UpdatedAt { get; set; }

		// Foreign key property
		public int UserId { get; set; }

		// Navigation property to represent the one-to-many relationship
		public User? User { get; set; }
	}
}
