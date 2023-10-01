using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AdminFullStack.DTO.Admin
{
    public class MemberDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsLocked { get; set; }

        public DateTime DateCreated { get;set; }

        public IEnumerable<string> Role { get; set; }
    }
}
