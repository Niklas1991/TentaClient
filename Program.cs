using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
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
			bool isRunning = true;
			while (isRunning)
			{
				Console.WriteLine("[a] Input a to create a first user");
				Console.WriteLine("[b] Input b to run everything as the first user(He is admin). This will also create the rest of the users");
				Console.WriteLine("[c] Input c to run everything as VD(will require additional input)");
				Console.WriteLine("[d] Input d to run everything as CountryManager(will require additional input)");
				Console.WriteLine("[e] Input e to run everything as Employee");
				Console.WriteLine("[x] Input x to exit the program");
				string menuChoice = Console.ReadLine();
				switch (menuChoice)
				{
					case ("a"):
						{
							var response = await PostUser("FirstUserIsAdmin1991", 1, "FirstUserIsAdmin1991@hotmail.com", "!Hejalla1234");
							Console.WriteLine(response.ToString());
							break;
						}
					case ("b"):
						{
							string userName = "FirstUserIsAdmin1991";
							string password = "!Hejalla1234";
							var userAuthentication = new AuthenticateLogin { UserName = userName, Password = password };
							var authResult = await AuthUser(userAuthentication);
							await PostCM(authResult.JwtToken);
							await PostVD(authResult.JwtToken);
							await PostUser("SecondUserIsEmployeeOnly1991", 2, "SecondUserIsEmployeeOnly1991@hotmail.com", "!Hejalla1234");
							Console.WriteLine("All users created! ");
							Thread.Sleep(2000);
							Console.WriteLine("Running GET ALL ORDERS (Should return everything) ");
							Thread.Sleep(2000);
							var getAllOrderList = await GetAllorders(authResult.JwtToken);
							foreach (var order in getAllOrderList)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}
							Thread.Sleep(2000);
							Console.WriteLine("Running GET MY ORDERS on employeeID 7 (Should return everything from employeeID 7)");
							Thread.Sleep(2000);
							var getMyOrderList = await GetMyOrders(authResult.JwtToken, 7);
							foreach (var order in getMyOrderList)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}
							Thread.Sleep(2000);
							Console.WriteLine("Running GET COUNTRY ORDERS (Should return everything from country FRANCE)");
							Thread.Sleep(2000);
							var getCountryOrderLIst = await GetCountryOrders(authResult.JwtToken, "FRANCE");
							foreach (var order in getCountryOrderLIst)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}

							break;
						}
					case ("c"):
						{
							string userName = "VD1991";
							string password = "!Hejalla1234";
							var userAuthentication = new AuthenticateLogin { UserName = userName, Password = password };
							var authResult = await AuthUser(userAuthentication);
							
							Thread.Sleep(2000);
							Console.WriteLine("Running GET ALL ORDERS as VD (Should return everything) ");
							Thread.Sleep(2000);
							var getAllOrderList = await GetAllorders(authResult.JwtToken);
							foreach (var order in getAllOrderList)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}
							Thread.Sleep(2000);
							Console.WriteLine("Running GET MY ORDERS as VD on employeeID 6 (Should return everything from employeeID 6)");
							Thread.Sleep(2000);
							var getMyOrderList = await GetMyOrders(authResult.JwtToken, 6);
							foreach (var order in getMyOrderList)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}
							Thread.Sleep(2000);
							Console.WriteLine("Running GET COUNTRY ORDERS (Should return everything from country GERMANY)");
							Thread.Sleep(2000);
							var getCountryOrderLIst = await GetCountryOrders(authResult.JwtToken, "GERMANY");
							foreach (var order in getCountryOrderLIst)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}
							break;
						}
					case ("d"):
						{
							string userName = "CM1991";
							string password = "!Hejalla1234";
							var userAuthentication = new AuthenticateLogin { UserName = userName, Password = password };
							var authResult = await AuthUser(userAuthentication);
							
							Console.WriteLine("Running GET ALL ORDERS as Country Manager (Should return everything where shipcountry = employee.country) ");
							Thread.Sleep(4000);
							var getAllOrderList = await GetAllorders(authResult.JwtToken);
							foreach (var order in getAllOrderList)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}
							Thread.Sleep(4000);
							Console.WriteLine("Running GET MY ORDERS as CM(Country manager has employee so returns everything on users employee)");
							Thread.Sleep(4000);
							var getMyOrderList = await GetMyOrders(authResult.JwtToken);
							foreach (var order in getMyOrderList)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}
							Thread.Sleep(4000);
							Console.WriteLine("Running GET COUNTRY ORDERS (Should return everything from own country)");
							Thread.Sleep(4000);
							var getCountryOrderLIst = await GetCountryOrders(authResult.JwtToken);							
							foreach (var order in getCountryOrderLIst)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}							
							
							break;
						}
					case ("e"):
						{
							string userName = "SecondUserIsEmployeeOnly1991";
							string password = "!Hejalla1234";
							var userAuthentication = new AuthenticateLogin { UserName = userName, Password = password };
							var authResult = await AuthUser(userAuthentication);

							Console.WriteLine("Running GET ALL ORDERS as Employee (List should return null so will be unauthorized) ");
							Thread.Sleep(4000);
							var getAllOrderList = await GetAllorders(authResult.JwtToken);
							if (getAllOrderList == null)
							{
								Console.WriteLine("Orderlist was empty, you don't have authorization to use this method");
							}
							else
							{
								foreach (var order in getAllOrderList)
								{
									Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
								}
							}							
							Thread.Sleep(4000);
							Console.WriteLine("Running GET MY ORDERS as Employee(Will return the orders on the logged in user)");
							Thread.Sleep(4000);
							var getMyOrderList = await GetMyOrders(authResult.JwtToken);
							foreach (var order in getMyOrderList)
							{
								Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
							}
							Thread.Sleep(4000);
							Console.WriteLine("Running GET COUNTRY ORDERS as Employee(List should return null so will be unauthorized)");
							Thread.Sleep(4000);
							var GetCountryOrderList = await GetCountryOrders(authResult.JwtToken);
							if (GetCountryOrderList == null)
							{
								Console.WriteLine("\nOrderlist was empty, you don't have authorization to use this method.");
							}
							else
							{
								foreach (var order in GetCountryOrderList)
								{
									Console.WriteLine(order.CustomerId + " " + order.EmployeeId + " " + order.ShipCountry);
								}
							}
							
							break;
						}
					case ("x"):
					default:
						{
							isRunning = false;
							break;
						}
				}
			}
			
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
			if (response.StatusCode != HttpStatusCode.OK)
			{
				return response.StatusCode.ToString() + response.Content.ReadAsStringAsync().Result;
			}
			
			string result = response.Content.ReadAsStringAsync().Result;
			return result;
		}
		public static async Task<string> PostAdmin(string token)
		{
			if (!TokenValidation(token))
			{
				return null;
			}
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
			if (response.StatusCode != HttpStatusCode.OK)
			{
				return response.StatusCode.ToString() + response.Content.ReadAsStringAsync().Result;
			}
			string result = response.Content.ReadAsStringAsync().Result;
			return result;
		}
		public static async Task<string> PostVD(string token)
		{
			if (!TokenValidation(token))
			{
				return null;
			}
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
			if (response.StatusCode != HttpStatusCode.OK)
			{
				return response.StatusCode.ToString() + response.Content.ReadAsStringAsync().Result;
			}
			string result = response.Content.ReadAsStringAsync().Result;
			return result;
		}
		public static async Task<string> PostCM(string token)
		{
			if (!TokenValidation(token))
			{
				return null;
			}
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
			if (response.StatusCode != HttpStatusCode.OK)
			{
				return response.StatusCode.ToString() + response.Content.ReadAsStringAsync().Result;
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
				var result = await response.Content.ReadAsStringAsync();
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
			if (!TokenValidation(token))
			{
				return null;
			}
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
			if (!TokenValidation(token))
			{
				return null;
			}
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
		public static async Task<string> UpdateUser(string token, UpdateRequest user)
		{
			if (!TokenValidation(token))
			{
				return null;
			}
			UriBuilder uribuilder = new UriBuilder();
			uribuilder.Scheme = "https";
			uribuilder.Port = 5001;
			uribuilder.Path = "/api/user/update-employee";
			uribuilder.Query = "username=" + user.UserName;
			Uri uri = uribuilder.Uri;
			var json = JsonConvert.SerializeObject(user);
			var data = new StringContent(json, Encoding.UTF8, "application/json");

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.PatchAsync(uri, data);
			return response.StatusCode.ToString();
		}

		#endregion

		#region CRUD Orders
		public static async Task<IEnumerable<OrderResponse>> GetAllorders(string token)
		{
			if (!TokenValidation(token))
			{
				return null;
			}
			string endpoint = "order/get-all-orders";
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			
			var response = await client.GetAsync(urlHttps + endpoint);

			var result = response.Content.ReadAsStringAsync().Result;
			var desResult = JsonConvert.DeserializeObject<IEnumerable<OrderResponse>>(result);
			return desResult;
		}

		public static async Task<IEnumerable<OrderResponse>> GetMyOrders(string token, int? employeeId = null)
		{
			if (!TokenValidation(token))
			{
				return null;
			}
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
		public static async Task<IEnumerable<OrderResponse>> GetCountryOrders(string token, string? country = null)
		{
			if (!TokenValidation(token))
			{
				return null;
			}
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

		public static bool TokenValidation(string jwt)
		{
			var handler = new JwtSecurityTokenHandler();
			var token = handler.ReadJwtToken(jwt);
			if (token.ValidTo <= DateTime.UtcNow)
			{
				return false;
			}
			return true;
		}
	}
}

