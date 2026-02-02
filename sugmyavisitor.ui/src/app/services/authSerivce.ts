
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private canAccessResetPassword = false;
  private readonly tenantValidatePath = "tenant/validate-tenant";
  constructor(private http: HttpClient) { }

  // Check if the user is logged in
  isLoggedIn(): boolean {
    //  logic for checking user session or token
    return !!sessionStorage.getItem('authToken');
  }

  // Optionally, methods for login and logout
  login(token: string): void {
    sessionStorage.setItem('authToken', token);
  }

  logout(): void {
    sessionStorage.removeItem('authToken');
  }

  // Allow access to the reset password page
  allowResetPasswordAccess(): void {
    this.canAccessResetPassword = true;
  }

  // Check if the user can access the reset password page
  canAccessResetPasswordPage(): boolean {
    return this.canAccessResetPassword;
  }
  validatedTenant(tenant: string): Observable<any> {
    return this.http.get(`${this.tenantValidatePath}?name=${tenant}`);
  }
}
