import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Site, Template } from '../../../models/template.model';
import { Theme } from '../../../models/theme.model';
import { ComponentViewerComponent } from '../../component-viewer/component-viewer.component';

@Component({
  selector: 'app-kulup-layout',
  standalone: true,
  imports: [CommonModule, ComponentViewerComponent],
  template: `
    <div class="kulup-layout" [ngStyle]="getStyles()">
      <!-- Header -->
      <header class="kulup-header">
        <div [innerHTML]="theme?.header"></div>
        <nav class="kulup-nav">
          <ul>
            <li><a href="#">Anasayfa</a></li>
            <li><a href="#">Etkinlikler</a></li>
            <li><a href="#">Üyelik</a></li>
            <li><a href="#">Galeri</a></li>
            <li><a href="#">Hakkımızda</a></li>
            <li><a href="#">İletişim</a></li>
          </ul>
        </nav>
      </header>
      
      <!-- Banner -->
      <div class="kulup-banner">
        <h1>{{ site?.name }}</h1>
        <p>{{ site?.description }}</p>
      </div>
      
      <!-- Main Content -->
      <main class="kulup-content">
        <div class="container">
          <!-- Dinamik içerik alanı -->
          <div class="component-container">
            <!-- Örnek olarak varsayılan bileşenler -->
            <app-component-viewer componentId="hero"></app-component-viewer>
            <app-component-viewer componentId="contact-form"></app-component-viewer>
          </div>
          
          <!-- Sosyal Medya -->
          <div class="social-media">
            <h3>Bizi Takip Edin</h3>
            <div class="social-icons">
              <a href="#" class="icon">Instagram</a>
              <a href="#" class="icon">Twitter</a>
              <a href="#" class="icon">Facebook</a>
            </div>
          </div>
        </div>
      </main>
      
      <!-- Footer -->
      <footer class="kulup-footer" [innerHTML]="theme?.footer"></footer>
    </div>
  `,
  styles: [`
    .kulup-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    
    .kulup-header {
      background-color: var(--primary-color, #800080);
      color: white;
      padding: 20px;
    }
    
    .kulup-nav ul {
      list-style: none;
      padding: 0;
      display: flex;
      margin-top: 15px;
    }
    
    .kulup-nav li {
      margin-right: 20px;
    }
    
    .kulup-nav a {
      color: white;
      text-decoration: none;
      font-weight: bold;
      padding: 5px 10px;
      border-radius: 3px;
    }
    
    .kulup-nav a:hover {
      background-color: rgba(255, 255, 255, 0.2);
    }
    
    .kulup-banner {
      background-color: var(--secondary-color, #E0B200);
      color: white;
      padding: 40px 20px;
      text-align: center;
    }
    
    .kulup-banner h1 {
      margin-top: 0;
      font-size: 2.5em;
    }
    
    .kulup-content {
      flex: 1;
      padding: 20px;
    }
    
    .container {
      max-width: 1200px;
      margin: 0 auto;
    }
    
    .social-media {
      margin-top: 30px;
      text-align: center;
    }
    
    .social-media h3 {
      color: var(--primary-color, #800080);
    }
    
    .social-icons {
      display: flex;
      justify-content: center;
      gap: 20px;
    }
    
    .social-icons .icon {
      color: var(--primary-color, #800080);
      text-decoration: none;
      font-weight: bold;
    }
    
    .kulup-footer {
      background-color: #333;
      color: white;
      padding: 20px;
      text-align: center;
    }
  `]
})
export class KulupLayoutComponent {
  @Input() site: Site | null = null;
  @Input() template: Template | null = null;
  @Input() theme: Theme | null = null;
  
  getStyles() {
    return {
      '--primary-color': '#800080', // Mor
      '--secondary-color': '#E0B200' // Altın sarısı
    };
  }
} 