using System;
using SQLite;

namespace SpendAndSave.Models
{
	public class LoginRequestModel
	{
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        [Ignore] // Ignore in SQLite database since it's only for validation
        public string ConfirmPassword { get; set; }

        public string ProfileImagePath { get; set; } // Store image as byte array

    }
}

