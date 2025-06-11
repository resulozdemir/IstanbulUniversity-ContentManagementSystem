import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DashboardPageComponent } from '../pages/dashboard-page/dashboard-page.component';
import { PagesPageComponent } from '../pages/pages-page/pages-page.component';
import { ThemesPageComponent } from '../pages/themes-page/themes-page.component';
import { ComponentsPageComponent } from '../pages/components-page/components-page.component';
import { SubdomainsPageComponent } from '../pages/subdomains-page/subdomains-page.component';
import { MediaPageComponent } from '../pages/media-page/media-page.component';
import { UsersPageComponent } from '../pages/users-page/users-page.component';
import { SettingsPageComponent } from '../pages/settings-page/settings-page.component';

@NgModule({
  imports: [
    CommonModule,
    AdminRoutingModule,
    DashboardComponent, // Moved from declarations to imports since it's a standalone component
    DashboardPageComponent,
    PagesPageComponent,
    ThemesPageComponent,
    ComponentsPageComponent,
    SubdomainsPageComponent,
    MediaPageComponent,
    UsersPageComponent,
    SettingsPageComponent
  ]
})
export class AdminModule { }
