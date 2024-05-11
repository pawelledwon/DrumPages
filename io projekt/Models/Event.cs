using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace io_projekt.Models
{
    public class Event
    {
        private int id; 
        private string name;
        private DateTime date; 
        private string description;
        private string place;
        private int organizorId;

        private const string connectionString = "Data Source=(local)\\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=False";
        private static IMemoryCache _cache; // Pole statyczne przechowujące pamięć podręczną

        private Event(int id, string name, DateTime date, string description, string place, int organizorId)
        { 
        this.id = id;
        this.name = name;
        this.date = date;
        this.description = description;
        this.place = place;
        this.organizorId = organizorId;    
        }

        public string getName()
        {
            return name;
        }
        public DateTime getDate()
        {
            return date;
        }
        public string getDescription()
        {
            return description;
        }
        public string getPlace()
        {
                return place;
        }
        public int getId() 
        {
        return id;
        }
        public int getOrganizorId()
        {
            return organizorId;
        }
        private static IMemoryCache GetCacheInstance()
        {
            if (_cache == null)
            {
                _cache = new MemoryCache(new MemoryCacheOptions());
            }
            return _cache;
        }

        public static bool EventExists(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM master.dbo.Wydarzenia WHERE idWydarzenia = @id";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@id", id);
                    int userCount = (int)checkCommand.ExecuteScalar();
                    return userCount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd w funkcji EventExists: " + ex.Message);
                return false;
            }
        }


        public static (Event? evnt, string message) GetEventById(int id)
        {
            IMemoryCache cache = GetCacheInstance();
            if (!cache.TryGetValue("AllEvents", out List<Event> cachedEvents))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string queryString = $"SELECT * FROM master.dbo.Wydarzenia WHERE idWydarzenia = {id}";
                        using (SqlCommand command = new SqlCommand(queryString, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    cachedEvents = new List<Event>();
                                    //Wczytanie danych z bazy 
                                    int dataId = reader.GetInt32(0);
                                    string dataName = reader.GetString(1);
                                    DateTime dataDate = reader.GetDateTime(2);
                                    string dataDescription = reader.GetString(3);
                                    string dataPlace = reader.GetString(4);
                                    int dataOrganizorId = reader.GetInt32(5);
                                    Console.WriteLine("pierwsze odczytanie " + dataName);
                                    //stworzenie noego obiekty typu user i wpisanie go do cache
                                    cachedEvents.Add(new Event(dataId, dataName, dataDate, dataDescription, dataPlace, dataOrganizorId)); 
                                    var cacheEntryOptions = new MemoryCacheEntryOptions
                                    {
                                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) //zapisanie usera na 10 min
                                    };
                                    cache.Set("AllEvents", cachedEvents, cacheEntryOptions);
                                    return (cachedEvents.Last(), Constants.GetEventSucces);
                                }
                                else
                                {
                                    return (null, Constants.emptyTable);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return (null, Constants.dataBaseException + " " + ex.ToString());
                }
                
            }
            Event cachedEvent = cachedEvents.FirstOrDefault(e => e.id == id);
            return (cachedEvent, Constants.GetEventSucces);
           
        }

        public static List<Event> GetAllEvents()
        {
            IMemoryCache cache = GetCacheInstance();
            if (!cache.TryGetValue("AllEvents", out List<Event> cachedEvents))
            {

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string queryString = $"SELECT * FROM master.dbo.Wydarzenia";
                        using (SqlCommand command = new SqlCommand(queryString, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                cachedEvents = new List<Event>();
                                while (reader.Read())
                                {
                                    //Wczytanie danych z bazy 
                                    int dataId = reader.GetInt32(0);
                                    string dataName = reader.GetString(1);
                                    DateTime dataDate = reader.GetDateTime(2);
                                    string dataDescription = reader.GetString(3);
                                    string dataPlace = reader.GetString(4);
                                    int dataOrganizorId = reader.GetInt32(5);
                                    Console.WriteLine("Imie:  " + dataName);
                                    //stworzenie nowego obiektu typu user i wpisanie go do cache
                                    cachedEvents.Add(new Event(dataId, dataName, dataDate, dataDescription, dataPlace, dataOrganizorId));
                                    var cacheEntryOptions = new MemoryCacheEntryOptions
                                    {
                                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) //uzytkownikow do pamieci na 10 min
                                        //AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                                    };
                                    cache.Set("AllEvents", cachedEvents, cacheEntryOptions);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in wrting from events data base: " + ex.ToString());
                    return cachedEvents;
                }
            }
            return cachedEvents;
        }

        public static int GetEventIdByName(string name)
        {
            List<Event> events = GetAllEvents();
            int index = events.FindIndex(e => e.name == name);
            if (index != -1)
            {
                return events[index].id;
            }
            else
            { 
                return -1;
            }
            
        }

        public static int GetMaxEventId()
        {
            int maxEventId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string queryString = "SELECT MAX(idWydarzenia) FROM master.dbo.Wydarzenia";

                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        object result = command.ExecuteScalar();

                        if (result != DBNull.Value)
                        {
                            maxEventId = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas pobierania maksymalnej wartości eventId: " + ex.Message);
            }
            return maxEventId;
        }

        public static (string message, bool boolean) AddNewEvet(string name, DateTime date, string description, string place, int organizorId)
        {
         MainUser ?organizor = MainUser.GetUserById(organizorId).user;
            if (organizor == null)
            {
                return (Constants.noUserFound, false);
            }
            else
            {
                try
                {
                    int id = GetMaxEventId() + 1;
                    if (id != -1)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "INSERT INTO master.dbo.Wydarzenia (nazwa, data, opis, lokalizacja, organizatorId) VALUES (@name, @date,@description,@place,@organizorId)";
                            SqlCommand command = new SqlCommand(query, connection);

                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@date", date);
                            command.Parameters.AddWithValue("@description", description);
                            command.Parameters.AddWithValue("@place", place);
                            command.Parameters.AddWithValue("@organizorId", organizorId);
                            command.ExecuteNonQuery();
                        }
                        IMemoryCache cache = GetCacheInstance();
                        List<Event> eventsFromCache = cache.Get<List<Event>>("AllEvents");
                        if (eventsFromCache == null)
                        {
                            eventsFromCache = new List<Event>();
                        }

                        eventsFromCache.Add(new Event(id, name, date, description, place, organizorId));
                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) //zapisanie eventa na 10 min
                        };
                        cache.Set("AllEvents", eventsFromCache, cacheEntryOptions);
                        return (Constants.addNewEventSucces, true);
                    }
                    else
                    {
                        return (Constants.addNewEventError, false); 
                    }
                }
                catch (Exception ex)
                {
                    return (Constants.addNewEventError + ": " + ex.Message, false); 
                }
            }

        }

        public static (string message, bool boolean) RemoveEvent(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM master.dbo.Wydarzenia WHERE idWydarzenia = @id";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@id", id);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        IMemoryCache cache = GetCacheInstance();
                        List<Event> eventsFromCache = cache.Get<List<Event>>("AllEvents");
                        if (eventsFromCache != null)
                        {
                            eventsFromCache.RemoveAll(ev => ev.id == id);
                            cache.Set("AllEvents", eventsFromCache);
                        }

                        return (Constants.RemoveEventSucces, true);
                    }
                    else 
                    {
                        return (Constants.RemoveEventError, false);
                    }
                    
                }                
            }
            catch(Exception ex)
            {
                return (Constants.RemoveEventError +": " + ex.Message, false);
            }
        }
        private static bool updateQuery(int eventId, string toUpdate, string newValue)
        {
            //zaktualizowanie pamieci cache 
            GetAllEvents();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE master.dbo.Wydarzenia SET {toUpdate} = @newValue WHERE idWydarzenia = @eventId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@newValue", newValue);
                    command.Parameters.AddWithValue("@eventId", eventId);
                    command.ExecuteNonQuery();
                    IMemoryCache cache = GetCacheInstance();
                    List<Event> eventsFromCache = cache.Get<List<Event>>("AllEvents");
                    if (eventsFromCache == null)
                    {
                        eventsFromCache = new List<Event>();
                    }
                    Event eventToUpdate = eventsFromCache.Find(e => e.id == eventId);
                    if (eventToUpdate != null)
                    {
                        //tu case dla kazdego przypadku 
                        switch (toUpdate)
                        {
                            case "nazwa":
                                eventToUpdate.name = newValue;
                                break;
                            case "data":
                                eventToUpdate.date = DateTime.Parse(newValue);
                                break;
                            case "opis":
                                eventToUpdate.description = newValue;
                                break;
                            case "lokalizacja":
                                eventToUpdate.place = newValue;
                                break;                          
                            case "organizatorId":
                                if (MainUser.UserExists(int.Parse(newValue)))
                                {
                                    eventToUpdate.organizorId = int.Parse(newValue);
                                }
                                else 
                                {
                                    return (false);
                                }
                                break;

                        }
                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                        };
                        cache.Set("AllEvents", eventsFromCache, cacheEntryOptions);
                        return (true);
                    }
                    return (true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error");
                return (false);
            }
        }


        /// <summary>
        /// Updates event info
        /// </summary>
        /// <param name="id">Event's id to update</param>
        /// <param name="what">What to update: nazwa, data, opis, lokalizacja, organizatorId</param>
        /// <param name="newValue">new value- always as string</param>
        /// <returns></returns>
        public static (string message, bool boolean) EditEvent(int id, string what, string newValue)
        {
            if (EventExists(id))
            {
                if (updateQuery(id, what, newValue))
                {
                    return ("gotowe", true);
                }
                return ("blad edycji", false);
            }
            return ("nie dziala", false);
        }


    }
}
