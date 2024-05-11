//Klasa na rozne roznosci 




using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace io_projekt.Models
{

    public struct Gear
    {
        public Gear(int id, string name)
        {
            ID = id;
            NAME = name;
        }
        public int ID { get; }
        public string NAME { get; }
    }

    public struct Style
    {
        public Style(int id, string name)
        {
            ID = id;
            NAME = name;
        }
        public int ID { get; }
        public string NAME { get; }
    }



    public static class Misc
    {
        private const string connectionString = "Data Source=(local)\\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=False";
        public static List<Gear> GetAllGear()
        {
            List<Gear> gearList = new List<Gear>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string queryString = $"SELECT * FROM master.dbo.Sprzet";
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {                       
                            while (reader.Read())
                            {
                                //Wczytanie danych z bazy 
                                int dataId = reader.GetInt32(0);
                                string dataName = reader.GetString(1);                             
                                gearList.Add(new Gear(dataId,dataName)); 
                            }
                            return (gearList);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return (null);
            }
        }

        public static bool hasAllSet(int id)
        {
            int gCount = GetUserGear(id).Count();
            int sCount = GetUserStyle(id).Count();
            if (gCount > 0 && sCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Style> GetAllStyles()
        {
            List<Style> styleList = new List<Style>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string queryString = $"SELECT * FROM master.dbo.Style";
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Wczytanie danych z bazy 
                                int dataId = reader.GetInt32(0);
                                string dataName = reader.GetString(1);
                                styleList.Add(new Style(dataId, dataName));
                            }
                            return (styleList);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return (null);
            }
        }

        public static Gear GetGearById(int idSprzetu)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string queryString = $"SELECT * FROM master.dbo.Sprzet WHERE idSprzetu = {idSprzetu}";
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int dataId = reader.GetInt32(0);
                                string dataName = reader.GetString(1);
                                return (new Gear(dataId, dataName));
                            }
                            return (new Gear(-1,"err"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                return (new Gear(-1,ex.Message));
            }           
        }

        public static Style GetStyleById(int idStylu)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string queryString = $"SELECT * FROM master.dbo.Style WHERE idStylu = {idStylu}";
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int dataId = reader.GetInt32(0);
                                string dataName = reader.GetString(1);
                                return (new Style(dataId, dataName));
                            }
                            return (new Style(-1, "err"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return (new Style(-1, ex.Message));
            }


        }

        public static List<Gear> GetUserGear(int userId) 
        {
            List<Gear> list = new List<Gear>();
            List<int> gearIds = new List<int>();

            if(!MainUser.UserExists(userId))
            {
                return list;

             }
            else {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string queryString = $"SELECT idSprzetu FROM master.dbo.SprzetyUzytkownik WHERE uzytkownikId = {userId}";
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                int dataId = reader.GetInt32(0);
                                gearIds.Add(dataId);
                            }

                        }
                    }
                }

                foreach (var a in gearIds)
                {
                   list.Add(GetGearById(a));
                }
            }
            catch (Exception ex) {
                list.Add(new Gear(-1, ex.Message));
                return list;
            
            }
              return list;
            }
        }

        public static List<Style> GetUserStyle(int userId)
        {

            List<Style> list = new List<Style>();
            List<int> styleIds = new List<int>();

            if (!MainUser.UserExists(userId))
            {
                return list;

            }
            else
            {

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string queryString = $"SELECT stylId FROM master.dbo.StyleUzytkownik WHERE uzytkownikId = {userId}";
                        using (SqlCommand command = new SqlCommand(queryString, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    int dataId = reader.GetInt32(0);
                                    styleIds.Add(dataId);
                                }

                            }
                        }
                    }

                    foreach (var a in styleIds)
                    {
                        list.Add(GetStyleById(a));
                    }
                }
                catch (Exception ex)
                {
                    list.Add(new Style(-1, ex.Message));
                    return list;

                }
                return list;
            }

        }

        public static bool GearExists(string gearName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM master.dbo.Sprzet WHERE nazwa = @gearName";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@gearName", gearName);
                    int gearCount = (int)checkCommand.ExecuteScalar();
                    return gearCount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd w funkcji GearExists: " + ex.Message);
                return false;
            }         
        }

        public static bool StyleExists(string styleName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM master.dbo.Style WHERE opis = @styleName";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@styleName", styleName);
                    int styleCount = (int)checkCommand.ExecuteScalar();
                    return styleCount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd w funkcji GearExists: " + ex.Message);
                return false;
            }
        }

        public static bool AddGear(string name) 
        {
            if (!GearExists(name))
            {
                try
                {               
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "INSERT INTO master.dbo.Sprzet (nazwa) VALUES (@name)";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@name", name);
                            command.ExecuteNonQuery();
                        }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Blad dodawania sprzetu: " +ex.Message);
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool AddStyle(string name) 
        {
            if (!StyleExists(name))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO master.dbo.Style (opis) VALUES (@name)";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@name", name);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Blad dodawania styli: " + ex.Message);
                    return false;
                }
                return true;
            }
            return false;
        }

        public static (string message, bool boolean) AddUserGear(int userId, int gearId)
        {
            List<Gear> gear = GetUserGear(userId);
            bool alreadyHave = gear.Exists(gear => gear.ID == gearId);
            if (alreadyHave)
            {
                return ("Sprzet przypisany wczesniej", false);
            }

            if (MainUser.UserExists(userId))
            {
                if (GetGearById(gearId).ID != -1)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "INSERT INTO master.dbo.SprzetyUzytkownik (uzytkownikId, idSprzetu) VALUES (@userId, @gearId)";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@userId", userId);
                            command.Parameters.AddWithValue("@gearId", gearId);
                            command.ExecuteNonQuery();
                            return ("Przypisano sprzet do uzytkownika", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Blad przypisania sprzetu do usera: " + ex.Message);
                        return ("Blad przypisania sprzetu do usera: " + ex.Message, false);
                    }
                }
                else
                {
                    return ("Bledne id sprzetu", false);
                }
            }
            else {
                return ("Blad: Przypisanie sprzetu do usera: " + Constants.noUserFound, false);
            }
        }

        public static (string message, bool boolean) AddUserStyle(int userId, int styleId)
        {
            List<Style> styles = GetUserStyle(userId);
            bool alreadyHave = styles.Exists(style => style.ID == styleId);
            if (alreadyHave)
            {
                return ("Styl przypisany wczesniej", false);
            }

            if (MainUser.UserExists(userId))
            {
                if (GetStyleById(styleId).ID != -1)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "INSERT INTO master.dbo.StyleUzytkownik (uzytkownikId, stylId) VALUES (@userId, @styleId)";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@userId", userId);
                            command.Parameters.AddWithValue("@styleId", styleId);
                            command.ExecuteNonQuery();
                            return ("Przypisano styl do uzytkownika", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Blad przypisania stylu do usera: " + ex.Message);
                        return ("Blad przypisania stylu do usera: " + ex.Message, false);
                    }
                }
                else
                {
                    return ("Bledne id stylu", false);
                }
            }
            else
            {
                return ("Blad: Przypisanie stylu do usera: " + Constants.noUserFound, false);
            }
        }


        /// <summary>
        /// Tylko dla ADMINA !! Usuwanie sprzetu z bazy Sprzety
        /// </summary>
        /// <param name="id">gear id to remove</param>
        public static (string message, bool result) RemoveGear(int id)
        {
            if ((GetGearById(id).ID == -1)) // no gear found
            {
                return ("Nie znaleziono sprzetu o podanym ID",false);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM master.dbo.Sprzet WHERE idSprzetu = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                    return ("Usunieto sprzet", true);
                    
 
                }
            }
            catch (Exception ex)
            {
                return ($"kasowanie sprzetu: {ex.Message}", false);
            }
        }


        /// <summary>
        /// Tylko dla ADMINA !!! - Usuwanie styli z bazy  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static (string message, bool result) RemoveStyle(int id)
        {
            if ((GetStyleById(id).ID == -1)) // no style found
            {
                return ("Nie znaleziono stylu o podanym ID", false);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM master.dbo.Style WHERE idStylu = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                    return ("Usunieto styl", true);


                }
            }
            catch (Exception ex)
            {
                return ($"kasowanie stylu: {ex.Message}", false);
            }
        }


        /// <summary>
        /// If gearId == -1 removes all gear !!
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gearId"></param>
        public static (string message, bool result) RemoveUserGear(int userId, int gearId)
        {
            if (!MainUser.UserExists(userId))
            {
                return (Constants.noUserFound, false);         
            }
            List<Gear> gear = GetUserGear(userId);

            bool haveIt = gear.Exists(gear => gear.ID == gearId);
            if (gearId == -1 && gear.Count > 0)
            {
                haveIt = true;
            }
            if (!haveIt)
            {
                return ("Uzytkownik nie ma przypisanego podanego sprztu",false);
            }

            //kasowanie 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (gearId == -1)
                    {
                        string query = "DELETE FROM master.dbo.SprzetyUzytkownik WHERE uzytkownikId = @userId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.ExecuteNonQuery();
                        return ("Usunieto caly sprzet", true);
                    }

                    else
                    {
                        string query = "DELETE FROM master.dbo.SprzetyUzytkownik WHERE uzytkownikId = @userId AND idSprzetu = @gearId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@gearId", gearId);
                        command.ExecuteNonQuery();
                        return ("Usunieto sprzet", true);
                    }
                                                            
                }
            }
            catch (Exception ex)
            {
                return ($"kasowanie: {ex.Message}", false);
            }
        }

        /// <summary>
        /// If stylId == -1 removes all styles !!
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        public static (string message, bool result) RemoveUserStyle(int userId, int styleId)
        {
            if (!MainUser.UserExists(userId))
            {
                return (Constants.noUserFound, false);
            }
            List<Style> style = GetUserStyle(userId);

            bool haveIt = style.Exists(style => style.ID == styleId);
            if (styleId == -1 && style.Count > 0)
            {
                haveIt = true;
            }
            if (!haveIt)
            {
                return ("Uzytkownik nie ma przypisanego stylu", false);
            }

            //kasowanie 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (styleId == -1)
                    {
                        string query = "DELETE FROM master.dbo.StyleUzytkownik WHERE uzytkownikId = @userId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.ExecuteNonQuery();
                        return ("Usunieto wszystkie style", true);
                    }

                    else
                    {
                        string query = "DELETE FROM master.dbo.StyleUzytkownik WHERE uzytkownikId = @userId AND stylId = @styleId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@styleId", styleId);
                        command.ExecuteNonQuery();
                        return ("Usunieto styl", true);
                    }

                }
            }
            catch (Exception ex)
            {
                return ($"kasowanie: {ex.Message}", false);
            }
        }


        public static bool EditGear(int gearId, string name)
        {
            if (GetGearById(gearId).ID == -1)
            {
                return false;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE master.dbo.Sprzet SET nazwa = @name WHERE idSprzetu = @gearId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@gearId", gearId);
                    command.ExecuteNonQuery();
                    return (true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error");
                return (false);
            }
        }

        public static bool EditStyle(int styleId, string name)
        {
            if (GetStyleById(styleId).ID == -1)
            {
                return false;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE master.dbo.Style SET opis = @name WHERE idStylu = @styleId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@styleId", styleId);
                    command.ExecuteNonQuery();
                    return (true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error");
                return (false);
            }
        }





        public static void EditUserGear(int userId, int gearId)
        { 

        }









    }
}
