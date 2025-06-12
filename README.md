# IU Backend Dashboard

A modern, modular admin dashboard for content management, built with Angular 19 and Material Design. This project is designed for managing sites, pages, components, themes, users, and media in a scalable, maintainable way.

## 🚀 Features

- **Site Management**: Create and manage multiple sites and subdomains
- **Page Management**: Add, edit, and organize content pages
- **Component System**: Reusable UI components for rapid development
- **Theme Management**: Switch and customize themes
- **User Management**: Register, login, and manage users
- **Media Library**: Upload and manage images and documents
- **Settings**: Configure application and site settings
- **Responsive Design**: Works on desktop and mobile

## 🛠️ Technology Stack

- **Angular 19**
- **Angular Material**
- **Bootstrap 5**
- **RxJS**
- **TypeScript**
- **Tailwind CSS (via PostCSS)**

## 📁 Project Structure

```
src/
  app/
    admin/           # Admin dashboard modules
    layouts/         # Layout components
    models/          # TypeScript models
    pages/           # Feature pages (dashboard, media, users, etc.)
    services/        # API and business logic services
    shared/          # Shared components and utilities
  environments/      # Environment configs
public/              # Static assets
```

## 🚦 Getting Started

### Prerequisites

- Node.js 18+
- npm 9+
- Angular CLI 19+

### Installation

1. **Clone the repository**
   ```powershell
   git clone https://github.com/your-org/iu-backend-dashboard.git
   cd iu-backend-dashboard
   ```
2. **Install dependencies**
   ```powershell
   npm install
   ```
3. **Configure API URL**

   - Edit `src/environments/environment.ts` and set `apiUrl` to your backend endpoint.

4. **Run the development server**
   ```powershell
   ng serve
   ```
   Open [http://localhost:4200](http://localhost:4200) in your browser.

## 🧪 Testing

- **Unit tests**: `ng test`
- **E2E tests**: `ng e2e` (configure your preferred e2e framework)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to your branch (`git push origin feature/your-feature`)
5. Open a Pull Request

## 📄 License

This project is developed for Istanbul University. All rights reserved.

---

**IU Backend Dashboard** – Modern content management for academic and enterprise needs.
