# Istanbul University - Content Management System

A modern, scalable Content Management System built with .NET Core following Clean Architecture principles.

## 🏗️ Architecture

This project implements **Clean Architecture** with the following layers:

- **Domain Layer**: Core business entities and interfaces
- **Application Layer**: Business logic, DTOs, and service interfaces  
- **Infrastructure Layer**: Data access, repositories, and external services
- **WebApi Layer**: REST API controllers and endpoints

## 🚀 Features

- **Multi-Site Management**: Support for multiple websites
- **Page Management**: Create and manage web pages with templates
- **Content Management**: Rich content creation and editing
- **Menu System**: Dynamic navigation menu management
- **News & Notices**: Built-in news and announcement system
- **Component System**: Reusable page components
- **Theme Support**: Multiple theme management
- **File Upload**: Document and image management
- **Sitemap Generation**: Automatic sitemap creation

## 🛠️ Technology Stack

- **.NET Core**: Backend framework
- **ASP.NET Core Web API**: RESTful API development
- **Entity Framework Core**: Object-Relational Mapping
- **Clean Architecture**: Project structure pattern
- **DTOs**: Data Transfer Objects for API communication
- **Dependency Injection**: Built-in IoC container

## 📁 Project Structure

```
new_cms/
├── Application/           # Application layer
│   ├── DTOs/             # Data Transfer Objects
│   ├── Interfaces/       # Service interfaces
│   ├── Mappings/         # Object mappings
│   └── Services/         # Business logic services
├── Domain/               # Domain layer
│   ├── Entities/         # Core business entities
│   └── Interfaces/       # Domain interfaces
├── Infrastructure/       # Infrastructure layer
│   ├── Persistence/      # Database context
│   └── Repositories/     # Data access layer
├── WebApi/              # Presentation layer
│   └── Controllers/      # API controllers
└── wwwroot/             # Static files
    └── uploads/         # File uploads
```

## 🚦 Getting Started

### Prerequisites

- .NET 6.0 or later
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/resulozdemir/IstanbulUniversity-ContentManagementSystem.git
   cd IstanbulUniversity-ContentManagementSystem
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   - Open `appsettings.json`
   - Update the connection string to match your database setup

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the API**
   - API: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`

## 📖 API Documentation

The API provides comprehensive endpoints for:

- **Pages**: CRUD operations for web pages
- **Content**: Content management and editing
- **Menus**: Navigation menu management
- **Sites**: Multi-site configuration
- **News**: News article management
- **Notices**: Announcement system
- **Templates**: Page template management
- **Components**: Reusable page components
- **Themes**: Theme and styling management
- **Uploads**: File and image management

### Example API Endpoints

```http
GET    /api/pages/paged              # Get paginated pages
GET    /api/pages/active             # Get active pages
GET    /api/pages/{id}               # Get page by ID
POST   /api/pages                    # Create new page
PUT    /api/pages/{id}               # Update page
DELETE /api/pages/{id}               # Delete page (soft delete)
```

## 🔧 Configuration

### Database Configuration

Update your connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NewCmsDb;Trusted_Connection=true;"
  }
}
```

### File Upload Configuration

File uploads are stored in `wwwroot/uploads/`:
- `documents/`: Document files
- `images/`: Image files  
- `temp/`: Temporary uploads

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 Code Standards

- Follow Clean Architecture principles
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Write unit tests for business logic
- Follow C# coding conventions

## 📄 License

This project is developed for Istanbul University. All rights reserved.

## 👥 Authors

- **Resul Özdemir** - *Initial work* - [resulozdemir](https://github.com/resulozdemir)

## 📞 Support

For support and questions:
- Create an issue in this repository
- Contact the development team

---

**Istanbul University - Content Management System** - Building modern web solutions for academic institutions.