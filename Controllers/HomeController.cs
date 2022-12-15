using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    //******************************LOGIN and REG***********************************************
    [HttpPost("users/create")]
    public IActionResult RegisterUser(User newUser)
    {
        if (ModelState.IsValid)
        {
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("FName", newUser.FName);
            return RedirectToAction("AllWeddings");
        } else {
            return View("Index");
        }
    }

    [HttpPost("users/login")]
    public IActionResult LoginUser(LoginUser loginUser)
    {
        if(ModelState.IsValid)
        {

            User? userInDb = _context.Users.FirstOrDefault(c=>c.Email == loginUser.LEmail);
            if (userInDb == null)
            {
                ModelState.AddModelError("LEmail", "Invalid Email/Password");
                return View("Index");
            }

            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LEmail", "Invalid Email/Password");
                return View("Index");
            }

            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            HttpContext.Session.SetString("FName", userInDb.FName);
            return RedirectToAction("AllWeddings");

        } else {
            return View("Index");
        }
    }
    //******************************LOGOUT***********************************************
    [HttpGet("logout")]
    public IActionResult LogOutUser()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    //******************************DISPLAY ALL WEDDINGS***********************************************
    [SessionCheck]
    [HttpGet("weddings")]
    public IActionResult AllWeddings()
    {   

        MyViewModel MyModels = new MyViewModel
        {
            AllWeddings = _context.Weddings.Include(c=>c.GuestList)
                                            .ThenInclude(c=>c.User)
                                            .ToList()
                                            
        };
        
        
        return View("AllWeddings", MyModels);
    }

    //****************************DISPLAY 1 WEDDING**************************************************
    [SessionCheck]
    [HttpGet("weddings/{Id}")]
    public IActionResult OneWedding(int Id)
    {
        MyViewModel MyModels = new MyViewModel
        {
            Wedding = _context.Weddings.Include(c=>c.GuestList)
                                        .ThenInclude(c=>c.User)
                                        .FirstOrDefault(c=>c.WeddingId == Id)
        };
        return View(MyModels);
    }

    //****************************WEDDING FORM**************************************************
    [SessionCheck]
    [HttpGet("weddings/new")]
    public IActionResult WeddingForm()

    {
        ViewBag.id = HttpContext.Session.GetInt32("UserId");
        System.Console.WriteLine(ViewBag.id);
        return View();
    }

    //****************************CREATE WEDDING**************************************************
    [SessionCheck]
    [HttpPost("weddings/create")]
    public IActionResult CreateWedding(Wedding Wedding)
    {
        if (ModelState.IsValid)
        {   
            _context.Add(Wedding);
            _context.SaveChanges();
            return RedirectToAction("OneWedding", new {id = Wedding.WeddingId});
        }
        return View("WeddingForm");
    }

    //****************************DESTROY WEDDING*************************************************
    [HttpPost("wedding/{WeddingId}/destroy")]
    public IActionResult DestroyWedding(int WeddingId)
    {
        System.Console.WriteLine(WeddingId);
        Wedding? WeddingToDelete = _context.Weddings.SingleOrDefault(c=>c.WeddingId == WeddingId);
        _context.Weddings.Remove(WeddingToDelete);
        _context.SaveChanges();
        return RedirectToAction("AllWeddings");
    }
    //****************************RSVP to Wedding/Create Connection*************************************************
    [HttpPost("Connections/{WeddingId}/{UserId}/create")]
    public IActionResult RSVP(int WeddingId, int UserId)
    {
        Connection newConnection = new Connection();
        newConnection.UserId = UserId;
        newConnection.WeddingId = WeddingId;

        _context.Connections.Add(newConnection);
        _context.SaveChanges();
        return RedirectToAction("AllWeddings");
        
    }
    //****************************UNRSVP to Wedding/DELETE Connection*************************************************
    [HttpPost("Connections/{WeddingId}/{UserId}/destroy")]
    public IActionResult UNRSVP(int WeddingId, int UserId)
    {
        Connection? connectionToDelete = _context.Connections.SingleOrDefault(c=>c.WeddingId == WeddingId && c.UserId == UserId);
        _context.Connections.Remove(connectionToDelete);
        _context.SaveChanges();
        return RedirectToAction("AllWeddings");
        
    }
    //****************************ERROR*************************************************

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


    //**********************************CHECK SESSION ATTRIBUTE*********************************
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}