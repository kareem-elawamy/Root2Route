# Root2Route Frontend 🌐

![Angular](https://img.shields.io/badge/Angular-17+-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-007ACC?style=for-the-badge&logo=typescript&logoColor=white)
![RxJS](https://img.shields.io/badge/RxJS-B7178C?style=for-the-badge&logo=reactivex&logoColor=white)

The frontend application for **Root2Route**, a robust B2B logistics and trading platform. Built entirely with modern **Angular 17+** features, this platform relies entirely on **Angular Signals** for reactive state management, completely eliminating legacy `ChangeDetectorRef` patterns.

## ✨ Key Features

- **Signals-Based Reactivity:** The entire application state (from data grids to auth context) is managed using Angular `signal`, `computed`, and `effect`.
- **Role-Based Access Control (RBAC):** UI elements, sidebar navigation, and routes are dynamically gated based on JWT permission claims (`auth.hasPermission()`).
- **Modern Routing:** Standalone components with lazy-loaded routes and strict `permissionGuard` protection.
- **Smart Polling:** Network-efficient RxJS `switchMap` patterns for live features like Chat, avoiding unnecessary polling when components are idle.
- **Reusable UI:** A robust library of standalone components (Modals, Pagination, Toasts, Confirm Dialogs) built for consistency and speed.

## 🚀 Getting Started

### Prerequisites
- [Node.js](https://nodejs.org/) (v18+)
- Angular CLI (`npm install -g @angular/cli`)

### Installation & Setup

1. **Install Dependencies:**
   ```bash
   npm install
   ```

2. **Development Server:**
   Start the local development server:
   ```bash
   ng serve
   ```
   Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

3. **Production Build:**
   Compile the application for production:
   ```bash
   ng build --configuration=production
   ```
   The build artifacts will be stored in the `dist/` directory.

## 🏗️ Architecture Standards

- **Standalone Components Only:** No `NgModules`. Every component defines its own imports.
- **Strict Dependency Injection:** Uses the modern `inject()` function pattern instead of constructor injection.
- **No `alert()` or `confirm()`:** All user prompts use the internal `ToastService` and `ConfirmDialogService`.
- **Smart vs. Dumb Components:** Pages (Smart) handle data fetching and pass signals down to visual pieces (Dumb).
