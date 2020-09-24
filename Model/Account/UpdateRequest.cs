using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TentaClient.Model.Entities;

namespace TentaClient.Model.Account
{
	public class UpdateRequest
	{
        private string _password;
        private string _role;
        private string _email;


        [Required]
        public string UserName { get; set; }
        

        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }



        // helpers

        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
