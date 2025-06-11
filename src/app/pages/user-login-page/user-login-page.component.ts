import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-login',
  templateUrl: './user-login-page.component.html',
  styleUrls: ['./user-login-page.component.scss']
})
export class UserLoginComponent {
  constructor(private router: Router) { }

  login() {
    // Burada ilerleyen süreçte auth servisi ile doğrulama ekleyebilirsin
    this.router.navigate(['/admin']); // Giriş yapınca admin dashboard'a yönlendir
  }
}
