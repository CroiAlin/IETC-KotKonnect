import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/api/auth';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class Navbar {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  deconnexion(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
