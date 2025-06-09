import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Site, Template } from '../../../models/template.model';
import { Theme } from '../../../models/theme.model';
import { ComponentViewerComponent } from '../../component-viewer/component-viewer.component';

@Component({
  selector: 'app-fakulte-layout',
  standalone: true,
  imports: [CommonModule, ComponentViewerComponent],
  template: `
    <div class="fakulte-layout" [ngStyle]="getStyles()">
      <!-- Header -->
      <header class="fakulte-header" [innerHTML]="theme?.header"></header>
      
      <!-- Sidebar ve İçerik -->
      <div class="container-with-sidebar">
        <!-- Sidebar -->
        <aside class="fakulte-sidebar">
          <h3>Fakülte Menüsü</h3>
          <ul>
            <li><a href="#">Anasayfa</a></li>
            <li><a href="#">Akademik Kadro</a></li>
            <li><a href="#">Bölümler</a></li>
            <li><a href="#">Duyurular</a></li>
            <li><a href="#">Etkinlikler</a></li>
            <li><a href="#">İletişim</a></li>
          </ul>
        </aside>
        
        <!-- Main Content -->
        <main class="fakulte-content">
          <h1>{{ site?.name }}</h1>
          <p>{{ site?.description }}</p>
          
          <!-- Dinamik içerik alanı -->
          <div class="component-container">
            <!-- Örnek olarak varsayılan bileşenler -->
            <app-component-viewer componentId="about-content"></app-component-viewer>
          </div>
        </main>
      </div>
      
      <!-- Footer -->
      <footer class="fakulte-footer" [innerHTML]="theme?.footer"></footer>
    </div>
  `,
  styles: [`
    .fakulte-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    
    .fakulte-header {
      background-color: var(--primary-color, #0047AB);
      color: white;
      padding: 20px;
    }
    
    .container-with-sidebar {
      display: flex;
      flex: 1;
    }
    
    .fakulte-sidebar {
      width: 250px;
      background-color: #f5f5f5;
      padding: 20px;
      border-right: 1px solid #ddd;
    }
    
    .fakulte-sidebar h3 {
      color: var(--primary-color, #0047AB);
      margin-top: 0;
    }
    
    .fakulte-sidebar ul {
      list-style: none;
      padding: 0;
    }
    
    .fakulte-sidebar li {
      margin-bottom: 10px;
    }
    
    .fakulte-sidebar a {
      color: #333;
      text-decoration: none;
    }
    
    .fakulte-sidebar a:hover {
      color: var(--primary-color, #0047AB);
    }
    
    .fakulte-content {
      flex: 1;
      padding: 20px;
    }
    
    .fakulte-footer {
      background-color: #333;
      color: white;
      padding: 20px;
      text-align: center;
    }
    
    h1 {
      color: var(--primary-color, #0047AB);
    }
  `]
})
export class FakulteLayoutComponent {
  @Input() site: Site | null = null;
  @Input() template: Template | null = null;
  @Input() theme: Theme | null = null;
  
  getStyles() {
    // Tema ID'sine göre farklı renkler uygula
    if (this.theme?.id === 2) { // MDBF
      return {
        '--primary-color': '#0047AB', // Mavi
        '--secondary-color': '#FFD700' // Altın
      };
    } else if (this.theme?.id === 3) { // Fen Fakültesi
      return {
        '--primary-color': '#800080', // Mor
        '--secondary-color': '#00CED1' // Turkuaz
      };
    } else {
      return {
        '--primary-color': '#0047AB',
        '--secondary-color': '#FFD700'
      };
    }
  }
} 