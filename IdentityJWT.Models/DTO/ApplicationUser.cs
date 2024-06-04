﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IdentityJWT.Models.DTO
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;
        public string Fname { get; set; }
        public string Lname { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}