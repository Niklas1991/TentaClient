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
			//var result = await PostUser(); //Creates user
			//var userResult = JsonConvert.DeserializeObject<AccountResponse>(result);
			
			//Console.WriteLine(userResult.UserName);
			//Console.WriteLine(userResult.Email);
			//Console.WriteLine(userResult.Created);
			//Console.WriteLine(userResult.Role);

			Console.WriteLine("Write your username: ");
			string userName = Console.ReadLine();
			Console.WriteLine("Write your password: ");
			string password = Console.ReadLine();
			var userAuthentication = new AuthenticateLogin { UserName = userName, Password = password};
			var authResult = await AuthUser(userAuthentication);
			Console.WriteLine(authResult.Role);
			var getAllUsersResult = await GetAllUsers(authResult.JwtToken);
			
			
			foreach (var item in getAllUsersResult)
			{
				Console.WriteLine(item.UserName);
			}

			//var authenticateUser = await AuthUser(); //authenticates user

			//var authResult = JsonConvert.DeserializeObject<AuthenticateResponse>(authenticateUser);//converts from Json
			//Console.WriteLine("\njwttoken: " + authResult.JwtToken);
			//Console.WriteLine("\nrefresh token: " + authResult.RefreshToken);

			//var newTokenResponse = await RefreshToken(authResult.JwtToken, authResult.RefreshToken);
			//var newAuthResult = JsonConvert.DeserializeObject<AuthenticateResponse>(newTokenResponse);
			//AuthenticateResponse refreshedToken = JsonConvert.DeserializeObject<AuthenticateResponse>(refreshToken);
			//RefreshTokenRequest refreshedToken = JsonConvert.DeserializeObject<RefreshTokenRequest>(refreshToken);
			//Console.WriteLine("\n" + refreshedToken.RefreshToken );

			//Console.WriteLine("\nrefreshed jwt token: " + newAuthResult.JwtToken);
			//Console.WriteLine("\nrefreshed refresh token: " + newAuthResult.RefreshToken);


			Console.ReadKey();
		}

		public static async Task<string> PostUser()
		{
			var user = new RegisterUser();
			user.UserName = "Niklas1991";
			user.EmployeeId = 5;
			user.Email = "Niklas1991@hotmail.com";
			user.Password = "!Hejalla1234";

			string endpoint = "user/register-employee";
			//var url = "http://localhost:5001/user/register-employee";
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
		public static async Task<string> PostAdmin()
		{
			var user = new RegisterUser();
			user.UserName = "Admin1991";
			user.EmployeeId = 1;
			user.Email = "Admin@hotmail.com";
			user.Password = "!Hejalla1234";

			string endpoint = "user/register-admin";
			//var url = "http://localhost:5001/user/register-employee";
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
				var account = new Account {UserName = authResult.UserName, RefreshToken = authResult.RefreshToken, JwtToken = authResult.JwtToken };
				await _context.Accounts.AddAsync(account);
				await _context.SaveChangesAsync();
				//var addTokenToAccount = await _context.Accounts.AddAsync();
				return authResult;
			}
		}

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

	}
}

