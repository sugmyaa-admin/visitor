
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../../services/authSerivce';  // Adjust the path as necessary

@Injectable({
  providedIn: 'root'
})
export class ResetPasswordAccessGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    // Check if the user can access the reset password page
    if (this.authService.canAccessResetPasswordPage()) {
      // Allow access if the user clicked the link
      return true;
    } else {
      // Redirect to login if not accessed via link
      this.router.navigate(['/login']);
      return false;
    }
  }
}
