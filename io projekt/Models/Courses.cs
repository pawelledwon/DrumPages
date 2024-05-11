using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Threading;

namespace io_projekt.Models
{
    public class Courses
    {


        public List<Course> courses = new List<Course>();
        public List<Course> Topcourses = new List<Course>();
        public List<Course> Usercourses = new List<Course>();
        
        public void connectToDataBase()
        {
            try
            {

                String connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine("tabela: ");
                    connection.Open();
                    String query = "select * from Kursy";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Course course = new Course();
                                course.setID(reader.GetInt32(0));
                                course.setTitle(reader.GetString(1));
                                course.setDescription(reader.GetString(2));
                                course.setAuthorID(reader.GetInt32(3));
                                course.setDifficulty(reader.GetInt32(4));
                                course.setRating(reader.GetDouble(5));

                                courses.Add(course);
                            }
                        }
                    }

                    query = "select * from Kursy order by ocena desc";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Course course = new Course();
                                course.setID(reader.GetInt32(0));
                                course.setTitle(reader.GetString(1));
                                course.setDescription(reader.GetString(2));
                                course.setAuthorID(reader.GetInt32(3));
                                course.setDifficulty(reader.GetInt32(4));
                                course.setRating(reader.GetDouble(5));

                                Topcourses.Add(course);
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


 public void loadUserCourses(int userId)
 {
     try
     {

         String connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
             Console.WriteLine("tabela: ");
             connection.Open();
             String query = "select * from Kursy where autorId =" + userId;

             using (SqlCommand command = new SqlCommand(query, connection))
             {
                 using (SqlDataReader reader = command.ExecuteReader())
                 {

                     while (reader.Read())
                     {
                         Course course = new Course();
                         course.setID(reader.GetInt32(0));
                         course.setTitle(reader.GetString(1));
                         course.setDescription(reader.GetString(2));
                         course.setAuthorID(reader.GetInt32(3));
                         course.setDifficulty(reader.GetInt32(4));
                         course.setRating(reader.GetDouble(5));

                         Usercourses.Add(course);
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



      
    }

    public class Course
    {
        private int id;
        private string title;
        private string description;
        private int authorID;
        private int difficutly;
        private double rating;
        private string authorName;


        public List<Lesson> lessons = new List<Lesson>();
        public List<Comment> comments = new List<Comment>();

        public Course()
        {
            title = "";
            description = "";
        }

        public Course(int courseid)
        {
            try
            {
                String connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine("tabela: ");
                    connection.Open();
                    String query = "select * from Kursy where kursId =" + courseid;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                id = reader.GetInt32(0);
                                title = reader.GetString(1);
                                description = reader.GetString(2);
                                authorID = reader.GetInt32(3);
                                difficutly = reader.GetInt32(4);
                                rating = reader.GetDouble(5);
                            }
                        }
                    }


                    query = "select imie, nazwisko from Uzytkownicy where uzytkownikId = " + authorID;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                               String name = reader.GetString(0);
                                String surname = reader.GetString(1);
                                authorName = name + " " + surname;
                            }
                        }
                    }

                    query = "select * from Lekcje where kursId = " + courseid;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Lesson les = new Lesson();
                                les.setID(reader.GetInt32(0));
                                les.setTitle(reader.GetString(1));
                            

                                lessons.Add(les);

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

        
  public void writeToDB()
  {
      try
      {
          String connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";
          // SQL query to insert a new row into the Kursy table
          string query = "INSERT INTO Kursy (tytul, opis, autorId, trudnosc, ocena) VALUES (@tytul, @opis, @autorId, @trudnosc, @ocena)";

          // Create and open a connection to the database
          using (SqlConnection connection = new SqlConnection(connectionString))
          {
              connection.Open();

              // Create a SqlCommand object with the query and connection
              using (SqlCommand command = new SqlCommand(query, connection))
              {
                  // Add parameters to the SqlCommand object
                  command.Parameters.AddWithValue("@tytul", title);
                  command.Parameters.AddWithValue("@opis", description);
                  command.Parameters.AddWithValue("@autorId", authorID);
                  command.Parameters.AddWithValue("@trudnosc", difficutly);
                  command.Parameters.AddWithValue("@ocena", rating);

                  // Execute the query
                  command.ExecuteNonQuery();
              }
          }
      }

      catch (Exception ex)
      {
          Console.WriteLine(ex.Message);
      }
  }
        public int getID() { return id; }
        public string getTitle() { return title; }
        public string getDescription() { return description; }
        public string getAuthorName() { return authorName; }
        public int getAuthorID() { return authorID; }
        public int getDifficulty() { return difficutly; }
        public double getRating() { return rating; }

        public void setID(int id) { this.id = id; }
        public void setTitle(string title) { this.title = title; }
        public void setDescription(string description) { this.description = description; }
        public void setAuthorID(int authorID) { this.authorID = authorID; }

        public void setAuthorName(String authorName) { this.authorName = authorName; }
        public void setDifficulty(int difficulty) { this.difficutly = difficulty; }
        public void setRating(double rating) { this.rating = rating; }

    }



    public class Comment
    {
        private int id;
        private int classID;
        private int authorID;
        private int courseID;
        private string content;
        private int rating;

        public Comment()
        {
            content = "";
        }

        public int getID() { return id; }
        public int getClassID() { return classID; }
        public int getAuthorID() { return authorID; }
        public int getCourseID() { return courseID; }
        public string getContent() { return content; }
        public int getRating() { return rating; }

        public void setID(int id) { this.id = id; }
        public void setClassID(int classID) { this.classID = classID; }
        public void setAuthorID(int authorID) { this.authorID = authorID; }
        public void setCourseID(int courseID) { this.courseID = courseID; }
        public void setContent(string content) { this.content = content; }
        public void setRating(int rating) { this.rating = rating; }

    }
}
