import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComponentService } from '../../services/component.service';

@Component({
  selector: 'app-debug-panel',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="debug-panel">
      <h3>Component JavaScript Debugger</h3>

      <div class="form-group">
        <label for="componentId">Component ID:</label>
        <input
          type="number"
          id="componentId"
          [(ngModel)]="componentId"
          class="form-control"
        />
        <button (click)="loadJavaScript()" class="btn btn-primary">
          Load JavaScript
        </button>
      </div>

      <div *ngIf="loading" class="loading">Loading...</div>

      <div *ngIf="error" class="error">{{ error }}</div>

      <div *ngIf="jsCode" class="code-container">
        <h4>JavaScript Code</h4>
        <div class="stats">
          <p>Code Length: {{ jsCode.length }} characters</p>
          <p>
            Suspicious Characters: {{ suspiciousChars.join(', ') || 'None' }}
          </p>
        </div>
        <pre>{{ jsCode }}</pre>

        <div class="formatted-code">
          <h4>Prettified Code</h4>
          <pre>{{ formattedCode }}</pre>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .debug-panel {
        margin: 20px;
        padding: 20px;
        border: 1px solid #ddd;
        border-radius: 5px;
        background-color: #f8f9fa;
      }

      .form-group {
        margin-bottom: 15px;
      }

      .form-control {
        padding: 8px;
        margin-right: 10px;
        border: 1px solid #ced4da;
        border-radius: 4px;
      }

      .btn {
        padding: 8px 15px;
        border: none;
        border-radius: 4px;
        cursor: pointer;
      }

      .btn-primary {
        background-color: #007bff;
        color: white;
      }

      .loading {
        margin: 10px 0;
        color: #6c757d;
      }

      .error {
        margin: 10px 0;
        padding: 10px;
        background-color: #f8d7da;
        border: 1px solid #f5c6cb;
        border-radius: 4px;
        color: #721c24;
      }

      .code-container {
        margin-top: 20px;
      }

      pre {
        background-color: #f5f5f5;
        padding: 15px;
        border: 1px solid #ddd;
        border-radius: 4px;
        overflow-x: auto;
        white-space: pre-wrap;
        word-break: break-word;
        max-height: 400px;
        overflow-y: auto;
      }

      .stats {
        margin-bottom: 10px;
        padding: 10px;
        background-color: #e9ecef;
        border-radius: 4px;
      }

      .formatted-code {
        margin-top: 20px;
      }
    `,
  ],
})
export class DebugPanelComponent implements OnInit {
  componentId: number = 0;
  jsCode: string = '';
  formattedCode: string = '';
  loading: boolean = false;
  error: string = '';
  suspiciousChars: string[] = [];

  constructor(private componentService: ComponentService) {}

  ngOnInit(): void {}

  loadJavaScript(): void {
    if (!this.componentId) {
      this.error = 'Please enter a valid component ID';
      return;
    }

    this.loading = true;
    this.error = '';
    this.jsCode = '';
    this.formattedCode = '';
    this.suspiciousChars = [];

    this.componentService.getComponentJavaScript(this.componentId).subscribe({
      next: (code) => {
        this.jsCode = code;
        this.loading = false;

        // Find suspicious characters
        const suspiciousMatches = code.match(
          /[^\w\s\(\)\{\}\[\]\.\,\;\:\'\"\`\+\-\*\/\%\&\|\!\=\<\>\?\@\#\$\^\\]/g
        );
        if (suspiciousMatches) {
          this.suspiciousChars = [...new Set(suspiciousMatches)];
        }

        // Try to format the code
        try {
          // Simple formatting - replace consecutive spaces/tabs with a single space
          this.formattedCode = code
            .replace(/[\u0000-\u001F\u007F-\u009F]/g, '') // Remove control chars
            .replace(/\u200B|\u200C|\u200D|\uFEFF/g, '') // Remove invisible chars
            .replace(/\s+/g, ' ') // Replace multiple spaces with one
            .replace(/\{\s*/g, '{\n  ') // Format opening braces
            .replace(/\s*\}/g, '\n}') // Format closing braces
            .replace(/;\s*/g, ';\n  ') // Format semicolons
            .replace(/,\s*/g, ', ') // Format commas
            .replace(/\)\s*\{/g, ') {') // Format function declarations
            .replace(/\n\s*\n/g, '\n'); // Remove empty lines
        } catch (e) {
          this.formattedCode = '// Error formatting code: ' + e;
        }
      },
      error: (err) => {
        this.error = `Error loading JavaScript: ${err.message}`;
        this.loading = false;
      },
    });
  }
}
