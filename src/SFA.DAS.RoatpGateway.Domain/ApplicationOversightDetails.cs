﻿using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class ApplicationOversightDetails
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public OversightReviewStatus OversightStatus { get; set; }
    }
}
