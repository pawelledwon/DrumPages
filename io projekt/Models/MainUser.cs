
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;
using System.Net.Security;
using System.Reflection.Metadata.Ecma335;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;


namespace io_projekt.Models
{
    public class MainUser
    {
        private int id;
        private string login;
        private string password;
        private string name;

        private string lastName;
        private int age;
        private string accountType;
        private int skills;
        private string email;

        private string image;

        private const string connectionString = "Data Source=(local)\\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=False";
        private static IMemoryCache _cache; // Pole statyczne przechowujące pamięć podręczną
        private static int maxId;
        // Konstruktor prywatny, aby zapobiec utworzeniu wielu instancji tej klasy
        private MainUser() { }

        private MainUser(int id, string login, string password, string name, string lastName, int age, string type, int skills, string email, string image = "/images/default.png") { 
            this.id = id;
            this.login = login;     
            this.password = password;
            this.name = name;
            this.lastName = lastName;
            this.age = age;
            this.accountType = type;
            this.skills = skills;
            this.email = email;
            if (image == "zdj")
            {
                this.image = "/images/default.png";

			}
            else
            {
				this.image = image;
			}
           
        }

        
        public static (string message, int id) GetIdFromLogin(string login)
        {
            List<MainUser> users = GetAllUsers();
            int index = users.FindIndex(user => user.login == login);
            if (index != -1)
            {
                return (Constants.getUserIdSucces,users[index].id);
            }
            else
            {
                return (Constants.getUserIdError, -1);
            }

        }
        public int getId() 
        {    
            return id; 
        }   
        public string getLogin()
        {
            return login;
        }
        public string getPassword()
        {
            return password;
        }
        public string getName()
        {
        return name;    
        }
        public string getLastName()
        {
            return lastName;
        }
        public int getAge()
        {
            return age;
        }
        public string getAccountType() 
        {
            return accountType;
        }
        public int getSkills()
        {
            return skills;
        }
        public string getEmial()
        {
            return email;
        }
        public string getImage() {

            Console.WriteLine("TO jest zdjecie: " + image);
            return image;
        }


        public static IMemoryCache GetCacheInstance()
        {
            if (_cache == null)
            {             
                _cache = new MemoryCache(new MemoryCacheOptions());
            }
            return _cache;
        }

   
        public static (MainUser ?user, String message) GetUserById(int id)
        {
        if(id == 0)
        {
                MainUser a = new MainUser(0, "deletedUser", "1", "deletedUser", "deletedUser", 0, "deletedUser", 0, "deletedUser");
                return (a, "Deleted User");
		}


            //Mozna tak czytac 
            //---------
           // List<MainUser> users = GetAllUsers();
           // int index = users.FindIndex(user => user.id == id);
           // if (index == -1)
           // {
           //     return (null, Constants.emptyTable);
           // }
           // return (users[index], Constants.getUserSucces);
            //--------- I WTEDY CALA RESZTA NIEPOTRZEBA 

            //LUB TAK JAK TERAZ - NA BIERZACO Z BAZY CZYTAC DANEGO USERA 
            
            //IMemoryCache cache = GetCacheInstance();
            //if (!cache.TryGetValue("AllUsers", out List<MainUser> cachedUsers)) //jezeli nie pobrano z pamieci do odczytaj z bazy
            //{
               try
               {                  
                 using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string queryString = $"SELECT * FROM master.dbo.Uzytkownicy WHERE uzytkownikId = {id}";
                        using (SqlCommand command = new SqlCommand(queryString, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                               {
            //                        cachedUsers = new List<MainUser>();
            //                        //Wczytanie danych z bazy 
                                    int dataId = reader.GetInt32(0);
                                    string dataLogin = reader.GetString(1);
                                    string dataPassword = reader.GetString(2);
                                    string dataName = reader.GetString(3);
                                    string dataLastName = reader.GetString(4);
                                    int dataAge = reader.GetInt32(5);
                                    string dataAccountType = reader.GetString(6);
                                    int dataSkills = reader.GetInt32(7);
                                    string dataEmial = reader.GetString(8);
                                    string dataImage = reader.GetString(9);
                                    //stworzenie noego obiekty typu user i wpisanie go do cache
                             MainUser newUser = new MainUser(dataId, dataLogin, dataPassword, dataName, dataLastName, dataAge, dataAccountType, dataSkills, dataEmial, dataImage);
                            //                        cachedUsers.Add(new MainUser(dataId, dataLogin, dataPassword, dataName, dataLastName, dataAge, dataAccountType, dataSkills));
                            //                        var cacheEntryOptions = new MemoryCacheEntryOptions
                            //                        {
                            //                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) //zapisanie usera na 10 min
                            //                        };
                            //                        cache.Set("AllUsers", cachedUsers, cacheEntryOptions);
                            //    return (cachedUsers.Last(), Constants.getUserSucces);
                            return (newUser, Constants.getUserSucces);
                                               }
                                               else 
                                               {
                                                   return (null,Constants.emptyTable);
                                               }
                                           }
                                       }      
                                   }
                               }
                               catch (Exception ex)
                               {    
                                   return (null, Constants.dataBaseException + " " + ex.ToString());
                               }
                            //}
                            //MainUser cachedUser = cachedUsers.FirstOrDefault(u => u.id == id);
                            //if (cachedUser != null)
                            //{
                            //    return (cachedUser, Constants.getUserSucces);
                            //}
                            //else
                            //{
                            //    // var cache2 = MainUser.GetCacheInstance();
                            //    //cache2.Remove("AllUsers");
                            //    //return GetUserById(id);
                            //    return (null, Constants.emptyTable);
                            //}
                        }

        public static List<MainUser> GetAllUsers()
        {
            IMemoryCache cache = GetCacheInstance();
            if (!cache.TryGetValue("AllUsers", out List<MainUser> cachedUsers))
            {
               
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string queryString = $"SELECT * FROM master.dbo.Uzytkownicy";
                        using (SqlCommand command = new SqlCommand(queryString, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                cachedUsers = new List<MainUser>();
                                while (reader.Read())
                                {
                                    //Wczytanie danych z bazy 
                                    int dataId = reader.GetInt32(0);
                                    string dataLogin = reader.GetString(1);
                                    string dataPassword = reader.GetString(2);
                                    string dataName = reader.GetString(3);
                                    string dataLastName = reader.GetString(4);
                                    int dataAge = reader.GetInt32(5);
                                    string dataAccountType = reader.GetString(6);
                                    int dataSkills = reader.GetInt32(7);
                                    string dataEmail = reader.GetString(8);
                                    string dataImage = reader.GetString(9);
                                    Console.WriteLine("Imie:  " + dataName);
                                    //stworzenie nowego obiektu typu user i wpisanie go do cache
                                    cachedUsers.Add(new MainUser(dataId, dataLogin, dataPassword, dataName, dataLastName, dataAge, dataAccountType, dataSkills, dataEmail, dataImage));
                                    var cacheEntryOptions = new MemoryCacheEntryOptions
                                    {
                                       AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) //uzytkownikow do pamieci na 10 min
                                        //AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                                    };
                                    cache.Set("AllUsers", cachedUsers, cacheEntryOptions);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in wrting from users data base: " + ex.ToString());
                    return cachedUsers;
                }
            }
            return cachedUsers;
        }


  

        public static (string message, bool boolean) AddNewUser(string login, string password, string name, string lastName, int age, string type, int skills, string email, string image = "/images/default.png")
        {
            if (ValidateLogin(login) && ValidatePassword(password))
            {
                try
                {
                    int newUserId = GetMaxUserId() + 1;
                    if(newUserId != -1)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "INSERT INTO master.dbo.Uzytkownicy (login, haslo, imie, nazwisko, wiek, rodzajKonta, umiejetnosci, email, image) VALUES (@login, @password,@name,@lastName,@age,@type,@skills,@email,@image)";
                            SqlCommand command = new SqlCommand(query, connection);

                            command.Parameters.AddWithValue("@login", login);
                            command.Parameters.AddWithValue("@password", password);
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@lastName", lastName);
                            command.Parameters.AddWithValue("@age", age);
                            command.Parameters.AddWithValue("@type", type);
                            command.Parameters.AddWithValue("@skills", skills);
                            command.Parameters.AddWithValue("@email", email);
							command.Parameters.AddWithValue("@image", image);
							command.ExecuteNonQuery();
                        }

                        IMemoryCache cache = GetCacheInstance();
                        List<MainUser> usersFromCache = cache.Get<List<MainUser>>("AllUsers");
                        if (usersFromCache == null)
                        {
                            usersFromCache = new List<MainUser>();
                        }

                        usersFromCache.Add(new MainUser(newUserId, login, password, name, lastName, age, type, skills,email,image));
                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) //zapisanie usera na 10 min
                        };
                        cache.Set("AllUsers", usersFromCache, cacheEntryOptions);
                        return (Constants.addNewUserSucces,true);
                    }
                    else 
                    { 
                        return (Constants.addNewUserError,false); 
                    }
                }
                catch (Exception ex)
                {
                    return (Constants.dataBaseException + ": " + ex.Message,false);
                    
                } 
            }
            else
            {

                return (Constants.addNewUserError + ": " + Constants.badLoginPassword, false);
            }
        }


        //Czy login jest poprawny- czy zajety, dlugosc
        public static bool ValidateLogin(string login) 
        {
            var users = GetAllUsers();
            //TODO
            //Bardziej konkretne wymagania co do loginu- teraz tylko dlogosc i powtarzanie 
            if (login.Length >= 3 && login.Length <= 50)
            {
                if (users.Any(user => user.getLogin() == login)) //GetAllUsers().Any(user => user.getlogin() == login)
                {
                    Console.WriteLine($"Znaleziono powtórzenie dla loginu: {login}");
                    return false;
                }
                else
                {
                    
                    Console.WriteLine("lOGIN jest wolny");
                    return true; //jezeli login jest zajety
                }               
            }        
            else
            {
                Console.WriteLine("zla dlugosc");
                return false;
            }

     
        }

        public static int GetUserIdByLogin(string login)
        {
            int userId = -1; // Domyślna wartość, jeśli nie znajdzie użytkownika

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"SELECT uzytkownikId FROM master.dbo.Uzytkownicy WHERE login = @login";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@login", login);

                    // Wykonanie zapytania i odczytanie ID użytkownika
                    object result = command.ExecuteScalar();

                    // Sprawdzenie czy wynik zapytania jest niepusty i jest liczbą całkowitą
                    if (result != null && result != DBNull.Value && int.TryParse(result.ToString(), out int id))
                    {
                        userId = id;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas pobierania ID użytkownika: " + ex.Message);
            }
            return userId;
        }


        public static int GetUserIdByMail(string email)
        {
            int userId = -1; // Domyślna wartość, jeśli nie znajdzie użytkownika

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"SELECT uzytkownikId FROM master.dbo.Uzytkownicy WHERE email = @email";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@email", email);

                    // Wykonanie zapytania i odczytanie ID użytkownika
                    object result = command.ExecuteScalar();

                    // Sprawdzenie czy wynik zapytania jest niepusty i jest liczbą całkowitą
                    if (result != null && result != DBNull.Value && int.TryParse(result.ToString(), out int id))
                    {
                        userId = id;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas pobierania maila użytkownika: " + ex.Message);
            }
            return userId;
        }



        private static bool updateQuery(int userId, string toUpdate ,string newValue)
        {
            Console.WriteLine("EDYCJA USERA!!!");
            //aktualizacja pamieci
            GetAllUsers();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE master.dbo.Uzytkownicy SET {toUpdate} = @newValue WHERE uzytkownikId = @userId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@newValue", newValue);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                    IMemoryCache cache = GetCacheInstance();
                    List<MainUser> usersFromCache = cache.Get<List<MainUser>>("AllUsers");
                    if (usersFromCache == null)
                    {
                        usersFromCache = new List<MainUser>();
                    }
                    MainUser userToUpdate = usersFromCache.Find(user => user.id == userId);
                    if (userToUpdate != null)
                    {
                        //tu case dla kazdego przypadku 
                        switch (toUpdate)
                        {
                            case "login":
                                userToUpdate.login = newValue;
                                break;
                            case "haslo":
                                userToUpdate.password = newValue;
                                break;
                            case "imie":
                                userToUpdate.name = newValue;
                                break;
                            case "wiek":
                                userToUpdate.age = int.Parse(newValue);
                                break;
                            case "umiejetnosci":
                                userToUpdate.skills = int.Parse(newValue);
                                break;
							case "nazwisko":
								userToUpdate.lastName = newValue;
								break;
							case "rodzajKonta":
                                userToUpdate.accountType = newValue;
                                break;
                            case "email":
                                userToUpdate.email = newValue;
                                break;
							case "image":
                                Console.WriteLine("DODANIE ZDJECIA!!!");
                                userToUpdate.image = newValue;
								break;
						}
                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                        };
                        cache.Set("AllUsers", usersFromCache, cacheEntryOptions);
						return (true);
                    }
                    return (true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error");
                return ( false);
            }
        }

        /// <summary>
        /// Updates user info- can update account type - so before check if admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="what">What to update: login or haslo or imie, wiek, umiejetnosci, rodzajKonta</param>
        /// <param name="newValue">new value- always as string</param>
        /// <returns></returns>
        public static (string message, bool boolean) EditAccount(int id, string what, string newValue)
        {
            if (UserExists(id))
            {
                if (updateQuery(id, what, newValue))
                {
                    return ("gotowe", true);
                }
                    return ("blad edycji", false);     
            }
                return ("nie dziala", false); 
        }

        public static (string message, bool booelean) DeleteAcoount(int userId)
        {
            if (UserExists(userId))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM master.dbo.Uzytkownicy WHERE uzytkownikId = @userId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.ExecuteNonQuery();

                        IMemoryCache cache = GetCacheInstance();
                        List<MainUser> usersFromCache = cache.Get<List<MainUser>>("AllUsers");
                        if (usersFromCache != null)
                        {
                            MainUser userToRemove = usersFromCache.FirstOrDefault(u => u.id == userId);
                            if (userToRemove != null)
                            {
                                usersFromCache.Remove(userToRemove);
                                cache.Set("AllUsers", usersFromCache);
                                return ("Usunieto usera", true);
                            }
                        }
                        return ("blad kasowania", false);
                    }
                }
                catch (Exception ex)
                {
                    return ($"kasowanie: {ex.Message}", false);
                }
            }
            return ("brak usera- kasowanie", false);
        }
        //Czy haslo jest poprawne - regex
        public static bool ValidatePassword(string password)
        {
            if (password.Length >= 5 && password.Length <= 50 && password.Any(char.IsDigit) && password.Any(c => !char.IsLetterOrDigit(c)))
            {
                Console.WriteLine("HASLO OK");
                return true;
            }
            else
            {
                return false;
            }
            
        }

       // //czy user jest w bazie- zwraca id-> do logowania można wykorzystac metodę GetUserIdByLogin
       // public int findUser(string login) 
       // {        
       //     return 1;
       // }

        public static bool UserExists(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM master.dbo.Uzytkownicy WHERE uzytkownikId = @id";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@id", id);
                    int userCount = (int)checkCommand.ExecuteScalar();
                    return userCount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd w funkcji UserExists: " + ex.Message);
                return false;
            }
        }


        //Czy podane podczas logowania hasło jest poprawne 
        public static bool CheckPassword(string login,string password)
        {
            int id = GetUserIdByLogin(login);
            if (id != -1) //user found
            {
                if (GetUserById(id).user.getPassword() == password) //correct password
                {
                    return true;
                }
                else //bad password
                { 
                return false;
                }
            }
            else
            {
                return false; //no user found
            }

        }


        public static int GetMaxUserId()
        {
            int maxUserId = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string queryString = "SELECT MAX(uzytkownikId) FROM master.dbo.Uzytkownicy";

                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        object result = command.ExecuteScalar();

                        if (result != DBNull.Value)
                        {
                            maxUserId = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas pobierania maksymalnej wartości uzytkownikId: " + ex.Message);
            }
            maxId = maxUserId;  
            return maxUserId;
        }

        //Narazie wstepnie zrobione - brak wiekszych zabezpieczen - oraz tylko wysyłanie na gmail
        public static bool RecoverPassword(string login)
        {
            var userId = GetUserIdByLogin(login);
            var user = GetUserById(userId).user;
            string email =user.getEmial();

            //Pagedrum123 - haslo
           
            string newPassword = genereteNewPassword();
            Console.WriteLine("odzyskiwanie:" + newPassword);
            string to = email; //To address              
            string from = "pagesdrum5@gmail.com"; //From address
            
            MailMessage message = new MailMessage(from, to);

            string mailbody = "Witaj " + user.getName() + ", twoje hasło zostalo zresetowane.  Oto nowe hasło:   " + newPassword;
            message.Subject = "DrumPages - odzyskiwanie hasla";
            message.Body = mailbody;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new
            System.Net.NetworkCredential("pagesdrum5@gmail.com", "sqmr xadv tgcs ejaj");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
               client.Send(message);
               Console.WriteLine("git");
               EditAccount(userId,"haslo" , newPassword);                         
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }


        private static string genereteNewPassword()
        {           
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static List<MainUser> searchUsersByNameOrSurname(string data) {
            List<MainUser> allUsers = GetAllUsers();
            List<MainUser> matchedUsers = new List<MainUser>();
            foreach (var user in allUsers)
            {
                if (user.getName().Contains(data)) {
                    matchedUsers.Add(user);
                } else if (user.getLastName().Contains(data)) {
                    matchedUsers.Add(user);
                }

            }
            return matchedUsers;
        }

    }
}
