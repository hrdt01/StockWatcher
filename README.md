# StockTracker

## Project Overview
StockTracker is a comprehensive stock market tracking application that monitors end-of-day stock data for specified companies. The project is built using modern cloud-native architecture and provides a user-friendly web interface for tracking stock performance.


## Key Features
- End-of-day stock data analysis
- Custom financial KPI calculations for selected companies
- Role-based access control (Admin and Viewer roles)
- Interactive data visualization
- Automated data extraction on business days

## Technical Stack

### Frontend
- Blazor WebAssembly
- FluentUI for Blazor
- Radzen Components
- Chart.js for data visualization

### Backend
- .NET 8 (Isolated Runtime)
- Azure Functions
- Azure Storage (Tables and Queues)
- MarketStack API Integration. For more information, check [here](https://marketstack.com/)
- Container support and image generation in CD pipeline.

### Infrastructure
- Azure Cloud Services
- Terraform for Infrastructure as Code (IaC)
- Azure Web Apps
- Azure Storage Account

### Architecture Components
- Extractor Function: Scheduled data collection from stock market APIs
- KPI Processor: Data analysis and metrics calculation
- Cleanup Processor: Data maintenance and housekeeping
- Frontend Service: Web-based user interface
- Authentication Service: Secure access control

### Development Tools & Practices
- Git for version control
- CI/CD pipelines for automated integration/deployment with Azure DevOps
- Infrastructure as Code using Terraform
- Dependency Injection
- Resilient HTTP communication
- CQRS pattern implementation

## Testing and Quality Assurance
### Test Coverage
- Unit Testing: Comprehensive test suite for business logic and services
- Integration Testing: End-to-end testing of component interactions
- API Testing: Validation of external API integrations

### Quality Metrics
- Automated test execution in Azure DevOps CI/CD pipelines
  - Tests run on pipeline execution 
- Code coverage reporting through Azure DevOps
  - Coverage reports generated for each test run
  - Coverage trends tracked across builds
   
### Testing Tools
- xUnit for integration testing
- NUnit for unit testing
- Moq for mocking dependencies
- Code coverage tools integrated with Azure DevOps
  - Test results publishing
  - Coverage reporting and tracking


## License
This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

---

## AI assistance disclosure

This README file (README.md) was generated with assistance from an AI tool. Only this README file was created using AI help; all other files and code in the repository were implemented by the project authors.