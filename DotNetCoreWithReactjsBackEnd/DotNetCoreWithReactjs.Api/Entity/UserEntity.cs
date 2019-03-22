using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreWithReactjs.Api.Entity
{
    public class UserEntity
    {
        [Key]
        public Guid Guid { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string FullName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [RegularExpression(@"^(?!\d+[09]{4}$)\d{9}$")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public byte[] PasswordHash { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public byte[] PasswordSalt { get; set; }

        public UserEntity()
        {

        }
    }
}
