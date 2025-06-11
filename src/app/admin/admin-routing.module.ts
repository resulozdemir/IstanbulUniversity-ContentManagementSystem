import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DashboardPageComponent } from '../pages/dashboard-page/dashboard-page.component';
import { PagesPageComponent } from '../pages/pages-page/pages-page.component';
import { ThemesPageComponent } from '../pages/themes-page/themes-page.component';
import { ComponentsPageComponent } from '../pages/components-page/components-page.component';
import { SubdomainsPageComponent } from '../pages/subdomains-page/subdomains-page.component';
import { MediaPageComponent } from '../pages/media-page/media-page.component';
import { UsersPageComponent } from '../pages/users-page/users-page.component';
import { SettingsPageComponent } from '../pages/settings-page/settings-page.component';
import { AddPageComponent } from '../pages/add-page/add-page.component';
import { AddComponentPageComponent } from '../pages/add-component-page/add-component-page.component';

const routes: Routes = [
  { path: '', component: DashboardComponent }, // /admin sayfası direkt dashboard'u açsın
  { path: 'dashboard', component: DashboardPageComponent },
  { path: 'pages', component: PagesPageComponent },
  { path: 'themes', component: ThemesPageComponent },
  { path: 'components', component: ComponentsPageComponent },
  { path: 'add-component', component: AddComponentPageComponent },
  { path: 'subdomains', component: SubdomainsPageComponent },
  { path: 'media', component: MediaPageComponent },
  { path: 'users', component: UsersPageComponent },
  { path: 'settings', component: SettingsPageComponent },
  { path: 'add-page', component: AddPageComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
