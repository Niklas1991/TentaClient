using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TentaClient.Model;
using TentaClient.Model.Account;
using TentaClient.Model.Entities;

namespace TentaClient
{
	class Program
	{
		protected static readonly DataContext.DataContext _context = new DataContext.DataContext();
		protected static readonly string urlHttp = "http://localhost:5000/api/";
		protected static readonly string urlHttps = "https://localhost:5001/api/";

		static async Task Main(string[] args)
		{
			// CREATE FIRST USER
			
			await PostUser("FirstUserIsAdmin1991", 1, "FirstUserIsAdmin1991@hotmail.com", "!Hejalla1234" );
			await PostUser("SecondUserIsEmployeeOnly1991", 2, "SecondUserIsEmployeeOnly1991@hotmail.com", "!Hejalla1234");
			switch ()
			{

			}

			


			// LOGIN AUTHENTICATION IN CONSOLE
			//Console.WriteLine("Write your username: ");
			//string userName = Console.ReadLine();
			//Console.WriteLine("Write your password: ");
			//string password = Console.ReadLine();

			// PREMADE AUTHENTICATION 
			string userName = "CM1991";
			string password = "!Hejalla1234";
			var userAuthentication = new AuthenticateLogin { UserName = userName, Password = password };
			var authResult = await AuthUser(userAuthentication);

			// CREATE ALL OTHER USERS (NEEDS ADMIN TOKEN)
			//await PostAdmin(authResult.JwtToken);
			//await PostCM(authResult.JwtToken);
			//await PostVD(authResult.JwtToken);

			// DELETE USER
			//string userToDelete = "Admin1991";
			//await DeleteUser(authResult.JwtToken, userToDelete);

			// GET ALL ORDERS
			//var orderList = await GetAllorders(authResult.JwtToken);
			//foreach (var order in orderList)
			//{
			//	Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
			//}

			//GET MY ORDERS
			//var orderList = await GetMyOrders(authResult.JwtToken, 5);
			//foreach (var order in orderList)
			//{
			//	Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
			//}

			//GET COUNTRY ORDERS
			//var orderList = await GetCountryOrders(authResult.JwtToken, "FRANCE");
			//foreach (var order in orderList)
			//{
			//	Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
			//}


			// GET ALL USERS
			//var getAllUsersResult = await GetAllUsers(authResult.JwtToken);

			//foreach (var item in getAllUsersResult)
			//{
			//	Console.WriteLine(item.UserName);
			//}





			Console.ReadKey();
		}

		#region CreatingUsers
		public static async Task<string> PostUser(string userName, int employeeID, string email, string password)
		{
			var user = new RegisterUser() { UserName = userName, EmployeeId = employeeID, Email = email, Password = password };			
			string endpoint = "user/register-employee";
			var json = JsonConvert.SerializeObject(user);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			using var client = new HttpClient();

			var response = await client.PostAsync(urlHttps + endpoint, data);
			if (response == null)
			{
				throw new Exception("Result was null, check your input");
			}
			string result = response.Content.ReadAsStringAsync().Result;
			return result;
		}
		public static async Task<string> PostAdmin(string token)
		{
			var user = new RegisterUser();
			user.UserName = "Admin1991";
			user.EmployeeId = 3;
			user.Email = "Admin@hotmail1.com";
			user.Password = "!Hejalla1234";
			string endpoint = "user/register-admin";
			var json = JsonConvert.SerializeObject(user);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.PostAsync(urlHttps + endpoint, data);
			if (response == null)
			{
				throw new Exception("Result was null, check your input");
			}
			string result = response.Content.ReadAsStringAsync().Result;
			return result;
		}
		public static async Task<string> PostVD(string token)
		{
			var user = new RegisterUser();
			user.UserName = "VD1991";
			user.EmployeeId = 4;
			user.Email = "VD@hotmail1.com";
			user.Password = "!Hejalla1234";

			string endpoint = "user/register-vd";
			var json = JsonConvert.SerializeObject(user);
			var data = new StringContent(json, Encoding.UTF8, "application/json");

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var response = await client.PostAsync(urlHttps + endpoint, data);
			if (response == null)
			{
				throw new Exception("Result was null, check your input");
			}
			string result = response.Content.ReadAsStringAsync().Result;
			return result;
		}
		public static async Task<string> PostCM(string token)
		{
			var user = new RegisterUser();
			user.UserName = "CM1991";
			user.EmployeeId = 5;
			user.Email = "CM@hotmail1.com";
			user.Password = "!Hejalla1234";

			string endpoint = "user/register-countrymanager";
			var json = JsonConvert.SerializeObject(user);
			var data = new StringContent(json, Encoding.UTF8, "application/json");

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var response = await client.PostAsync(urlHttps + endpoint, data);
			if (response == null)
			{
				throw new Exception("Result was null, check your input");
			}
			string result = response.Content.ReadAsStringAsync().Result;
			return result;
		}
		#endregion
		public static async Task<AuthenticateResponse> AuthUser(AuthenticateLogin user)
		{
			string endpoint = "user/authenticate";
			var json = JsonConvert.SerializeObject(user);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			using (var client = new HttpClient())
			{
				var response = await client.PostAsync(urlHttps + endpoint, data);
				var result = response.Content.ReadAsStringAsync().Result;
				var authResult = JsonConvert.DeserializeObject<AuthenticateResponse>(result);

				var findUser = await _context.Accounts.Where(x => x.UserName == user.UserName).FirstOrDefaultAsync();
				if (findUser == null)
				{
					var account = new Account { UserName = authResult.UserName, RefreshToken = authResult.RefreshToken, JwtToken = authResult.JwtToken };
					await _context.Accounts.AddAsync(account);
					await _context.SaveChangesAsync();
					return authResult;
				}
				findUser.JwtToken = authResult.JwtToken;
				findUser.RefreshToken = authResult.RefreshToken;

				await _context.SaveChangesAsync();
				return authResult;
			}
		}
		#region CRUD Users
		public static async Task<IEnumerable<AccountResponse>> GetAllUsers(string token)
		{
			string endpoint = "user/get-all-users";
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var response = await client.GetAsync(urlHttps + endpoint);

			var result = response.Content.ReadAsStringAsync().Result;
			var desResult = JsonConvert.DeserializeObject<IEnumerable<AccountResponse>>(result);
			return desResult;
		}


		public static async Task<string> DeleteUser(string token, string userName)
		{

			UriBuilder uribuilder = new UriBuilder();
			uribuilder.Scheme = "https";
			uribuilder.Port = 5001;
			uribuilder.Path = "/api/user/delete";
			uribuilder.Query = "username=" + userName;
			Uri uri = uribuilder.Uri;

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.DeleteAsync(uri);
			return response.StatusCode.ToString();
		}

		#endregion

		#region CRUD Orders
		public static async Task<IEnumerable<OrderResponse>> GetAllorders(string token)
		{
			string endpoint = "order/get-all-orders";
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			
			var response = await client.GetAsync(urlHttps + endpoint);

			var result = response.Content.ReadAsStringAsync().Result;
			var desResult = JsonConvert.DeserializeObject<IEnumerable<OrderResponse>>(result);
			return desResult;
		}

		public static async Task<IEnumerable<OrderResponse>> GetMyOrders(string token, int employeeId)
		{
			
			UriBuilder uribuilder = new UriBuilder();
			uribuilder.Scheme = "https";
			uribuilder.Port = 5001;
			uribuilder.Path = "/api/order/get-my-orders";
			uribuilder.Query = "employeeId=" + employeeId;
			Uri uri = uribuilder.Uri;
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);			
						
			var response = await client.GetAsync(uri);

			var result = response.Content.ReadAsStringAsync().Result;
			var desResult = JsonConvert.DeserializeObject<IEnumerable<OrderResponse>>(result);
			return desResult;
		}
		public static async Task<IEnumerable<OrderResponse>> GetCountryOrders(string token, string country)
		{

			UriBuilder uribuilder = new UriBuilder();
			uribuilder.Scheme = "https";
			uribuilder.Port = 5001;
			uribuilder.Path = "/api/order/get-country-orders";
			uribuilder.Query = "country=" + country;
			Uri uri = uribuilder.Uri;

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);			
		
			var response = await client.GetAsync(uri);

			var result = response.Content.ReadAsStringAsync().Result;
			var desResult = JsonConvert.DeserializeObject<IEnumerable<OrderResponse>>(result);
			return desResult;			
		}
		#endregion
	}
}

