using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpGateway.Domain.CharityCommission;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using SFA.DAS.RoatpGateway.Web.Extensions;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public class GatewayOrganisationChecksOrchestrator : IGatewayOrganisationChecksOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly ILogger<GatewayOrganisationChecksOrchestrator> _logger;
        private readonly IRoatpOrganisationSummaryApiClient _organisationSummaryApiClient;
        private readonly IRoatpApiClient _roatpApiClient;
        public GatewayOrganisationChecksOrchestrator(IRoatpApplicationApiClient applyApiClient,
                                                      IRoatpOrganisationSummaryApiClient organisationSummaryApiClient, IRoatpApiClient roatpApiClient, ILogger<GatewayOrganisationChecksOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
            _roatpApiClient = roatpApiClient;
            _organisationSummaryApiClient = organisationSummaryApiClient;
        }

        public async Task<LegalNamePageViewModel> GetLegalNameViewModel(GetLegalNameRequest request)
        {
            _logger.LogInformation($"Retrieving legal name details for application {request.ApplicationId}");

            var model = new LegalNamePageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewaySequences.OrganisationChecks,
                                                                                            GatewayPageIds.LegalName,
                                                                                            request.UserId,
                                                                                            request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.LegalName,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.LegalName]);

            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);

            if (ukrlpDetails != null)
            {
                model.UkrlpLegalName = ukrlpDetails.ProviderName;

                if (ukrlpDetails.VerifiedByCompaniesHouse)
                {
                    var companiesHouseDetails = await _applyApiClient.GetCompaniesHouseDetails(request.ApplicationId);
                    if (companiesHouseDetails != null)
                    {
                        model.CompaniesHouseLegalName = companiesHouseDetails.CompanyName;
                    }
                }

                if (ukrlpDetails.VerifiedbyCharityCommission)
                {
                    var charityNumber= ExtractCharityNumberFromUkrlpDetails(ukrlpDetails);

                    if (!string.IsNullOrEmpty(charityNumber))
                    {
                        var charityDetails = await _roatpApiClient.GetCharityDetails(charityNumber);
                        if (charityDetails != null)
                        {
                            model.CharityCommissionLegalName = charityDetails.Name;
                        }
                    }
                }
            }

            return model;
        }

        public async Task<TradingNamePageViewModel> GetTradingNameViewModel(GetTradingNameRequest request)
        {
            _logger.LogInformation($"Retrieving trading name details for application {request.ApplicationId}");

            var model = new TradingNamePageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewaySequences.OrganisationChecks,
                                                                                            GatewayPageIds.TradingName,
                                                                                            request.UserId,
                                                                                            request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.TradingName,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.TradingName]);

            var ukrlpDetail = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);
            if (ukrlpDetail.ProviderAliases != null && ukrlpDetail.ProviderAliases.Count > 0)
            {
                model.UkrlpTradingName = ukrlpDetail.ProviderAliases.First().Alias;
            }

            model.ApplyTradingName = await _applyApiClient.GetTradingName(request.ApplicationId);

            return model;
        }

        public async Task<OrganisationStatusViewModel> GetOrganisationStatusViewModel(GetOrganisationStatusRequest request)
        {
            _logger.LogInformation($"Retrieving organisation status details for application {request.ApplicationId}");

            var model = new OrganisationStatusViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewaySequences.OrganisationChecks, 
                                                                                                GatewayPageIds.OrganisationStatus,
                                                                                                request.UserId,
                                                                                                request.UserName,
                                                                                                RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                                RoatpGatewayConstants.Headings.OrganisationStatusCheck,
                                                                                                NoSelectionErrorMessages.Errors[GatewayPageIds.OrganisationStatus]);

            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);

            if (ukrlpDetails != null)
            {
                model.UkrlpStatus = ukrlpDetails.ProviderStatus?.CapitaliseFirstLetter();

                if (ukrlpDetails.VerifiedByCompaniesHouse)
                {
                    var companiesHouseDetails = await _applyApiClient.GetCompaniesHouseDetails(request.ApplicationId);
                    if (companiesHouseDetails != null)
                    {
                        model.CompaniesHouseStatus = companiesHouseDetails.Status.CapitaliseFirstLetter();
                    }
                }

                if (ukrlpDetails.VerifiedbyCharityCommission)
                {
                    var charityNumber = ExtractCharityNumberFromUkrlpDetails(ukrlpDetails);

                    if (!string.IsNullOrEmpty(charityNumber))
                    {
                        var charityDetails = await _roatpApiClient.GetCharityDetails(charityNumber);
                        if (charityDetails != null)
                        {
                            model.CharityCommissionStatus = charityDetails.Status.CapitaliseFirstLetter();
                        }
                    }
                }
            }

            return model;
        }

        public async Task<AddressCheckViewModel> GetAddressViewModel(GetAddressRequest request)
        {
            _logger.LogInformation($"Retrieving address check details for application {request.ApplicationId}");

            var model = new AddressCheckViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewaySequences.OrganisationChecks,
                                                                                            GatewayPageIds.Address,
                                                                                            request.UserId,
                                                                                            request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.AddressCheck,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.Address]);

            var organisationAddress = await _applyApiClient.GetOrganisationAddress(request.ApplicationId);
            if (organisationAddress != null)
            {
                var AddressArray = new[] { organisationAddress.Address1, organisationAddress.Address2, organisationAddress.Address3, organisationAddress.Address4, organisationAddress.Town, organisationAddress.PostCode };
                model.SubmittedApplicationAddress = string.Join(", ", AddressArray.Where(s => !string.IsNullOrEmpty(s)));
            }

            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);
            if (ukrlpDetails != null)
            {
                var ukrlpAddressLine1 = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Address1;
                var ukrlpAddressLine2 = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Address2;
                var ukrlpAddressLine3 = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Address3;
                var ukrlpAddressLine4 = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Address4;
                var ukrlpTown = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.Town;
                var ukrlpPostCode = ukrlpDetails.ContactDetails.FirstOrDefault().ContactAddress.PostCode;

                var ukrlpAarray = new[] { ukrlpAddressLine1, ukrlpAddressLine2, ukrlpAddressLine3, ukrlpAddressLine4, ukrlpTown, ukrlpPostCode };
                var ukrlpAddress = string.Join(", ", ukrlpAarray.Where(s => !string.IsNullOrEmpty(s)));
                model.UkrlpAddress = ukrlpAddress;
            }

            return model;
        }

        public async Task<IcoNumberViewModel> GetIcoNumberViewModel(GetIcoNumberRequest request)
        {
            _logger.LogInformation($"Retrieving ICO Number check details for application {request.ApplicationId}");

            var model = new IcoNumberViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewaySequences.OrganisationChecks,
                                                                                            GatewayPageIds.IcoNumber,
                                                                                            request.UserId,
                                                                                            request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.IcoNumber,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.IcoNumber]);


            var organisationAddress = await _applyApiClient.GetOrganisationAddress(request.ApplicationId);
            if (organisationAddress != null)
            {
                var AddressArray = new[] { organisationAddress.Address1, organisationAddress.Address2, organisationAddress.Address3, organisationAddress.Address4, organisationAddress.Town, organisationAddress.PostCode };
                model.OrganisationAddress = string.Join(", ", AddressArray.Where(s => !string.IsNullOrEmpty(s)));
            }

            var icoNumber = await _applyApiClient.GetIcoNumber(request.ApplicationId);
            model.IcoNumber = icoNumber.Value;

            return model;
        }

        public async Task<WebsiteViewModel> GetWebsiteViewModel(GetWebsiteRequest request)
        {
            _logger.LogInformation($"Retrieving Website check details for application {request.ApplicationId}");

            var model = new WebsiteViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewaySequences.OrganisationChecks,
                                                                                            GatewayPageIds.WebsiteAddress,
                                                                                            request.UserId,
                                                                                            request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.Website,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.WebsiteAddress]);

            model.SubmittedWebsite = await _applyApiClient.GetOrganisationWebsiteAddress(request.ApplicationId);

            var isSubmittedWebsiteProperUri = StringExtensions.ValidateHttpURL(model.SubmittedWebsite, out Uri submittedWebsiteProperUri);
            if (isSubmittedWebsiteProperUri)
            {
                model.SubmittedWebsiteUrl = submittedWebsiteProperUri?.AbsoluteUri;
            }

            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);
            if (ukrlpDetails != null && ukrlpDetails.ContactDetails != null)
            {
                model.UkrlpWebsite = ukrlpDetails.ContactDetails.FirstOrDefault(x => x.ContactType == RoatpGatewayConstants.ProviderContactDetailsTypeLegalIdentifier)?.ContactWebsiteAddress;

                var isUkrlpWebsiteProperUri = StringExtensions.ValidateHttpURL(model.UkrlpWebsite, out Uri ukrlpWebsiteProperUri);
                if (isUkrlpWebsiteProperUri)
                {
                    model.UkrlpWebsiteUrl = ukrlpWebsiteProperUri?.AbsoluteUri;
                }
            }

            return model;
        }

        public async Task<OrganisationRiskViewModel> GetOrganisationRiskViewModel(GetOrganisationRiskRequest request)
        {
            _logger.LogInformation($"Retrieving Organisation high risk check details for application {request.ApplicationId}");

            var model = new OrganisationRiskViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewaySequences.OrganisationChecks,
                                                                                            GatewayPageIds.OrganisationRisk,
                                                                                            request.UserId,
                                                                                            request.UserName,
                                                                                            RoatpGatewayConstants.Captions.OrganisationChecks,
                                                                                            RoatpGatewayConstants.Headings.OrganisationRisk,
                                                                                            NoSelectionErrorMessages.Errors[GatewayPageIds.OrganisationRisk]);

            model.OrganisationType = await _organisationSummaryApiClient.GetTypeOfOrganisation(request.ApplicationId);
            model.TradingName = await _applyApiClient.GetTradingName(request.ApplicationId);
            _logger.LogInformation($"Retrieving company number in organisation high risk for application {request.ApplicationId}");
            model.CompanyNumber = await _organisationSummaryApiClient.GetCompanyNumber(request.ApplicationId);

            _logger.LogInformation($"Retrieving charity number in organisation high risk for application {request.ApplicationId}");
            model.CharityNumber = await _organisationSummaryApiClient.GetCharityNumber(request.ApplicationId);

            return model;
        }

        private static string ExtractCharityNumberFromUkrlpDetails(ProviderDetails ukrlpDetails)
        {
            const string charityCommission = "Charity Commission";
            var charityNumber = string.Empty;
            foreach (var verificationDetail in ukrlpDetails.VerificationDetails.Where(res =>
                res.VerificationAuthority == charityCommission))
            {
                charityNumber = verificationDetail.VerificationId;
            }

            return charityNumber;
        }
    }
}
