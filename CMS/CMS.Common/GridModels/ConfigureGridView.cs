using CMS.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;


namespace CMS.Common.GridModels
{
    class ConfigureGridView
    {
        public int Id { get; set; }

        public string name { get; set; }

        public string aboutus { get; set; }

        public string address { get; set; }

        public string email_id { get; set; }

        public string sender_id { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        
        public string brocherfile { get; set; }
    }
}
