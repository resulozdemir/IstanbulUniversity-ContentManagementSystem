# Istanbul University Content Management System Dashboard

A modern, modular, and extensible Content Management System (CMS) dashboard built with Angular for Istanbul University. This project provides a flexible platform for managing university websites, faculty pages, club sites, and more, with support for dynamic templates, themes, and component-based content.

## Features

- **Multi-site Management:** Easily manage multiple university sites (faculties, clubs, institutions) from a single dashboard.
- **Dynamic Templates & Themes:** Assign templates and themes to sites for consistent branding and layout.
- **Component-Based Architecture:** Add, configure, and preview reusable content components (e.g., hero, contact form, news, events).
- **Live Preview:** Instantly preview site changes and component updates.
- **Role-based Navigation:** Sidebar and navigation adapt to the selected site type (faculty, club, etc.).
- **API Integration:** Fetches site, template, and theme data from a backend API.
- **Responsive Design:** Modern, mobile-friendly UI using Angular and SCSS.

## Getting Started

### Prerequisites
- [Node.js](https://nodejs.org/) (v18 or newer recommended)
- [Angular CLI](https://angular.io/cli)

### Installation
```powershell
# Clone the repository
git clone https://github.com/IstanbulUniversity-ContentManagementSystem-dashboard.git
cd IstanbulUniversity-ContentManagementSystem-dashboard

# Install dependencies
npm install
```

### Running the Application
```powershell
# Start the development server
npm start
# or
ng serve
```
The app will be available at [http://localhost:4200](http://localhost:4200).

## Project Structure
- `src/app/components/` — UI components (layout manager, unified viewer, templates, etc.)
- `src/app/models/` — TypeScript interfaces for site, template, theme, and component data
- `src/app/services/` — Angular services for API communication and state management
- `src/app/pages/` — Page viewer and related logic
- `src/environments/` — Environment configuration

## Customization
- **Templates & Themes:** Add or modify templates/themes in the backend and connect via API.
- **Components:** Create new Angular components in `src/app/components/` and register them as needed.

## Contributing
Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.

## License
This project is for educational and internal use at Istanbul University. For other uses, please contact the project maintainers.

---
*Developed for Istanbul University as a flexible CMS dashboard solution.*