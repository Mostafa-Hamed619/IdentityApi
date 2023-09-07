﻿using System.ComponentModel.DataAnnotations;

namespace AdminFullStack.DTO.Account
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(15,MinimumLength =3,ErrorMessage ="First name must be at least {2} characters,Maximum {1} characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Last name must be at least {2} characters,Maximum {1} characters")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage ="Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [StringLength(15,MinimumLength =6,ErrorMessage = "password must be at least {2},Maximum {1} characters")]
        public string Password { get; set; }
    }
}
