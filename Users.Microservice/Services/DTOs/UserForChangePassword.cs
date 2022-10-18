﻿using System.ComponentModel.DataAnnotations;

namespace Users.Microservice.Services.DTOs
{
    public class UserForChangePassword
    {
        [Required(ErrorMessage = "Value must not be null or empty!")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Old password must not be null or empty!")]
        public string OldPassword { get; set; }


        [Required(ErrorMessage = "New password must not be null or empty!")]
        public string NewPassword { get; set; }

        public string ComfirmPassword { get; set; }
    }
}
