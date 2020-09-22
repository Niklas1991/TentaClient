using System;
using System.Collections.Generic;
using System.Text;

namespace TentaClient.Model
{
	public class AccountResponse
	{
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
