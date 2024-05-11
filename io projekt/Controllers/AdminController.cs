using io_projekt.Models;
using io_projekt.Views.Home;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace io_projekt.Controllers
{
	public class AdminController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public IActionResult DeleteThread(int id)
		{
			//USUWANIE WATKU 
			Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-USUNIETO Watek-=-=-=-=-=-=-=-=-=-=-=-=-=-");
			Models.Thread.RemoveThread(id);
			
			return RedirectToAction("AdminPanel", "Home");
		}

		[HttpPost]
		public IActionResult EditThread(int threadId, string editAction, string t_name_input, int user_id_input)
		{
			//USUWANIE WATKU 
			Console.WriteLine("-+-+-+-++-+-+EDYTOWANIE WATKU+-+-+-+-++-+-+" + threadId +" :-:" + editAction + " :-:" + t_name_input +" :-: " +user_id_input);
			if (editAction == "author")
			{
				Models.Thread.updateQuery(threadId, "uzytkownikId", user_id_input.ToString());
			}
			else if (editAction == "t_name")
			{
				Models.Thread.updateQuery(threadId, "temat", t_name_input);
			}
			//Models.Thread.RemoveThread(id);

			return RedirectToAction("AdminPanel", "Home");
		}


		[HttpGet]
		public IActionResult testA(int threadId)
		{
			Console.WriteLine("AJAXXXXXXXXX");
			// Tutaj pobierz listę postów na podstawie threadId
			//tablica postow  
			List<Post> posts = Post.GetPostsByThreadId(threadId);
			//List<string> wpis = new List<string>();
			string[] com = posts.Select(c => c.getContent()).ToArray();
			Console.WriteLine(posts.Count());

			var data = posts.Select(post => new
			{	
				Content = post.getContent(),
				Id = post.getID(),
				Datum = post.getCreationDate(),
				Uid = post.getUserID()
				

			});	
			return Json(data);
		}


		[HttpPost]
		public IActionResult DeleteComment(int comId)
		{
			//USUWANIE WATKU 
			Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-USUNIETO KOMENTARZ NA FORUM-=-=-=-=-=-=-=-=-=-=-=-=-=-" + comId);
			var a =Post.RemovePost(comId);
			Console.WriteLine(a.message);
			return RedirectToAction("AdminPanel", "Home");
		}


		[HttpPost]
		public IActionResult DeleteUser(int id)
		{
			//USUWANIE WATKU 
			Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-USUNIETO Usera-=-=-=-=-=-=-=-=-=-=-=-=-=-" + id);
			MainUser.DeleteAcoount(id);

			return RedirectToAction("AdminPanel", "Home");
		}


		[HttpPost]
		public IActionResult SetToPro(int id)
		{
			//USUWANIE WATKU 
			Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-UPdate statusu Usera-=-=-=-=-=-=-=-=-=-=-=-=-=-" + id);
			MainUser.EditAccount(id, "rodzajKonta", "Pro");

			return RedirectToAction("AdminPanel", "Home");
		}



		[HttpPost]
		public IActionResult EditGear(string editAction, int gearId, string t_name_input)
		{
			if (editAction == "t_new")
			{
				Misc.AddGear(t_name_input);
			}
			else if(editAction =="t_name")
			{
				Misc.EditGear(gearId, t_name_input);
			}
			Console.WriteLine("Edytowanie sprzetu " + gearId + t_name_input + editAction);
			//Misc.EditGear(gearId, t_name_input);

			return RedirectToAction("AdminPanel", "Home");
		}

		[HttpPost]
		public IActionResult DeleteGear(int id)
		{
			Console.WriteLine("kasowanie sprzetu " + id);
			//Misc.RemoveGear(id);
			return RedirectToAction("AdminPanel", "Home");
		}



		[HttpPost]
		public IActionResult EditStyle(string editAction, int gearId, string t_name_input)
		{
			if (editAction == "t_new")
			{
				Misc.AddStyle(t_name_input);
			}
			else if (editAction == "t_name")
			{
				Misc.EditStyle(gearId, t_name_input);
			}
			Console.WriteLine("Edytowanie styla " + gearId + t_name_input + editAction);
			return RedirectToAction("AdminPanel", "Home");
		}

		[HttpPost]
		public IActionResult DeleteStyle(int id)
		{
			Console.WriteLine("kasowanie styla :) " + id);
			Misc.RemoveStyle(id);
			return RedirectToAction("AdminPanel", "Home");
		}



		[HttpPost]
		public IActionResult EditUser([FromForm] int userId,
				[FromForm] string editAction,
				[FromForm] string t_login_input,
				[FromForm] string t_name_input,
				[FromForm] string t_lname_input,
				[FromForm] string type_input,
				[FromForm] string skill_input,
				[FromForm] string t_age_input,
				[FromForm] string t_email_input)
		{
			Console.WriteLine("EDYCJA USERA");
			switch (editAction)
			{
				case "t_login":
					if (!string.IsNullOrEmpty(t_login_input)) {
						MainUser.EditAccount(userId, "login", t_login_input);
					}
					break;
				case "t_name":
					if (!string.IsNullOrEmpty(t_name_input))
					{
						MainUser.EditAccount(userId, "imie", t_name_input);
					}
					break;
				case "t_lastname":
					if (!string.IsNullOrEmpty(t_lname_input))
					{
						MainUser.EditAccount(userId, "nazwisko", t_lname_input);
					}
					break;
				case "t_age":
					if (!string.IsNullOrEmpty(t_age_input))
					{
						Console.WriteLine("UPADATE wiek" + t_age_input);
						MainUser.EditAccount(userId, "wiek", t_age_input);
			}
					break;
				case "t_skill":
					if (!string.IsNullOrEmpty(skill_input))
					{
						Console.WriteLine("UPADATE SKILLL" + skill_input);
						MainUser.EditAccount(userId, "umiejetnosci", skill_input);
					}
					break;
				case "t_type":
					if (!string.IsNullOrEmpty(type_input))
					{
						MainUser.EditAccount(userId, "rodzajKonta", type_input);
					}
					break;
				case "t_mail":
					if (!string.IsNullOrEmpty(t_email_input))
					{
						MainUser.EditAccount(userId, "email", t_email_input);
					}
					break;
			}
			return RedirectToAction("AdminPanel", "Home");
		}

		[HttpPost]
		public IActionResult EditEvent([FromForm] int eventId,
				[FromForm] string editActionEvent,
				[FromForm] string event_name_input,
				[FromForm] string event_date_input,
				[FromForm] string event_desc_input,
				[FromForm] string event_place_input)
		{
			Console.WriteLine("EDYCJA Eventu: id:" +eventId +" akcja: "+ editActionEvent + "wynik: " + event_place_input);


			switch (editActionEvent)
			{
				case "t_name":
					if (!string.IsNullOrEmpty(event_name_input))
					{
						Event.EditEvent(eventId,"nazwa",event_name_input);
					}
					break;
				case "t_date":
					if (!string.IsNullOrEmpty(event_date_input))
					{
						Event.EditEvent(eventId, "data", event_date_input);
					}
					break;
				case "t_description":
					if (!string.IsNullOrEmpty(event_desc_input))
					{
						Event.EditEvent(eventId, "opis", event_desc_input);
					}
					break;
				case "t_place":
					if (!string.IsNullOrEmpty(event_place_input))
					{
						Event.EditEvent(eventId, "lokalizacja", event_place_input);
					}
					break;

			}
			return RedirectToAction("AdminPanel", "Home");
		}


		[HttpPost]
		public IActionResult DeleteEvent(int id)
		{
			//USUWANIE WATKU 
			Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-USUNIETO EVENT-=-=-=-=-=-=-=-=-=-=-=-=-=-" + id);
			Event.RemoveEvent(id);

			return RedirectToAction("AdminPanel", "Home");
		}

	}
}
