# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service
##  RoATP Gateway
# das-roatp-gateway



#### Requirements

- Install [.NET Core 2.2](https://www.microsoft.com/net/download)
- Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on v5.3)
- Install [Azure Storage Explorer](http://storageexplorer.com/)
- Install the editor of your choice:
  - [Jetbrains Rider](https://www.jetbrains.com/rider/)
  - [Visual Studio Code](https://code.visualstudio.com/)
  - [Visual Studio](https://visualstudio.microsoft.com/)

#### Setup

- Clone this repository


##### Code
- Grab the das-roatp-gateway configuration json file from [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-gateway/SFA.DAS.RoatpGateway.json)
- Create a Configuration table in your (Development) local Azure Storage account.
- Add a row to the Configuration table with fields: PartitionKey: LOCAL, RowKey: SFA.DAS.RoatpGateway_1.0, Data: {The contents of the local config json file}.

### Running the code

- `dotnet run` the following project:
  - SFA.DAS.RoatpGateway.Web
- Navigate to (https://localhost:45668) and you should see the start page.

- run the following repos locally to get the application working, and dashboard links working suitably
  - das-apply-service (https://github.com/SkillsFundingAgency/das-apply-service)
  - das-admin-service (https://github.com/SkillsFundingAgency/das-admin-service)
  - das-assessor-service (https://github.com/SkillsFundingAgency/das-assessor-service)
  - das-roatp-service (https://github.com/SkillsFundingAgency/das-roatp-service)
