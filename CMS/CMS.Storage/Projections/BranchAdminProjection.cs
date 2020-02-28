﻿namespace CMS.Domain.Storage.Projections
{
    public class BranchAdminProjection
    {
        public int AId { get; set; }

        public int BranchId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public string Email { get; set; }

        public string ConfirmEmail { get; set; }

        public string ContactNo { get; set; }

        public string BranchName { get; set; }
    }
}


