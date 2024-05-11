
using io_projekt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Diagnostics;
using System.Threading;
using System.Data.SqlClient;
using Thread = io_projekt.Models.Thread;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;

namespace io_projekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISession _session;
        private int currentUserID;
        private string whoIsLogged;  // sprawdzanie kto jest zalogowany - rozne funkcje dla roznych kont
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _session = httpContextAccessor.HttpContext.Session;
        }



        public IActionResult Index()
        {

            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            if (currentUserID != 0)
            {
                whoIsLogged = MainUser.GetUserById(currentUserID).user.getAccountType();
                ViewBag.UserName = MainUser.GetUserById(currentUserID).user.getName();
                ViewBag.IsLoggedIn = whoIsLogged;
                ViewBag.CurrentUserID = currentUserID;
            }
            ViewBag.CurrentSite = "Home";
            return View();
        }


        
        public IActionResult SearchProfile(List<MainUser> users)
        {
            ViewBag.CurrentSite = "Search Profile";
            return View(users);
        }

        public IActionResult Forum()
        {
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            ViewBag.CurrentSite = "Forum";
            if (currentUserID != 0)
            {
                whoIsLogged = MainUser.GetUserById(currentUserID).user.getAccountType();
                ViewBag.IsLoggedIn = whoIsLogged;
                ViewBag.CurrentUserID = currentUserID;
            }
            // Retrieve entries for the specified threadId from your data source
            List<Thread> threads = Thread.GetAllThreads();
            List<Tuple<Thread, List<Post>>> threadDataList = new List<Tuple<Thread, List<Post>>>();
            List<Tuple<int, bool>> ratedThreadIdsAndRatings = new List<Tuple<int, bool>>();

            if (currentUserID != 0)
            {
                (String msg, bool boolean, ratedThreadIdsAndRatings) = Thread.GetThreadIdsAndRatingsByUser(currentUserID);

            }

            foreach (var thread in threads)
            {
                List<Post> posts = Post.GetPostsByThreadId(thread.getID());
                Tuple<Thread, List<Post>> threadData = new Tuple<Thread, List<Post>>(thread, posts);
                threadDataList.Add(threadData);
            }

            var dataToReturn = (threadDataList, ratedThreadIdsAndRatings);

            return View(dataToReturn);
        }


        public IActionResult Events()
        {
            ViewBag.CurrentSite = "Events";
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            if (currentUserID != 0)
            {
                whoIsLogged = MainUser.GetUserById(currentUserID).user.getAccountType();
                ViewBag.IsLoggedIn = whoIsLogged;
                ViewBag.CurrentUserID = currentUserID;
            }
            return View();
        }


        public IActionResult Courses()
        {
            ViewBag.CurrentSite = "Courses";
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            if (currentUserID != 0)
            {
                whoIsLogged = MainUser.GetUserById(currentUserID).user.getAccountType();
                ViewBag.IsLoggedIn = whoIsLogged;
                ViewBag.UserID = currentUserID;
                ViewBag.CurrentUserID = currentUserID;
            }
            return View();

        }


        [HttpPost]
        public IActionResult SearchProfile(string searchData)
        {
            ViewBag.CurrentSite = "Search Profile";
            List<MainUser> foundUsers = MainUser.searchUsersByNameOrSurname(searchData);
            return View(foundUsers);
        }

        public IActionResult Profile() {
            ViewBag.CurrentSite = "Your Profile";
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            (MainUser user, string msg) = MainUser.GetUserById(currentUserID);


            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(string name, string surname, string email, int age,  int skill, string password, string style, string gear) {
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            (MainUser user, string msg) = MainUser.GetUserById(currentUserID);
            if (style == "RESET") {
                foreach (var userStyle in Misc.GetUserStyle(currentUserID)) {
                    Misc.RemoveUserStyle(currentUserID, userStyle.ID);
                }
            }
            else {
                if (style != null) {
                    var addStyleResult = Misc.AddUserStyle(currentUserID, Int32.Parse(style));
                }
                
            }

            if (gear == "RESET") {
                foreach (var userGear in Misc.GetUserGear(currentUserID))
                {
                    Misc.RemoveUserGear(currentUserID, userGear.ID);
                }
            }
            else {
                if (gear != null) {
                    var addGearResult = Misc.AddUserGear(currentUserID, Int32.Parse(gear));
                }
            }
            

            if (user.getName() != name)
            {
                MainUser.EditAccount(currentUserID, "imie", name);
            }
            if (user.getLastName() != surname)
            {
                MainUser.EditAccount(currentUserID, "nazwisko", surname);
            }
            if (user.getEmial() != email) 
            {
                MainUser.EditAccount(currentUserID, "email", email);
            }
            if (user.getAge() != age)
            {
                MainUser.EditAccount(currentUserID, "wiek", age.ToString());
            }
            if (user.getSkills() != skill)
            {
                MainUser.EditAccount(currentUserID, "umiejetnosci", skill.ToString());
            }
            if (user.getPassword() != password)
            {
                MainUser.EditAccount(currentUserID, "haslo", password);
            }

            
            return RedirectToAction("Profile", user);
        }

           public IActionResult Course(int courseID)
{
       ViewBag.CurrentSite = "Courses";
       currentUserID = _session.GetInt32("currentUserID") ?? 0;
   		if (currentUserID != 0)
   		{
       		ViewBag.UserID = currentUserID;
   		}
       	ViewBag.courseId = courseID;
       ViewBag.rating = getCurrentUserCourseRating(courseID);
       return View();
}

   public double getCurrentUserCourseRating(int courseID)
   {
      
       var rate = 0;
       if (currentUserID != 0)
       {
           try
           {

               String connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                   Console.WriteLine("tabela: ");
                   connection.Open();
                   String query = "select * from OcenyKursy where (userID=" + currentUserID + " and courseID=" + courseID + ")";

                   using (SqlCommand command = new SqlCommand(query, connection))
                   {
                       using (SqlDataReader reader = command.ExecuteReader())
                       {

                           while (reader.Read())
                           {
                               
                               rate = Convert.ToInt32(reader.GetDouble(3));

                           }
                       }
                   }
               }

           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message);
           }
          
       }

       return rate;
       
   }

          [HttpPost]
  public IActionResult AddCourse(String title, String description, String difficulty)
  {
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            if (currentUserID != 0)
            {
                Course course = new Course();
                course.setTitle(title);
                course.setDescription(description);
                course.setAuthorID(currentUserID);
                course.setAuthorName(MainUser.GetUserById(currentUserID).user.getLogin());
                course.setDifficulty(int.Parse(difficulty));
                course.setRating(0);
                course.writeToDB();
            }

      return RedirectToAction("Courses");
  }




 	public IActionResult deleteCourse(int courseID)
{

            try
            {
                String connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
               // SQL query to delete a record from the "Kursy" table with the given ID
                string query = "DELETE FROM Kursy WHERE kursId = @kursId";

               // Create and open a connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                   // Create a command with the query and connection
        
            using (SqlCommand command = new SqlCommand(query, connection))
                    {
                       // Add parameter for the kursId
       
                       command.Parameters.AddWithValue("@kursId", courseID);

                      //  Execute the command to delete the record
       
                       int rowsAffected = command.ExecuteNonQuery();

                      //  Check if any rows were affected
                if (rowsAffected > 0)
                        {
                            Console.WriteLine("Record deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No record found with the provided ID: " + courseID);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Courses");
}

public IActionResult rateCourse(int rating, int courseID)
{
    currentUserID = _session.GetInt32("currentUserID") ?? 0;
    if (currentUserID != 0)
    {
        try
        {
            String connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
            // SQL query to insert a new row into the Kursy table
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Construct the SQL command with parameters
                string sql = @"
                 MERGE INTO OcenyKursy AS target
                 USING (VALUES (@userID, @courseID, @rating)) AS source (userID, courseID, rating)
                     ON target.userID = source.userID AND target.courseID = source.courseID
                 WHEN MATCHED THEN
                     UPDATE SET rating = source.rating
                 WHEN NOT MATCHED THEN
                     INSERT (userID, courseID, rating)
                     VALUES (source.userID, source.courseID, source.rating);";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@userID", currentUserID);
                    command.Parameters.AddWithValue("@courseID", courseID);
                    command.Parameters.AddWithValue("@rating", rating);

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }


    }
    return RedirectToAction("Course", new { courseID = courseID });
}

        [HttpPost]
        public IActionResult AddLesson(String title, String content, String videoURL, String courseID)
        {
            Lesson lesson = new Lesson();
            lesson.setTitle(title);
            lesson.setContent(content);
            lesson.setCourseID(int.Parse(courseID));
            if (string.IsNullOrEmpty(videoURL))
	    {
   		 lesson.setVideoURL("None");
	    }
	    else {
    		 lesson.setVideoURL(videoURL);
	    }
            lesson.setRating(0);
            lesson.writeToDB();

            return RedirectToAction("Course", new { courseID = courseID });
        }



 	 public IActionResult deleteLesson(int lessonID, int courseID)
 {

     try
     {
         String connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
         // SQL query to delete a record from the "Kursy" table with the given ID
         string query = "DELETE FROM Lekcje WHERE lekcjaId = @lekcjaId";

         // Create and open a connection to the database
         using (SqlConnection connection = new SqlConnection(connectionString))
         {
             connection.Open();

             // Create a command with the query and connection
             using (SqlCommand command = new SqlCommand(query, connection))
             {
                 // Add parameter for the kursId
                 command.Parameters.AddWithValue("@lekcjaId", lessonID);

                 // Execute the command to delete the record
                 int rowsAffected = command.ExecuteNonQuery();

                 // Check if any rows were affected
                 if (rowsAffected > 0)
                 {
                     Console.WriteLine("Record deleted successfully.");
                 }
                 else
                 {
                     Console.WriteLine("No lesson found with the provided ID: " + lessonID);
                 }
             }
         }
     }

     catch (Exception ex)
     {
         Console.WriteLine(ex.Message);
     }
     return RedirectToAction("Course", new { courseID = courseID });
 }


        public IActionResult Lesson(int classID)
        {
            ViewBag.CurrentSite = "Courses";
            ViewBag.LessonId = classID;
            ViewBag.UserId = currentUserID;
            return View();
        }
        public IActionResult AdminPanel()
        {
            ViewBag.CurrentSite = "Admin Panel";
            var threads = Thread.GetAllThreads();
			int firstThreadId = threads.Count > 0 ? threads[0].getID() : 0;
			List<Post> posts = Post.GetPostsByThreadId(firstThreadId);
			return View(posts);
        }

        [HttpPost]
        public IActionResult Login(string uname, string psw, bool remember)
        {
            Console.WriteLine("LOGIN: "+uname + "  psw: " + psw);
            var cache = MainUser.GetCacheInstance();
            cache.Remove("AllUsers");

            bool pswCorrect = MainUser.CheckPassword(uname, psw);
            if (pswCorrect)
            {
                currentUserID = MainUser.GetUserIdByLogin(uname);

                _session.SetInt32("currentUserID", currentUserID);
                whoIsLogged = MainUser.GetUserById(currentUserID).user.getAccountType();
                Console.WriteLine(whoIsLogged);
                ViewBag.IsLoggedIn = whoIsLogged;
                ViewBag.CurrentUserID = currentUserID;
                // ViewBag.IsLoggedIn = true;
                Console.WriteLine("ZALOGOWANO");
                
                if (whoIsLogged == "Admin")
                {
                    var dataa = new { mess = "Welcome boss" };
                    // return RedirectToAction("AdminPanel");
                    return Json(dataa);
                }
                var data = new { mess = "Login correctly" };
                return Json(data);
            }
            else
            {
                Console.WriteLine("NIE ZALOGOWANO");
                var data = new { mess = "Incorrect data" };
                return Json(data);
               

            }
            return RedirectToAction("Index"); // Przekierowanie po zalogowaniu

        }

        [HttpPost]
        public IActionResult Register(string name, string surname, string email, string regname, string regpass, int age, string accountType, int skill)
        {
       if(MainUser.ValidateLogin(regname) && MainUser.ValidatePassword(regpass))
            {
                MainUser.AddNewUser(regname, regpass, name, surname, age, accountType, skill, email);
                //odrazu go zaloguj 
                Login(regname, regpass, false);
            }
            return RedirectToAction("Index"); // Przekierowanie po zarejestrowaniu
        }

        [HttpPost]
        public IActionResult CheckLogin(string regname)
        {
            Console.WriteLine("METODA CHECK LOGIN " + regname);
            bool isAvailable = false;
            if (!string.IsNullOrEmpty(regname))
            {
                isAvailable = (MainUser.ValidateLogin(regname));
                
            }
            else {
                isAvailable = false;
            }
            Console.WriteLine(isAvailable); 
            return Json(new { IsAvailable = isAvailable });

            return RedirectToAction("Index"); // Przekierowanie po zarejestrowaniu
        }

        [HttpGet]
        public IActionResult ViewThread()
        {
            // Retrieve entries for the specified threadId from your data source
            List<int> threadIds = Thread.GetThreadIds();
            List<Tuple<int, List<Post>>> threadDataList = new List<Tuple<int, List<Post>>>();

            foreach (int threadId in threadIds)
            {
                List<Post> posts = Post.GetPostsByThreadId(threadId);
                Tuple<int, List<Post>> threadData = new Tuple<int, List<Post>>(threadId, posts);
                threadDataList.Add(threadData);
            }

            // You may need to pass the entries to the view
            return View(threadDataList);
        }

        [HttpPost]
        public IActionResult AddThread(string newThreadTheme, string threadContent)
        {
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            DateTime creationDate = DateTime.Now;

            (String msg, bool ifWorked, int threadId) = Thread.AddNewThread(newThreadTheme, threadContent, creationDate, currentUserID);
            return RedirectToAction("Forum");
        }

        [HttpPost]
        public IActionResult AddImage(IFormFile fileInput)
        {        
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            if (fileInput != null && fileInput.Length > 0)
            {
                try
                {
                    string uploadPath = "wwwroot\\images";
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileInput.FileName;

                    string filePath = Path.Combine(uploadPath, uniqueFileName);
                    Console.WriteLine("up: " + uploadPath + " uni: " + uniqueFileName + " file path: " + filePath);
                    Console.WriteLine(MainUser.EditAccount(currentUserID, "image", "/images/" + uniqueFileName).message);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        fileInput.CopyTo(fileStream);
                       

                    }                 
                    return Json(new { success = true, message = "Image uploaded successfully" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error uploading image: " + ex.Message });
                }
            
            
            }
            else
            {
                return Json(new { success = false, message = "No image selected for upload" });
            }
        }



        [HttpPost]
        public IActionResult addPositiveRatingForThread(int threadId)
        {
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            (String msg, bool ifWorked) = Thread.AddThreadRatings(1, threadId, currentUserID);

            return RedirectToAction("Forum");
        }

        [HttpPost]
        public IActionResult addNegativeRatingForThread(int threadId)
        {
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            (String msg, bool ifWorked) = Thread.AddThreadRatings(0, threadId, currentUserID);

            return RedirectToAction("Forum");
        }


        [HttpPost]
        public IActionResult AddPost(string newPostContent, int threadId)
        {
            try
            {
                currentUserID = _session.GetInt32("currentUserID") ?? 0;
                DateTime creationDate = DateTime.Now;

                Post.AddNewPost(newPostContent, creationDate, threadId, currentUserID);

                return RedirectToAction("Forum");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in adding a new post to the database: " + ex.ToString());
                // Handle the exception as needed

                return RedirectToAction("Forum");
            }
        }



        [HttpPost]
        public IActionResult AddEvent(string Event_name, DateTime Event_date, string Event_description, string Event_location, string Event_creator)
        {
            //dodawanie eventu 
            currentUserID = _session.GetInt32("currentUserID") ?? 0;
            
            var result = Event.AddNewEvet(Event_name, Event_date, Event_description, Event_location, currentUserID);

            
            if (result.boolean)
            {
                return RedirectToAction("Events");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public IActionResult LogOut() {
			Console.WriteLine("=+=+=++=+=+=++=+=+=+WYLOGOWANIE=+=+=++=+=+=++=+=+=+");
			currentUserID = 0;
            _session.SetInt32("currentUserID", currentUserID);
            
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult ForgotPassword(string user)
        {
            if (MainUser.GetUserIdByLogin(user) == -1)
            {
                // return RedirectToAction("Index");
                var data = new { mess = "No User Found" };
                return Json(data);
            }
            else
            {
                MainUser.RecoverPassword(user);
                Console.WriteLine("odzyskiwanie hasla" + user);
                //  return Json(new { NotFoundMessage = "Nie znaleziono użytkownika." });
                var data = new { mess = "A new password has been sent to an e-mail" };
                return Json(data);
            }
           // return RedirectToAction("Index");
        }


		public IActionResult Privacy()
        {
            



			Console.WriteLine("-----------------------");
            foreach (var i in Misc.GetUserStyle(1))
            {
                Console.WriteLine("Id: " + i.ID + " Nazwa: " + i.NAME);
            }
            Console.WriteLine("-----------------------");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       

    }
}
