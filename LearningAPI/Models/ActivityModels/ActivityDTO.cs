namespace Uplay.Models.ActivityModels
{
	public class ActivityDTO
	{
		public int activity_Id { get; set; }
		public int tag_Id { get; set; }

		public string activity_Name { get; set; } = string.Empty;
		public string tag_Name { get; set; } = string.Empty;

		public string activity_Desc { get; set; } = string.Empty;

		public string? ImageFile { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }

		public int UserId { get; set; }

		public UserBasicDTO? User { get; set; }
	}
}
