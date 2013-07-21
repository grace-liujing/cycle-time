using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace myCycle.Models
{
    public class StoryCycleModel
    {
        public StoryCycleModel()
        {
            StatusPieces = new List<StatusPiece>();
        }

        public string ProjectName { get; set; }
        public string StoryNumber { get; set; }
        public string StoryName { get; set; }
        public List<StatusPiece> StatusPieces { get; set; } 
    }

    public class StatusPiece
    {
        public StatusPiece(string status, TimeSpan span)
        {
            Status = status;
            Span = span;
        }

        public string Status { get; private set; }
        public TimeSpan Span { get; private set; }
        public double Duration
        {
            get { return Span.TotalMinutes; }
        }
    }

    public class StatusMoment
    {
        private string status;
        public string Status { get { return status; } set { status = value.Trim() == string.Empty ? "Not Set" : value; } }
        public DateTime Moment { get; set; } 
    }


    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
