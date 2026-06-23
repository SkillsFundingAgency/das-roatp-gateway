# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service
##  RoATP Gateway

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status%2FApprenticeships%20Providers%2Fdas-roatp-gateway?repoName=SkillsFundingAgency%2Fdas-roatp-gateway&branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2174&repoName=SkillsFundingAgency%2Fdas-roatp-gateway&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-roatp-gateway&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-roatp-gateway)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


## About

The front end for tribal users to view and administer provider gateway applications.


### Developer Setup

### Pre-Requisites

* A clone of this repository
* A storage emulator like Azurite
* Visual studio or similar IDE 

### Dependencies

* DfE Signin for user authentication
* The Apply Service [das-apply-service](https://github.com/SkillsFundingAgency/das-apply-service) should be available either running locally or accessible in an Azure tenancy.
* The Admin Service [das-admin-service](https://github.com/SkillsFundingAgency/das-admin-service) should be available either running locally or accessible in an Azure tenancy.
* The Roatp Api [das-roatp-service](https://github.com/SkillsFundingAgency/das-roatp-service) should be available either running locally or accessible in an Azure tenancy.


#### Setup

- Create a Configuration table in your (Development) local storage account.
- Obtain the local config json from the das-employer-config for das-roatp-gateway repo (https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-gateway/SFA.DAS.RoatpGateway.json) 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.RoatpGateway_1.0
  - Data: {The contents of the local config json file}
  
- In the web project, if not exist already, add `AppSettings.Development.json` file with following content:
```json  
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "cdn": {
    "url": "https://das-prd-frnt-end.azureedge.net"
  },
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.RoatpGateway,SFA.DAS.Provider.DfeSignIn",
  "ConnectionStrings": {
    "Redis": ""
  },
  "EnvironmentName": "LOCAL"
}
```

Open the solution with Visual Studio, and run the project SFA.DAS.RoatpGateway.Web, running under process 'SFA.DAS.RoatpGateway.Web' (not IIS)

## Technologies
* .NetCore 10.0
* NUnit
* Moq
* FluentAssertions