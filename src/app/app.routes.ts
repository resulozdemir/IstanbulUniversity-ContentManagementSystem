import { Routes } from '@angular/router';
import { UserLoginComponent } from './pages/user-login-page/user-login-page.component';
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout.component';

export const routes: Routes = [ // <-- `export` ekledik!
  { path: '', redirectTo: 'login', pathMatch: 'full' }, // Varsayılan olarak login sayfasına yönlendir
  { path: 'login', component: UserLoginComponent }, // Giriş sayfası
  {
    path: 'admin',
    component: AdminLayoutComponent, // Admin Layout içinde yönlendirmeler olacak
    children: [
      { path: '', loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule) }
    ]
  }
];
