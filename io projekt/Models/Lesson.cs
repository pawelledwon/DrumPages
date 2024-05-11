using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Threading;




namespace io_projekt.Models
{
    public class Lesson : PageModel
    {

        private int id;
        private string title;
        private string content;
        private int courseID;
        private string videoURL;
        private int rating;


        public Lesson()
        {

        }
            public Lesson(int classid) 
        {
            title = "";
            content = "";
            videoURL = "";
			id = classid;
           


                try
				{

					String connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";

					using (SqlConnection connection = new SqlConnection(connectionString))
					{
						Console.WriteLine("tabela: ");
						connection.Open();
						String query = "select * from Lekcje where lekcjaId=" + this.id;

						using (SqlCommand command = new SqlCommand(query, connection))
						{
							using (SqlDataReader reader = command.ExecuteReader())
							{

								while (reader.Read())
								{

                                this.content = reader.GetString(2);
                                    this.videoURL = reader.GetString(4);

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
                string query = "INSERT INTO Lekcje (tytul, tresc, kursId, wideoURL, ocena) VALUES (@tytul, @tresc, @kursId, @wideoURL, @ocena)";

                // Create and open a connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SqlCommand object with the query and connection
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the SqlCommand object
                        command.Parameters.AddWithValue("@tytul", title);
                        command.Parameters.AddWithValue("@tresc", content);
                        command.Parameters.AddWithValue("@kursId", courseID);
                        command.Parameters.AddWithValue("@wideoURL", videoURL);
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
        public string getContent() { return content; }
        public int getCourseID() { return courseID; }
        public string getVideoURL() { return videoURL; }
        public int getRating() { return rating; }

        public void setID(int id) { this.id = id; }
        public void setTitle(string title) { this.title = title; }
        public void setContent(string content) { this.content = content; }
        public void setCourseID(int courseID) { this.courseID = courseID; }
        public void setVideoURL(string url) { this.videoURL = url; }
        public void setRating(int rating) { this.rating = rating; }

    }
}

