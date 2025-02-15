# JOStore

JOStore is a Razor Pages project built with .NET 9.0. It includes features such as user authentication, session management, and integration with Stripe for payment processing.

## Features

- User Authentication with Identity
- Facebook Authentication
- Session Management
- Stripe Payment Integration
- Entity Framework Core for data access
- Razor Pages for UI

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022
- SQL Server

### Installation

1. Clone the repository:git clone https://github.com/yourusername/JOStore.git
2. Navigate to the project directory:cd JOStore
3. Restore the dependencies:dotnet restore
4. Update the database connection string in `appsettings.json`:"ConnectionStrings": {
    "DefaultConnection": "YourConnectionStringHere"
}
5. Apply the migrations to create the database:dotnet ef database update
6. Run the application:dotnet run

### Configuration

#### Stripe

Update the Stripe settings in `appsettings.json`:"Stripe": { "SecretKey": "YourSecretKeyHere", "PublishableKey": "YourPublishableKeyHere" }

#### Facebook Authentication

Update the Facebook authentication settings in `Program.cs`:builder.Services.AddAuthentication().AddFacebook(option => { option.AppId = "YourAppIdHere"; option.AppSecret = "YourAppSecretHere"; });


## Project Structure

- **JOStore**: Main project containing Razor Pages and configuration.
- **Store.DataAccess**: Data access layer with repositories and database context.
- **Store.Models**: Models used in the application.
- **Store.Utility**: Utility classes and helpers.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License.

