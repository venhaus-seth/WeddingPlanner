#pragma warning disable CS8618
namespace WeddingPlanner.Models;
public class MyViewModel
{
    public Wedding? Wedding {get;set;}
    public List<Wedding> AllWeddings {get;set;}
    public List<User> AllUsers {get;set;}
}