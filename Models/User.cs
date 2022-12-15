using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618
namespace WeddingPlanner.Models;
public class User
{
    [Key]
	public int UserId {get;set;}
    [Required]
    [MinLength(2, ErrorMessage = "First name must be longer than 2 characters")]
    [Display(Name = "First Name")]

	public string FName {get;set;}
    [Required]
    [MinLength(2, ErrorMessage = "Last name must be longer than 2 characters")]
    [Display(Name = "Last Name")]

	public string LName {get;set;}
    [Required]
    [EmailAddress]
    [UniqueEmail]

	public string Email {get;set;}
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]

	public string Password {get;set;}
    
    public List<Connection> WeddingsList {get;set;} = new List<Connection>();
    
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [NotMapped]
    [Compare("Password")]
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
	public string CPassword {get;set;}
}

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
    	// Though we have Required as a validation, sometimes we make it here anyways
    	// In which case we must first verify the value is not null before we proceed
        if(value == null)
        {
    	    // If it was, return the required error
            return new ValidationResult("Email is required!");
        }
    
    	// This will connect us to our database since we are not in our Controller
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        // Check to see if there are any records of this email in our database
        if(_context.Users.Any(e => e.Email == value.ToString()))
        {
    	    // If yes, throw an error
            return new ValidationResult("Email must be unique!");
        } else {
    	    // If no, proceed
            return ValidationResult.Success;
        }
    }
}