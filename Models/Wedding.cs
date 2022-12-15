using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618
namespace WeddingPlanner.Models;
public class Wedding
{
    [Key]
	public int WeddingId {get;set;}
	[Required]
	[MinLength(2, ErrorMessage = "Name must be longer than 2 characters")]
	public string WedderOne {get;set;}
	[Required]
	[MinLength(2, ErrorMessage = "Name must be longer than 2 characters")]
	public string WedderTwo {get;set;}
	[Required]
	[FutureDate]
	public DateTime Date {get;set;}
	[Required]
	public string Address {get;set;}
	[Required]
	public int UserId {get;set;}
	public User? Creator {get;set;}
	public List<Connection> GuestList {get;set;} = new List<Connection>();
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value == null)
        {
            return new ValidationResult("Date is required!");
        }

		DateTime WedDate = Convert.ToDateTime(value);
		if(DateTime.Compare(WedDate, DateTime.Now) <= 0)
        {
            return new ValidationResult("Wedding date must be in the future!");
        } else {
            return ValidationResult.Success;
        }
    }
}

