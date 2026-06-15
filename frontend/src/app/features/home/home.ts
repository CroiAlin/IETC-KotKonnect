import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  deconnexion(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
