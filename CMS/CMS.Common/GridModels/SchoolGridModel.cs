﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Common.GridModels
{
  public  class SchoolGridModel
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string CenterNumber { get; set; }
        [Exclude]
        public string Action { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
