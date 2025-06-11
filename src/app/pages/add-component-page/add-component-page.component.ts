import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpClientModule } from '@angular/common/http';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {
  ThemeComponentService,
  Theme,
} from '../../services/theme-component.service';
import { ComponentService } from '../../services/component.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-add-component-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatProgressSpinnerModule,
    HttpClientModule,
    MatSnackBarModule,
    RouterModule,
  ],
  templateUrl: './add-component-page.component.html',
  styleUrl: './add-component-page.component.css',
})
export class AddComponentPageComponent implements OnInit {
  componentForm: FormGroup;
  themes: Theme[] = [];
  isLoading = false;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private themeComponentService: ThemeComponentService,
    private componentService: ComponentService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {
    this.componentForm = this.fb.group({
      themeId: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      template: ['', Validators.required],
      style: ['/* Bu bileşen için CSS kodu henüz eklenmemiştir. */'],
      javascript: ['// Bu bileşen için JavaScript kodu henüz eklenmemiştir.'],
    });
  }

  ngOnInit(): void {
    this.loadThemes();
  }

  loadThemes(): void {
    this.isLoading = true;
    this.themeComponentService.getThemes().subscribe({
      next: (themes) => {
        this.themes = themes;
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error('Error loading themes:', error);
        this.snackBar.open('Temalar yüklenirken hata oluştu!', 'Tamam', {
          duration: 3000,
        });
        this.isLoading = false;
      },
    });
  }

  generateUniqueId(): number {
    return Math.floor(100000 + Math.random() * 900000); // 6 digit unique number
  }

  onSubmit(): void {
    if (this.componentForm.invalid) {
      this.snackBar.open('Lütfen tüm zorunlu alanları doldurun.', 'Tamam', {
        duration: 3000,
      });
      return;
    }

    this.isSubmitting = true;
    const formValues = this.componentForm.value;

    try {
      // First, create the base component
      const componentId = this.generateUniqueId();
      const componentData = {
        id: componentId,
        name: formValues.name,
        description: formValues.description,
        template: 'empty',
        style: 'empty',
        formjson: 'empty',
        tagName: 'empty',
        column1: 'empty',
        column2: 'empty',
        column3: 'empty',
        column4: 'empty',
      };

      // API'ye gönderilecek veriyi konsola yazdıralım
      console.log('Creating component with ID:', componentId);
      console.log('Component data:', JSON.stringify(componentData));

      this.componentService.createComponent(componentData).subscribe({
        next: (response) => {
          console.log('Component created successfully:', response);

          // Then create the theme component
          const themeComponentId = this.generateUniqueId();
          const themeComponentData = {
            id: themeComponentId,
            themeId: parseInt(formValues.themeId),
            componentId: componentId,
            name: formValues.name,
            description: formValues.description,
            template: formValues.template,
            style: formValues.style,
            javascript: formValues.javascript,
            formJson: null,
            formHtml: null,
            formJs: null,
            isDeleted: 0,
          };

          // API'ye gönderilecek veriyi konsola yazdıralım
          console.log('Creating theme component with ID:', themeComponentId);
          console.log(
            'Theme component data:',
            JSON.stringify(themeComponentData)
          );

          this.componentService
            .createThemeComponent(themeComponentData)
            .subscribe({
              next: (response) => {
                console.log('Theme component created successfully:', response);
                this.snackBar.open('Bileşen başarıyla oluşturuldu!', 'Tamam', {
                  duration: 3000,
                });
                this.isSubmitting = false;
                this.router.navigate(['/admin/components']);
              },
              error: (error: any) => {
                console.error('Error creating theme component:', error);
                // Hata mesajını detaylı gösterelim
                const errorMsg =
                  error.error?.message ||
                  error.message ||
                  'Tema bileşeni oluşturulurken hata oluştu!';
                this.snackBar.open(errorMsg, 'Tamam', {
                  duration: 3000,
                });
                this.isSubmitting = false;
              },
            });
        },
        error: (error: any) => {
          console.error('Error creating component:', error);
          // Hata mesajını detaylı gösterelim
          const errorMsg =
            error.error?.message ||
            error.message ||
            'Bileşen oluşturulurken hata oluştu!';
          this.snackBar.open(errorMsg, 'Tamam', {
            duration: 3000,
          });
          this.isSubmitting = false;
        },
      });
    } catch (error) {
      console.error('Exception during component creation:', error);
      this.snackBar.open('Bileşen oluşturulurken bir hata oluştu!', 'Tamam', {
        duration: 3000,
      });
      this.isSubmitting = false;
    }
  }
}
