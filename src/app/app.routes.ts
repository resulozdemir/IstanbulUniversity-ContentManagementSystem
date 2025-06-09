import { Routes } from '@angular/router';
import { DebugPanelComponent } from './components/debug-panel/debug-panel.component';
import { PageViewerComponent } from './pages/page-viewer/page-viewer.component';

export const routes: Routes = [
  { path: 'debug', component: DebugPanelComponent },
  { path: 'pages', component: PageViewerComponent },
  { path: '', redirectTo: 'pages', pathMatch: 'full' },
];
