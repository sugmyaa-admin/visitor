

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { VisitorService } from 'src/app/services/visitor.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/authSerivce';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
 

  constructor(
    private fb: FormBuilder, 
    private router: Router,
    private _visitor: VisitorService,
    private toastr: ToastrService,
    private authService: AuthService,
   
  ) {
    this.loginForm = this.fb.group({
      loginId: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      isRemember: [true]
    });
  }

  ngOnInit(): void {
    sessionStorage.removeItem('authToken');
    const savedUsername = localStorage.getItem('loginId');
    const savedPassword = localStorage.getItem('password');
    const savedRememberMe = JSON.parse(localStorage.getItem('rememberMe') || 'false');

    if (savedUsername) {
      this.loginForm.get('loginId')?.setValue(Number(savedUsername));
    }

    if (savedPassword) {
      this.loginForm.get('password')?.setValue(savedPassword);
    }

    this.loginForm.get('isRemember')?.setValue(savedRememberMe);
  }

  
  onSubmit(): void {
    if (this.loginForm.valid) {
      const { loginId, password } = this.loginForm.value;
  
      // Start the loading spinner (if you have implemented one)
  
      this._visitor.login(loginId, password).subscribe({
        next: (response: any) => {
          const token = response.data;
          // const personName=response.data.user;
          // const personEmail=response.data.email;
          // If login is successful and token is returned
          if (token) {
            sessionStorage.setItem('authToken', token);

            // this.userService.setUserDetails(personName, personEmail);
            this.toastr.success('Login successful!', 'Success');
            this.router.navigate(['/dashboard']); // Navigate to dashboard
          } else if (response.status && !response.status.success) {
            // Handle specific error messages from the server, like account locked
            this.toastr.error(response.status.message, 'Error');
          }
  
          // Handle "Remember Me" functionality using localStorage
          if (this.loginForm.get('isRemember')?.value) {
            // Convert loginId to a number before saving to localStorage
            localStorage.setItem('loginId', String(Number(loginId)));
           // Store as a stringified number
            localStorage.setItem('password', password);
            localStorage.setItem('rememberMe', JSON.stringify(this.loginForm.get('isRemember')?.value));
          } else {
            // Clear stored data if "Remember Me" is not selected
            localStorage.removeItem('loginId');
            localStorage.removeItem('password');
            localStorage.removeItem('rememberMe');
          }
        },
        error: (error: any) => {
          // Display error message based on response
          if (error.status && error.status.message) {
            // Show specific server error (like account locked)
            this.toastr.error(error.status.message, 'Error');
          } else {
            // Fallback error message if no specific message is available
            this.toastr.error('Login failed! Please check your credentials.', 'Error');
          }
        }
      });
    } else {
      // Form validation failed
      this.toastr.warning('Please fill in all required fields correctly.', 'Warning');
    }
  }
  

  resetPassword(): void {
    this.authService.allowResetPasswordAccess();
    this.router.navigate(['/reset-password']);
  }

  toggleRememberMe(): void {
    const rememberMeControl = this.loginForm.get('isRemember');
    if (rememberMeControl) {
      rememberMeControl.setValue(!rememberMeControl.value);
    }
  }
}

