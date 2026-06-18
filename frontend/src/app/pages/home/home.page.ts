import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/api/auth';

@Component({
  selector: 'app-home-page',
  imports: [],
  templateUrl: './home.page.html',
  styleUrl: './home.page.css',
})
export class HomePage {
  readonly auth = inject(AuthService);
}
