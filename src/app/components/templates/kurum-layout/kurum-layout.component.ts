import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Site, Template } from '../../../models/template.model';
import { Theme } from '../../../models/theme.model';
import { ComponentViewerComponent } from '../../component-viewer/component-viewer.component';

@Component({
  selector: 'app-kurum-layout',
  standalone: true,
  imports: [CommonModule, ComponentViewerComponent],
  template: `
    <div class="kurum-layout" [ngStyle]="getStyles()">
      <!-- Header -->
      <header class="kurum-header" [innerHTML]="theme?.header"></header>
      
      <!-- Main Content -->
      <main class="kurum-content">
        <div class="container">
          <h1>{{ site?.name }}</h1>
          <p>{{ site?.description }}</p>
          
          <!-- Dinamik içerik alanı -->
          <div class="component-container">
            <!-- Örnek olarak varsayılan bileşenler -->
            <app-component-viewer componentId="navbar"></app-component-viewer>
            <app-component-viewer componentId="hero"></app-component-viewer>
            <app-component-viewer componentId="footer"></app-component-viewer>
          </div>
        </div>
      </main>
      
      <!-- Footer -->
      <footer class="kurum-footer" [innerHTML]="theme?.footer"></footer>
    </div>
  `,
  styles: [`
    .kurum-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    
    .kurum-header {
      background-color: #00733E;
      color: white;
      padding: 20px;
    }
    
    .kurum-content {
      flex: 1;
      padding: 20px;
    }
    
    .container {
      max-width: 1200px;
      margin: 0 auto;
    }
    
    .kurum-footer {
      background-color: #333;
      color: white;
      padding: 20px;
      text-align: center;
    }
    
    h1 {
      color: #00733E;
    }
  `]
})
export class KurumLayoutComponent {
  @Input() site: Site | null = null;
  @Input() template: Template | null = null;
  @Input() theme: Theme | null = null;
  
  getStyles() {
    return {
      // Tema bazlı stil değişiklikleri burada uygulanabilir
      '--primary-color': '#00733E',
      '--secondary-color': '#E0B200'
    };
  }
} 