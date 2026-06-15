import { CanActivateFn , Router} from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth';
import { Role } from '../models/auth.models';

export const roleGuard = (...rolesAutorises: Role[]): CanActivateFn => {
  return (route, state) => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const user = auth.user();

    if (!user){
      return router.createUrlTree(['/login']);
    }
    else if (rolesAutorises.includes(user.role)){
      return true;
    }
    else 
    {
      return router.createUrlTree(['/']);
    }
  };
}
