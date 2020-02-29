using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class BatchViewModel
    {
       

        public int BatchId { get; set; }

        [Required(ErrorMessage = "The Class field is required.")]
        public int ClassId { get; set; }


        [Required]
        [MaxLength(50, ErrorMessage = "The field Batch Name must be a minimum length of '5' and maximum length of '50'.")]
        [MinLength(5, ErrorMessage = "The field Batch Name must be a minimum length of '5' and maximum length of '50'.")]
        [Display(Name = "Batch Name")]
        public string Name { get; set; }

        [MaxLength(8, ErrorMessage = "Please select valid time.")]
        [MinLength(8, ErrorMessage = "Please select valid time.")]
        [Display(Name = "In-Time")]
        public string InTime { get; set; }

        [MaxLength(8, ErrorMessage = "Please select valid time.")]
        [MinLength(8, ErrorMessage = "Please select valid time.")]
        [Display(Name = "Out-Time")]
        public string OutTime { get; set; }

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Class is required.")]
        public string SelectedClasses { get; set; }

        public string CurrentUserRole { get; set; }


        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }
    }
}