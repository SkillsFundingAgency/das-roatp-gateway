﻿using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class GatewayPageAnswer
    {
        private Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string PageId { get; set; }
        public string Status { get; set; }
        public string GatewayPageData { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
